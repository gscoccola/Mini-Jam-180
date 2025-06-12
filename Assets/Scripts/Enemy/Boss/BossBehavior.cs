using System.Collections;
using TMPro;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] int _totalHitsToStun;
    [SerializeField] int _totalPhases;

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
    [SerializeField] TextMeshProUGUI BossStateText;

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
    public float _stunnedXSpeed;
    bool _normalMovement = true;
    bool _returnHand = false;

    BossState _bossState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
        _manager = FindAnyObjectByType<BossFightManager>();
        _rightHandAnimator = RightHand.GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(IRandomizeHandTarget());
        _bossState = BossState.StandBy;
        _hitsLeftToStun = _totalHitsToStun;
        _intGenerator = new SemiRandomIntGenerator(new float[1]{ 0.5f ,}, 1);
    }


    private void Update()
    {
        BossStateText.text = _bossState.ToString();

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


    private void Move(BossState bossState, Vector3 relativePlayerPos)
    {
        if (_bossState == BossState.StandBy) return;

        

        if (_bossState == BossState.AdjustingPosition)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (relativePlayerPos + TargetXOffset * Vector3.left).normalized
            * MoveSpeedFast, 3f * Acceleration * Time.deltaTime);
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
            MoveSpeedFast * Vector3.down, Acceleration * 2f * Time.deltaTime);
        }
        else if (_bossState == BossState.Stunned)
        {
            _rigidbody.linearVelocityY = Vector2.MoveTowards(_rigidbody.linearVelocity, (transform.position.y - Mathf.Clamp(
                transform.position.y, 8f , 12f)) * Vector3.down
            * MoveSpeedSlow/5f, Acceleration  * Time.deltaTime / 2f).y;
            _rigidbody.linearVelocityX = -_stunnedXSpeed/2f;
        }
        else if(_bossState == BossState.CigarAttack || _bossState == BossState.ThrowAttack)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.zero, 6f * Acceleration * Time.deltaTime);
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
        
        //_intGenerator.IncreaseChoiceNumber();
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
        _bossState = BossState.PhaseChange;
        yield return new WaitForSeconds(4f);
        if (_currentPhase == 1) MusicPlayer.Instance._isThirdTrackEnabled = true;
        else if (_currentPhase == 2) MusicPlayer.Instance._isFourthTrackEnabled = true;
        TeleportAndReset();
        yield return new WaitForSeconds(0.5f);
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
            SFXPlayer.Instance.PlayClipFromArrayAtIndex(SFXPlayer.Instance._bossExplosion, Random.Range(0, 2));
            randPost = Random.insideUnitCircle * 4f;
            Instantiate(Explosion1, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);
            yield return new WaitForSeconds(0.2f);
            SFXPlayer.Instance.PlayClipFromArrayAtIndex(SFXPlayer.Instance._bossExplosion, Random.Range(0, 2));
            Instantiate(Explosion2, transform.position + new Vector3(randPost.x, randPost.y, 0f), Quaternion.identity, _enemies.transform);
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(1f);
        SFXPlayer.Instance.PlayClipFromArrayAtIndex(SFXPlayer.Instance._bossExplosion, 2);
        _animator.SetTrigger("Dead");
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
        Vector3 newPos = _playerTransform.position + 40f * Vector3.left + 7f * Vector3.up;
        transform.position = newPos;
        LeftHand.transform.position = newPos;
        RightHand.transform.position = newPos;
        _bossState = BossState.Idle;
        _animator.SetTrigger("BackToIdle");
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
        float angle1 = 25f * Mathf.Deg2Rad;
        float angle3 = 45f * Mathf.Deg2Rad;


        ThrowAtAngle(angle1, _rockMinVelocity * Random.Range(1f, 1.5f));
        ThrowAtAngle(angle3, _rockMinVelocity * Random.Range(1f, 1.5f));

    }

    private void ThrowAtAngle(float angle, float speed)
    {
        Instantiate(Rock, RightHand.transform.position, Quaternion.identity, _enemies.transform)
            .GetComponent<Rigidbody2D>().linearVelocity = speed * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
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

    StandBy,
    Stunned,
    Damaged,
    PhaseChange,
    Dead,
    Dying,
}

public class SemiRandomIntGenerator
{
    private float[] _initialProbabilities;
    private float[] _currentProbabilities;
    private int _choiceNumber;

    public SemiRandomIntGenerator(float[] probabilities, int choiceNumber = 0)
    {
        _initialProbabilities = probabilities;
        _currentProbabilities = probabilities;
        _choiceNumber = choiceNumber;

        if (_choiceNumber > probabilities.Length) Debug.LogError("Not enough probabilities given");
        foreach (float probability in probabilities)
        {
            if (probability > 1f || probability < 0f) Debug.LogError("Invalid Probabilities");
        }
    }

    public void IncreaseChoiceNumber()
    {
        _choiceNumber++;
        _currentProbabilities = _initialProbabilities;
        if (_choiceNumber > _initialProbabilities.Length) Debug.LogError("Not enough probabilities given");
    }

    public int GetInt()
    {
        if (_choiceNumber == 0) return 0;

        int result = 0;
        float randFloat = Random.Range(0f, 1f);
        float aggregatedProb = 0f;

        for (int i = 0; i < _choiceNumber; i++)
        {
            aggregatedProb += _currentProbabilities[i];
            if (randFloat < aggregatedProb) result = i + 1;                
        }
        AdjustProbabilities(result);

        Debug.Log("Result is " + result);
        for (int i = 0; i < _choiceNumber; i++)
        {
            Debug.Log((i+1).ToString() + " " + _currentProbabilities[i]);
        }
        
        return result;
    }

    private void AdjustProbabilities(int result)
    {
        /*for (int i = 0; i < _choiceNumber; i++)
        {
            if (i == result) _currentProbabilities[i] -=
        }*/
    }
}