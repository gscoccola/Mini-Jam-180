using System.Collections;
using TMPro;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] int _totalHitsToStun;
    [SerializeField] int _totalPhases;

    [SerializeField] float _throwProbability;
    [SerializeField] float _chargeProbability;

    [SerializeField] float MoveSpeedSlow;
    [SerializeField] float MoveSpeedFast;
    [SerializeField] float PosAdjustDistance;
    [SerializeField] float IdleReturnDistance;

    [SerializeField] float Acceleration;
    [SerializeField] float TargetXOffset;

    [SerializeField] float AttackDelay;
    [SerializeField] float AttackMovementPause;
    [SerializeField] float MaxYOffsetToAttack;
    [SerializeField] float MaxXOffsetToCigaretteAttack;
    [SerializeField] float _rockMinVelocity;


    [SerializeField] float _chargePrepareTime1;
    [SerializeField] float _chargePrepareTime2;
    [SerializeField] float _chargePrepareSpeed;
    [SerializeField] float _chargeAttackSpeed;
    [SerializeField] float _chargeXRange;
    [SerializeField] float _velocityCorrectionFactor;


    [SerializeField] float MaxDistanceToPunchAttack;
    [SerializeField] float _punchFixedSpeed;
    [SerializeField] float _fistReturnSpeed;
    [SerializeField] float _fistReturnTime;


    [SerializeField] float hoverXOffset;
    [SerializeField] float hoverYOffset;

    [SerializeField] Vector3 _leftHandCenter;
    [SerializeField] Vector3 _rightHandCenter;
    [SerializeField] float _elasticConstant;
    [SerializeField] float _swayRandomizePeriod;
    [SerializeField] float _swayCenterAmplitude;
    Vector3 _leftHandTarget;
    Vector3 _rightHandTarget;

    [SerializeField] GameObject Cigarette;
    [SerializeField] GameObject Rock;
    [SerializeField] GameObject Explosion1;
    [SerializeField] GameObject Explosion2;
    [SerializeField] Rigidbody2D RightHand;
    [SerializeField] Rigidbody2D LeftHand;
    [SerializeField]  Animator _cigarAnimator;
    [SerializeField]  GameObject _chargeIndicator;
    [SerializeField]  Rigidbody2D _chargeBoss;

    Animator _animator;
    Transform _playerTransform;
    Rigidbody2D _rigidbody;
    GameObject _enemies;
    BossFightManager  _manager;
    SemiRandomIntGenerator _intGenerator;
    Animator _rightHandAnimator;


    bool _rangedAttackOnCooldown;
    bool _punchAttackOnCooldown;
    int _hitsLeftToStun;
    int _currentPhase = 0;
    bool _normalMovement = true;
    bool _returnHand = false;
    Camera _camera;

    BossState _bossState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
        _manager = FindAnyObjectByType<BossFightManager>();
        _rightHandAnimator = RightHand.GetComponent<Animator>();
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Start()
    {
        StartCoroutine(IRandomizeHandTarget());
        _bossState = BossState.StandBy;
        _hitsLeftToStun = _totalHitsToStun;
        _intGenerator = new SemiRandomIntGenerator(new float[2]{ _throwProbability, _chargeProbability }, 1);
    }


    private void Update()
    {
        MoveHandToTarget(ref RightHand, _rightHandTarget);
        MoveHandToTarget(ref LeftHand, _leftHandTarget);

        if (_bossState == BossState.Dead)
        {
            _rigidbody.linearVelocity = new Vector2(0f, -MoveSpeedSlow/5f);
            return;
        }
            

        Vector3 relativePlayerPos = _playerTransform.position - transform.position;

        if (_bossState == BossState.Idle) Attack(relativePlayerPos);
        if (_bossState == BossState.PunchAttack || (_bossState == BossState.ThrowAttack && _returnHand == true)) 
            _rightHandTarget = ReturnTargetToCenter(_rightHandTarget, _rightHandCenter);
        if (_bossState == BossState.Stunned && transform.position.x > _manager._cameraEdgeX + 5f) TeleportAndReset();
        if (_bossState == BossState.Celebrating)
        {
            _rightHandTarget = ReturnTargetToCenter(_rightHandTarget, _rightHandCenter);
            _leftHandTarget = ReturnTargetToCenter(_leftHandTarget, _leftHandCenter);
        }

        AdjustPosition(_bossState, relativePlayerPos);
        Move(_bossState, relativePlayerPos);
    }

    private void AdjustPosition(BossState bossState, Vector3 relativePlayerPos)
    {
        if (bossState == BossState.Idle && (relativePlayerPos.x < 0f
            || (relativePlayerPos + TargetXOffset * Vector3.left).magnitude > PosAdjustDistance))
            _bossState = BossState.AdjustingPosition;

        else if (_bossState == BossState.AdjustingPosition && (relativePlayerPos + TargetXOffset * Vector3.left).magnitude < IdleReturnDistance)
            _bossState = BossState.Idle;
    }


    private void Attack(Vector3 relativePlayerPos)
    {

        if (!_punchAttackOnCooldown && _bossState == BossState.Idle && relativePlayerPos.magnitude <= MaxDistanceToPunchAttack)
            StartCoroutine(IPunchAttack(relativePlayerPos));

        else if (!_rangedAttackOnCooldown && _bossState == BossState.Idle && Mathf.Abs(relativePlayerPos.y) <= MaxYOffsetToAttack &&
            relativePlayerPos.x > 0 && relativePlayerPos.x <= MaxXOffsetToCigaretteAttack)
        {
            
            switch (_intGenerator.GetInt())
            {
                case 0:
                    StartCoroutine(ICigaretteAttack());
                    break;
                case 1:
                    StartCoroutine(IThrowAttack());
                    break;
                case 2:
                    StartCoroutine(IChargeAttack());
                    break;
            }
        }
            
    }

    public IEnumerator ICigaretteAttack(bool pauseAfterShot = false)
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossCigarAttack);
        _bossState = BossState.CigarAttack;
        _normalMovement = false;
        _animator.SetTrigger("Shoot");
        _rangedAttackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        Instantiate(Cigarette, transform.position + new Vector3(-hoverXOffset, -hoverYOffset, 0f), Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(AttackMovementPause - 0.2f);
        _bossState = pauseAfterShot ? BossState.StandBy : BossState.Idle;
        _normalMovement = true;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _rangedAttackOnCooldown = false;
    }

    private IEnumerator IThrowAttack()
    {
        _bossState = BossState.ThrowAttack;
        _rightHandAnimator.SetBool("Open", true);
        _returnHand = false;
        _normalMovement = false;
        _rangedAttackOnCooldown = true;
        _rightHandTarget = new Vector3(-4.5f, 0.5f, 0f);
        yield return new WaitForSeconds(0.4f);
        _rightHandTarget = new Vector3(1f, 1f, 0f);
        _animator.SetTrigger("Frown");
        yield return new WaitForSeconds(_fistReturnTime / 8f);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossThrow);
        yield return new WaitForSeconds(_fistReturnTime / 8f);
        SpawnRockWave();
        _normalMovement = true;
        yield return new WaitForSeconds(0.3f);
        _returnHand = true;
        yield return new WaitForSeconds(0.1f);
        _rightHandAnimator.SetBool("Open", false);
        yield return new WaitForSeconds(_fistReturnTime * 3f / 4f);
        
        _bossState = BossState.Idle;
        yield return new WaitForSeconds(AttackDelay - _fistReturnTime - 0.2f);
        _rangedAttackOnCooldown = false;
    }

    private IEnumerator IPunchAttack(Vector3 relativePlayerPos)
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossPunch);
        _animator.SetTrigger("Frown");
        _bossState = BossState.PunchAttack;
        RightHand.GetComponent<Fist>()._SFXAvailable = true;
        _rightHandTarget = relativePlayerPos.normalized * MaxDistanceToPunchAttack;
        _punchAttackOnCooldown = true;
        yield return new WaitForSeconds(_fistReturnTime);
        _bossState = BossState.Idle;
        RightHand.GetComponent<Fist>()._SFXAvailable = false;
        yield return new WaitForSeconds(AttackDelay - _fistReturnTime);
        _punchAttackOnCooldown = false;
    }

    private IEnumerator IChargeAttack()
    {
        _bossState = BossState.ChargeAttackPrepare;
        yield return new WaitForSeconds(_chargePrepareTime1 - 0.2f);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossChargeWarning);
        yield return new WaitForSeconds(0.2f);
        
        _chargeIndicator.SetActive(true);
        _chargeIndicator.GetComponent<Animator>().SetTrigger("Expand");
        _chargeIndicator.transform.position = new Vector3(_playerTransform.transform.position.x 
            + _playerTransform.GetComponent<Rigidbody2D>().linearVelocityX * _velocityCorrectionFactor
            + Random.Range(-1f, 1f) * _chargeXRange / 2f, 18f, 0f);
        yield return new WaitForSeconds(_chargePrepareTime2);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossCharge);
        Vector3 teleportPostition = new Vector3(_chargeIndicator.transform.position.x, -3.5f, 0f);
        _chargeBoss.transform.position = teleportPostition;
        _chargeBoss.linearVelocityY = _chargeAttackSpeed;
        _bossState = BossState.ChargeAttackExecute;
        yield return new WaitForSeconds(0.3f);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossChargeWoosh);
        yield return new WaitForSeconds(1.7f);
        TeleportAndReset();
        _chargeIndicator.SetActive(false);
    }


    private void Move(BossState bossState, Vector3 relativePlayerPos)
    {
        if (_bossState == BossState.StandBy) return;

        

        if (_bossState == BossState.AdjustingPosition)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (relativePlayerPos + TargetXOffset * Vector3.left).normalized
            * MoveSpeedFast, 5f * Acceleration * Time.deltaTime);
        }
        else if (_bossState == BossState.Idle || _bossState == BossState.PunchAttack
            || (_normalMovement ==true && (_bossState == BossState.CigarAttack || _bossState == BossState.ThrowAttack)))
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (relativePlayerPos + TargetXOffset * Vector3.left).normalized
            * MoveSpeedSlow, Acceleration * Time.deltaTime);
        }
        else if (_bossState == BossState.PhaseChange)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            MoveSpeedFast * Vector3.down, Acceleration * Time.deltaTime);
        }
        else if (_bossState == BossState.Stunned)
        {
            _rigidbody.linearVelocityY = Vector2.MoveTowards(_rigidbody.linearVelocity, 
                (transform.position.y - 8f) * Vector3.down
            * MoveSpeedSlow/5f, Acceleration  * Time.deltaTime / 2f).y;
            _rigidbody.linearVelocityX = /*-_stunnedXSpeed/8f*/ 2f;
            
        }
        else if(_bossState == BossState.CigarAttack || _bossState == BossState.ThrowAttack)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.zero, 6f * Acceleration * Time.deltaTime);
        }
        else if(_bossState == BossState.ChargeAttackPrepare)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.down * _chargePrepareSpeed, 4f * Acceleration * Time.deltaTime);
        }
        else if(_bossState == BossState.ChargeAttackExecute)
        {
            _rigidbody.linearVelocity = Vector2.zero;
        }
        else if (_bossState == BossState.Celebrating)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
                (_camera.transform.position - transform.position).normalized
            * MoveSpeedSlow, Acceleration * Time.deltaTime / 2f);
        }

    }

    private void MoveHandToTarget(ref Rigidbody2D hand, Vector3 center)
    {
        Vector3 direction = center + transform.position - hand.transform.position;
        hand.AddForce(direction * _elasticConstant * hand.mass * Time.deltaTime);
    }

    private IEnumerator IRandomizeHandTarget()
    {
        while (true)
        {
            if (_bossState == BossState.PunchAttack) yield return new WaitForSeconds(_swayRandomizePeriod);
            if (_bossState == BossState.Celebrating) yield break;
            _leftHandTarget = RandomizeHandTarget(_leftHandCenter);
            _rightHandTarget = RandomizeHandTarget(_rightHandCenter);
            yield return new WaitForSeconds(_swayRandomizePeriod);
        }
    }

    private Vector3 RandomizeHandTarget(Vector3 center)
    {
        Vector2 newTarget = Random.insideUnitCircle * _swayCenterAmplitude;
        return center + new Vector3(newTarget.x, newTarget.y, 0f);
    }

    private Vector3 ReturnTargetToCenter(Vector3 target, Vector3 center)
    {
        return Vector3.MoveTowards(target, center, _fistReturnSpeed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        
        _rigidbody.linearVelocityX += -7f;
        Interrupt();
        StartCoroutine(ITakeDamage());
    }

    private void Interrupt()
    {
        StopAllCoroutines();
        _rightHandTarget = RandomizeHandTarget(_rightHandCenter);
        _rangedAttackOnCooldown = false;
        _punchAttackOnCooldown = false;
        _rightHandAnimator.SetBool("Open", false);
    }

    private IEnumerator ITakeDamage()
    {
        _animator.SetTrigger("TakeDamage");
        _bossState = BossState.Damaged;
        
        
        if (_hitsLeftToStun == 1) 
        {
            yield return new WaitForSeconds(0.4f);
            Stun();
            VibrationHandler.Instance.MediumVibrate();
        }
        else
        {
            VibrationHandler.Instance.LightVibrate();
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossHurtShort);
            yield return new WaitForSeconds(_swayRandomizePeriod);
            _hitsLeftToStun--;
            _bossState = BossState.Idle;
        }
    }

    private void Stun()
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossStun);
        _animator.SetTrigger("Stunned");
        _bossState = BossState.Stunned;
    }

    private void TriggerPhaseChange()
    {
        _currentPhase++;
        
        
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossDived);
        VibrationHandler.Instance.HeavyVibrate();
        _animator.SetTrigger("TakeDamage");
        StartCoroutine(_manager.IVibrateScreen());
        if (_currentPhase == _totalPhases) StartCoroutine(IDie());
        else StartCoroutine(IPhaseChange());
    }

    private IEnumerator IPhaseChange()
    {
        
        _hitsLeftToStun = _totalHitsToStun;
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossHurtLong);
        MusicPlayer.Instance.EnableLowPass();
        _bossState = BossState.PhaseChange;
        yield return new WaitForSeconds(0.2f);
        _cigarAnimator.SetTrigger("PhaseChange");
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossExplosion);
        Instantiate(Explosion1, LeftHand.transform.position + 0.7f * Vector3.right, Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(1.8f);
        SFXPlayer.Instance.PlayClipFromArrayAtIndex(SFXPlayer.Instance._playerTaunt, _currentPhase - 1);
        yield return new WaitForSeconds(2f);

        if (_currentPhase == 1)
        {
            MusicPlayer.Instance._isThirdTrackEnabled = true;
            _intGenerator.IncreaseChoiceNumber(new float[2] {0f, 1f});
        }
            
        else if (_currentPhase == 2)
        {
            _intGenerator.IncreaseChoiceNumber(new float[3] {0.5f, 0.5f, 0f });
            MusicPlayer.Instance._isFourthTrackEnabled = true;
            MusicPlayer.Instance.DisableLowPass();
            StartCoroutine(IChargeAttack());
            yield break;
        }  
        TeleportAndReset();
        yield return new WaitForSeconds(0.1f);
        MusicPlayer.Instance.DisableLowPass();
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossNewPhase);
    }

    private IEnumerator IDie()
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossHurtLong);
        MusicPlayer.Instance.Pause();
        _manager.OnBossDying();
        _bossState = BossState.Dying;
        Vector2 randPost;
        
        yield return new WaitForSeconds(0.4f);
        _animator.SetTrigger("Dying");
        yield return new WaitForSeconds(0.7f);
        for (int i = 0; i < 12; i++)
        {
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossExplosion);
            randPost = Random.insideUnitCircle * 4f;
            Instantiate(Explosion1, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);
            yield return new WaitForSeconds(0.2f);
            randPost = Random.insideUnitCircle * 4f;
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossExplosion);
            Instantiate(Explosion2, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);
            yield return new WaitForSeconds(0.2f);
        }

        randPost = Random.insideUnitCircle * 4f;
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossBigExplosion, 0.05f);
        Instantiate(Explosion2, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);

        yield return new WaitForSeconds(1.2f);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossBigExplosion, dontDestroyOnLoad : true);
        yield return new WaitForSeconds(0.2f);
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossDead, dontDestroyOnLoad: true);
        _animator.SetTrigger("Dead");
        _cigarAnimator.SetTrigger("PhaseChange");
        _manager.OnBossDead();
        _bossState = BossState.Dead;
        StartCoroutine(_manager.IVibrateScreen());
        Instantiate(Explosion1, transform.position, Quaternion.identity, _enemies.transform).transform.localScale *= 6f;
        while (true)
        {
            randPost = Random.insideUnitCircle * 4f;
            Instantiate(Explosion2, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);
            yield return new WaitForSeconds(0.6f);
        }
    }

    private void TeleportAndReset()
    {
        Teleport((_playerTransform.position.x - 45f) * Vector3.right + 10f * Vector3.up);
        _bossState = BossState.AdjustingPosition;
        _animator.SetTrigger("BackToIdle");
    }

    private void Teleport(Vector3 newPos)
    {
        transform.position = newPos;
        LeftHand.transform.position = newPos;
        RightHand.transform.position = newPos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" && _bossState == BossState.Stunned &&
            collision.collider.GetComponent<PlayerMovement>()._isDiving)
        {
            collision.collider.GetComponent<Rigidbody2D>().linearVelocityY = 18f;
            TriggerPhaseChange();
        }       
    }

    private void SpawnRockWave()
    {
        Vector3 relativePlayerPos = _playerTransform.position - transform.position;
        float angle1 = Mathf.Atan(relativePlayerPos.y / relativePlayerPos.x);
        float angle2 = angle1 + 15f * Mathf.Deg2Rad;
        float angle3 = angle1 - 15f * Mathf.Deg2Rad;


        //ThrowAtAngle(angle1, _rockMinVelocity * Random.Range(1f, 1.5f));
        ThrowAtAngle(angle2, _rockMinVelocity * Random.Range(1f, 1.5f));
        ThrowAtAngle(angle3, _rockMinVelocity * Random.Range(1f, 1.5f));

    }

    private void ThrowAtAngle(float angle, float speed)
    {
        Instantiate(Rock, RightHand.transform.position, Quaternion.identity, _enemies.transform)
            .GetComponent<Rigidbody2D>().linearVelocity = speed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void OnPlayerDeath()
    {
        Interrupt();
        _bossState = BossState.Celebrating;
        gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable");
        StartCoroutine(ICelebrate());
    }

    private IEnumerator ICelebrate()
    {
        _animator.ResetTrigger("BackToIdle");
        _animator.SetTrigger("BackToIdle");
        _normalMovement = false;
        yield return new WaitForSeconds(0.5f);
        
        _animator.SetTrigger("Laugh");
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossVictoryLaugh);
        yield return new WaitForSeconds(0.7f);
        _rightHandCenter = new Vector3(-3f, 4f, 0f);
        _leftHandCenter = new Vector3(3f, 4f, 0f);
        yield return new WaitForSeconds(0.2f);
        LeftHand.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        RightHand.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        while (true)
        {
            _rightHandCenter += 2f * Vector3.up;
            _leftHandCenter += 2f * Vector3.up;
            yield return new WaitForSeconds(0.2f);
            _rightHandCenter -= 2f * Vector3.up;
            _leftHandCenter -= 2f * Vector3.up;
            yield return new WaitForSeconds(0.2f);
        }
    }
}


public enum BossState
{
    Idle,
    //This states can only be accessed from idle
    AdjustingPosition,
    CigarAttack,
    PunchAttack,
    ThrowAttack,
    ChargeAttackPrepare,
    ChargeAttackExecute,

    StandBy,
    Stunned,
    Damaged,
    PhaseChange,
    Dead,
    Dying,
    Celebrating,
}

public class SemiRandomIntGenerator
{
    private float[] _initialProbabilities;
    private float[] _currentProbabilities;
    private int _choiceNumber;

    public SemiRandomIntGenerator(float[] probabilities, int choiceNumber = 0)
    {
        _choiceNumber = choiceNumber;
        _initialProbabilities = new float[probabilities.Length + 1];
        _initialProbabilities[0] = 1f;
        for (int i = 0; i < probabilities.Length; i++)
        {
            _initialProbabilities[0] -= probabilities[i];
            _initialProbabilities[i + 1] = probabilities[i];
        }

        _currentProbabilities = NormalizeProbabilities(_initialProbabilities, choiceNumber);

        if (_choiceNumber > probabilities.Length + 1) Debug.LogError("Not enough probabilities given");
        foreach (float probability in probabilities)
        {
            if (probability > 1f || probability < 0f) Debug.LogError("Invalid Probabilities");
        }

    }

    public void IncreaseChoiceNumber(float[] currentProbabilities = null)
    {
        _choiceNumber++;
        _currentProbabilities = currentProbabilities == null ? _initialProbabilities  : currentProbabilities;
        if (_choiceNumber > _initialProbabilities.Length) Debug.LogError("Not enough probabilities given");
    }

    public int GetInt()
    {
        if (_choiceNumber == 1) return 0;

        int result = 0;
        float randFloat = Random.Range(0f, 1f);
        float aggregatedProb = 0f;

        for (int i = 0; i < _choiceNumber; i++)
        {
            aggregatedProb += _currentProbabilities[i];
            if (randFloat < aggregatedProb)
            {
                result = i;
                break;
            }      
        }

        Debug.Log("Result is " + result);
        for (int i = 0; i < _choiceNumber; i++)
        {
            Debug.Log("The attack number " + (i).ToString() + " had probability " + _currentProbabilities[i]);
        }

        _currentProbabilities = AdjustProbabilities(result, _currentProbabilities, _initialProbabilities);

        return result;
    }

    private float[] AdjustProbabilities(int result, float[] currentProbabilities, float[] initialProbabilities)
    {
        currentProbabilities[result] -= (1f - initialProbabilities[result])/2f;
        for (int i = 0; i < _choiceNumber; i++)
        {
            if (i == result) continue;
            currentProbabilities[i] += initialProbabilities[i] / 2f;
        }
        return NormalizeProbabilities(currentProbabilities, currentProbabilities.Length);
    }

    private float[] NormalizeProbabilities(float[] probabilities, int choiceNumber)
    {
        float[] result = new float[choiceNumber];
        float sum = 0f;
        for (int i = 0; i < choiceNumber; i++)
        {
            result[i] = Mathf.Clamp(probabilities[i], 0f, 1f);
            sum += result[i];
        }
        for (int i = 0; i < _choiceNumber; i++)
        {
            result[i] /= sum;
        }

        return result;
    }

    public string PrintProbabilities()
    {
        string result = "";
        foreach (float probability in _currentProbabilities)
        {
            result += System.Math.Round(probability, 2).ToString() + "   ";
        }
        return result;
    }


}