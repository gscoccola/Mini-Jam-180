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

    [SerializeField] GameObject Projectile;
    [SerializeField] Rigidbody2D RightHand;
    [SerializeField] Rigidbody2D LeftHand;
    [SerializeField] TextMeshProUGUI BossStateText;

    Animator _animator;
    Transform _playerTransform;
    Rigidbody2D _rigidbody;
    GameObject _enemies;
    BossFightManager  _manager;
    SemiRandomIntGenerator _intGenerator;
    bool _cigarAttackOnCooldown;
    bool _punchAttackOnCooldown;
    int _hitsLeftToStun;
    int _currentPhase = 0;
    public float _stunnedXSpeed;

    BossState _bossState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
        _manager = FindAnyObjectByType<BossFightManager>();
    }

    private void Start()
    {
        StartCoroutine(IRandomizeHandTarget());
        _bossState = BossState.StandBy;
        _hitsLeftToStun = _totalHitsToStun;
        _intGenerator = new SemiRandomIntGenerator(new float[1]{ 0.5f ,});
    }


    private void Update()
    {
        BossStateText.text = _bossState.ToString();

        MoveHandToTarget(ref RightHand, _rightHandTarget);
        MoveHandToTarget(ref LeftHand, _leftHandTarget);

        if (_bossState == BossState.Dead) return;

        Vector3 relativePlayerPos = _playerTransform.position - transform.position;

        if (_bossState == BossState.Idle) Attack(relativePlayerPos);
        if (_bossState == BossState.PunchAttack) _rightHandTarget = ReturnTargetToCenter(_rightHandTarget, _rightHandCenter);
        if (_bossState == BossState.Stunned && transform.position.x > _manager._cameraEdgeX + 5f) TeleportAndReset();

        AdjustPosition(_bossState, relativePlayerPos);
        Move(_bossState, relativePlayerPos);
    }

    public IEnumerator ICigaretteAttack(bool pauseAfterShot = false)
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossCigarAttack);
        _bossState = BossState.CigarAttack;
        _animator.SetTrigger("Shoot");
        _cigarAttackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        Instantiate(Projectile, transform.position + new Vector3(-hoverXOffset, -hoverYOffset, 0f), Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(AttackMovementPause - 0.2f);
        _bossState = pauseAfterShot ? BossState.StandBy : BossState.Idle;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _cigarAttackOnCooldown = false;
    }

    private IEnumerator IPunchAttack(Vector3 relativePlayerPos)
    {
        _animator.SetTrigger("Frown");
        _bossState = BossState.PunchAttack;
        _rightHandTarget = relativePlayerPos.normalized * MaxDistanceToPunchAttack;
        _punchAttackOnCooldown = true;
        yield return new WaitForSeconds(_fistReturnTime);
        _bossState = BossState.Idle;
        yield return new WaitForSeconds(AttackDelay - _fistReturnTime);
        _punchAttackOnCooldown = false;
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

        if (!_punchAttackOnCooldown && relativePlayerPos.magnitude <= MaxDistanceToPunchAttack)
            StartCoroutine(IPunchAttack(relativePlayerPos));

        else if (!_cigarAttackOnCooldown && Mathf.Abs(relativePlayerPos.y) <= MaxYOffsetToAttack &&
            relativePlayerPos.x > 0 && relativePlayerPos.x <= MaxXOffsetToCigaretteAttack)
        {
            StartCoroutine(ICigaretteAttack());
            _intGenerator.GetInt();
        }
            
    }

    private void Move(BossState bossState, Vector3 relativePlayerPos)
    {
        if (_bossState == BossState.StandBy) return;

        if (_bossState == BossState.CigarAttack )
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.zero, 6f * Acceleration * Time.deltaTime);
            return;
        }

        if (_bossState == BossState.AdjustingPosition)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (relativePlayerPos + TargetXOffset * Vector3.left).normalized
            * MoveSpeedFast, 3f * Acceleration * Time.deltaTime);
        }
        else if (_bossState == BossState.Idle || _bossState == BossState.PunchAttack)
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
            _rigidbody.linearVelocityY = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (transform.position.y - 6f ) * Vector3.down
            * MoveSpeedSlow/5f, Acceleration  * Time.deltaTime / 2f).y;
            _rigidbody.linearVelocityX = -_stunnedXSpeed/2f;
        }

    }

    private void MoveHandToTarget(ref Rigidbody2D hand, Vector3 center)
    {
        Vector3 direction = center + transform.position - hand.transform.position;
        hand.AddForce(direction * _elasticConstant * hand.mass);
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
        _cigarAttackOnCooldown = false;
        _punchAttackOnCooldown = false;
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
        StartCoroutine(_manager.IVibrateScreen());
        VibrationHandler.Instance.HeavyVibrate();
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossHurtLong);
        _animator.SetTrigger("TakeDamage");
        _hitsLeftToStun = _totalHitsToStun;
        StartCoroutine(IPhaseChange());
    }

    private IEnumerator IPhaseChange()
    {
        _bossState = BossState.PhaseChange;
        yield return new WaitForSeconds(4f);
        TeleportAndReset();   
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
            collision.collider.GetComponent<Rigidbody2D>().linearVelocityY += 8f;
            TriggerPhaseChange();
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

    StandBy,
    Stunned,
    Damaged,
    PhaseChange,
    Dead,
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