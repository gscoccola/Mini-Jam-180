using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using System;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    
    #region Parameters

    [Header("Horizontal")]

    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultGroundSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultMoveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _upgradedMoveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _minimumFlyingSpeed;

    [FoldoutGroup("Player Parameters")][SerializeField] float _groundAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultAirAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _upgradedAirAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _flyingDeceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _flyingDecelerationDifference;

    

    [Header("Vertical")]
    [FoldoutGroup("Player Parameters")][SerializeField] float _jumpSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _fallSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _gravityAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _diveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _diveAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _diveCooldown;
    [FoldoutGroup("Player Parameters")][SerializeField] float _maxHeight = 200f;
    [FoldoutGroup("Player Parameters")][SerializeField] float _minHeight = - 200f;


    [Header("PowerUP")]
    [FoldoutGroup("Player Parameters")][SerializeField] float _invulnerabilityDuration;


    [Header("Horizontal")]
    [FoldoutGroup("Boost Parameters")][SerializeField] float _birdDeviation;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _droneDeviation;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _balloonDeviation;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _goldenBalloonDeviation;

    [FoldoutGroup("Boost Parameters")][SerializeField] float _footstepsDelay;

    [Header("Vertical")]
    [FoldoutGroup("Boost Parameters")][SerializeField] float _birdBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _droneBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _balloonBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _goldenBalloonBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _stunDuration;

    [Header("Is this the epilogue?")]
    [FoldoutGroup("Epilogue")][SerializeField] bool _isEpilogue;

    #endregion

    #region Global Variables
    Animator _animator;
    BoxCollider2D _feetCollider;
    public bool IsOnFloor {  get; private set; }
    float _horizontalInput;
    Rigidbody2D _rigidBody;
    int _fallingStance;
    GameManager _gameManager;
    public bool IsDead { get; private set; }
    bool _isHurtSoundOnCooldown;
    public bool IsHorizonalMovementDisabled = false;
    public bool _isInputDisabled = false;
    float _currentMoveSpeed;
    float _currentAirAcceleration;
    float worldMin;
    float worldMax;
    ParticleSystem _particleSystem;
    Dictionary<EnemyType, float[]> _enemyBoosts = new Dictionary<EnemyType, float[]>();
    public int PlayerCombo { get; private set; } = 0;
    bool _isOutOfBounds;
    bool _isRunning;
    bool _isPlayingFootsteps;
    bool _isLeftFootstepNext;
    float _lastX;
    PolygonCollider2D confiner;
    bool _isFlying;
    float _currentGravity;
    bool _isStunned;
    GameObject _stunVFX;
    bool _verticalInput;
    bool _diveAvailable = true;
    bool _touchScreenRight;
    bool _touchScreenLeft;
    public bool _isDiving;
    public bool _allowHorizontalSpeed;
    #endregion

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _feetCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _particleSystem = GetComponent<ParticleSystem>();
        confiner = FindAnyObjectByType<PolygonCollider2D>();
    }

    private void Start()
    {
        _currentAirAcceleration = _defaultAirAcceleration;
        _currentMoveSpeed = _defaultMoveSpeed;
        _currentGravity = _gravityAcceleration;
        _stunVFX = transform.GetChild(0).gameObject;

        _enemyBoosts = new Dictionary<EnemyType, float[]>
        {
            [EnemyType.Bird] = new float[2]{ _birdDeviation, _birdBoost },
            [EnemyType.Drone] = new float[2]{ _droneDeviation, _droneBoost },
            [EnemyType.DronePink] = new float[2]{ _droneDeviation, _droneBoost },
            [EnemyType.Balloon] = new float[2]{ _balloonDeviation, _balloonBoost },
            [EnemyType.GoldenBalloon] = new float[2]{ _goldenBalloonDeviation, _goldenBalloonBoost },
            [EnemyType.Cigarette] = new float[2]{ _birdDeviation, _birdBoost },
        };

        CalculateBounds();
    }

    void FixedUpdate()
    {
        CalculateIsOnFloor();
        HandleOutOfBounds();
        if (IsDead)
        {
            if (!_isOutOfBounds && !_allowHorizontalSpeed) _rigidBody.linearVelocityX = 0f;
            return;
        }
        MoveVertically();
        MoveHorizontally(); 
    }

    public void CalculateBounds()
    {
        worldMin = confiner.bounds.min.x;
        worldMax = confiner.bounds.max.x;
    }

    private void HandleOutOfBounds()
    {
        if (_isEpilogue || _isInputDisabled) return;
        if ( !IsOnFloor && transform.position.x < worldMin - 1f )
        {
            _rigidBody.linearVelocityY = 0f;
            _rigidBody.linearVelocityX = -3f;
            if (!_isOutOfBounds) GoOutOfBounds();

        }
        else if (!IsOnFloor && transform.position.x > worldMax +1f )
        {
            _rigidBody.linearVelocityY = 0f;
            _rigidBody.linearVelocityX = 3f;
            if (!_isOutOfBounds) GoOutOfBounds();

        }
        else if (transform.position.y < _minHeight)
        {
            _rigidBody.linearVelocityX = 0f;
            _rigidBody.linearVelocityY = -3f;
            if (!_isOutOfBounds) GoOutOfBounds();

        }
        else if (transform.position.y > _maxHeight + 3f)
        {
            _rigidBody.linearVelocityX = 0f;
            _rigidBody.linearVelocityY = 3f;
            if (!_isOutOfBounds) GoOutOfBounds();

        }
    }

    public void GoOutOfBounds()
    {
        _isOutOfBounds = true;
        _gameManager.OutOfBounds();
        _particleSystem.Stop();
        IsDead = true;
    }


    private void MoveHorizontally()
    {
        if (IsHorizonalMovementDisabled )
        {
            _rigidBody.linearVelocityX = 0f;
            return;
        }

        Vector2 inputDirection = _isInputDisabled || _isStunned ? Vector2.zero : new Vector2(_horizontalInput, 0f);

        if (IsOnFloor)
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, inputDirection * _defaultGroundSpeed, _groundAcceleration * Time.deltaTime).x;
            bool playerHasHorizontalSpeed = _horizontalInput != 0f;
            if (playerHasHorizontalSpeed)
            {
                transform.localScale = new Vector3(MathF.Sign(_horizontalInput), 1f, 1f);
            }
            playerHasHorizontalSpeed = (playerHasHorizontalSpeed && Mathf.Abs(transform.position.x - _lastX) > Mathf.Epsilon);
            _isRunning = playerHasHorizontalSpeed;
            _animator.SetBool("_isRunning", playerHasHorizontalSpeed);
            _lastX = transform.position.x;
            HandleFootsteps();
            return;
        }
        transform.localScale = new Vector3(1f, 1f, 1f);

        if (!_isFlying)
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, inputDirection * _currentMoveSpeed, _currentAirAcceleration * Time.deltaTime).x;
            return;
        }

        /*_rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, inputDirection * _flyingMoveSpeed, _flyingAirAcceleration * Time.deltaTime).x;
        if (_rigidBody.linearVelocityX > -_minimumFlyingSpeed) SetFly(false);*/

        _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector3.zero,
            (_flyingDeceleration + inputDirection.x * _flyingDecelerationDifference) * Time.deltaTime).x;

        if (_rigidBody.linearVelocityX > -_minimumFlyingSpeed) SetFly(false);
        /*
        if (_rigidBody.linearVelocityX > -_minimumFlyingSpeed)
        {
            if (_belowFlyingSpeed) SetFly(false);
            else _belowFlyingSpeed = true;
        }*/
    }

    private void HandleFootsteps()
    {
        if (_isRunning && !_isPlayingFootsteps)
        {
            StartCoroutine(PlayFootsteps());
        }
    }

    private IEnumerator PlayFootsteps()
    {
        _isPlayingFootsteps = true;
        if (_isLeftFootstepNext) SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerLeftFootsteps);
        else SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerRightFootsteps);
        _isLeftFootstepNext = !_isLeftFootstepNext;
        yield return new WaitForSeconds(_footstepsDelay);
        _isPlayingFootsteps = false;
    }

    private void MoveVertically()
    {
        if (Application.isMobilePlatform) CombinationInput();
        _animator.SetBool("_isFalling", !IsOnFloor);
        if (IsOnFloor) return;
        _isDiving = (_verticalInput && !_isInputDisabled &&  !_isStunned &&  !_isFlying && _diveAvailable);
        _rigidBody.linearVelocityY = _isDiving
            ? _rigidBody.linearVelocityY = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector2.down * _diveSpeed, _diveAcceleration * Time.deltaTime).y
            : _rigidBody.linearVelocityY = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector2.down * _fallSpeed, _currentGravity * Time.deltaTime).y;
        _animator.SetBool("_isDiving", _isDiving && _rigidBody.linearVelocityY < -5f );
    }

    void CalculateIsOnFloor()
    {
        IsOnFloor = _feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        gameObject.layer = IsOnFloor ? LayerMask.NameToLayer("GroundedPlayer") : LayerMask.NameToLayer("Player");
        if (!IsOnFloor) _isRunning = false;
        if (IsOnFloor && PlayerCombo!=0)
        {
            ResetCombo();
        }
    }

    void OnMove(InputValue value)
    {
        _horizontalInput = Mathf.Abs(value.Get<Vector2>().x) > 0.8f ? Mathf.Sign(value.Get<Vector2>().x) : 0f;
        _verticalInput = Mathf.Abs(value.Get<Vector2>().y) > 0.2f;
    }

    public void TouchMove(TouchScreenButtonType direction, bool state)
    {
        if (direction == TouchScreenButtonType.Left)
        {
            _touchScreenLeft = state;
        }
        else
        {
            _touchScreenRight = state;
        }
    }

    public void CombinationInput()
    {
        _verticalInput = _touchScreenLeft && _touchScreenRight;
        if (_touchScreenLeft && !_touchScreenRight) _horizontalInput = -1f;
        else if (!_touchScreenLeft && _touchScreenRight) _horizontalInput = 1f;
        else _horizontalInput = 0f;
    }

    public bool Jump()
    {
        if (!IsOnFloor) return false;
        _rigidBody.linearVelocityY += _jumpSpeed;
        _fallingStance = 0;
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerJump);
        VibrationHandler.Instance.LightVibrate();
        if (_isEpilogue) _isInputDisabled = true;
        return true;
    }

    void ChangeStance()
    {
        _fallingStance = (_fallingStance + 1) % 3;
        _animator.SetInteger("_fallingStance", _fallingStance);
    }

    public void Boost(EnemyType type)
    {
        _rigidBody.linearVelocityY = _enemyBoosts[type][1];
        if (_isInputDisabled && type == EnemyType.Cigarette) _isInputDisabled = false;
        if (_isFlying) SetFly(false);
        IncreaseCombo();
        ChangeStance();
        StartCoroutine(IDiveInterruption());
        VibrationHandler.Instance.LightVibrate();
    }

    public void Deviate(EnemyType type, bool appliesBackwardsMomentunm = true)
    {
        

        if (type == EnemyType.GoldenBalloon)
        {
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerBounce);
            _rigidBody.linearVelocityY = _enemyBoosts[type][1];
            _rigidBody.linearVelocityX = -_goldenBalloonDeviation;
            PlayerCombo = 3;
            IncreaseCombo();
            SetFly(true);
            VibrationHandler.Instance.HeavyVibrate();
            return;
        }
        if (type == EnemyType.DronePink) StartCoroutine(IStun());

        if (appliesBackwardsMomentunm) _rigidBody.linearVelocityX += _enemyBoosts[type][0];
        else _rigidBody.linearVelocityX = 0f;
        if (!_isHurtSoundOnCooldown)
        {
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerHurt);
            _isHurtSoundOnCooldown = true;
            StartCoroutine(HurtSoundCooldown());
        }   
        ResetCombo();
        ChangeStance();
        StartCoroutine(IDiveInterruption());
        if (_isFlying) SetFly(false);
        VibrationHandler.Instance.MediumVibrate();
    }

    public void MissileHit(float downwardVelocity)
    {
        _rigidBody.linearVelocityX = 0f;
        _rigidBody.linearVelocityY -= downwardVelocity;
        StartCoroutine(IStun());
        StartCoroutine(IDiveInterruption());

        if (!_isHurtSoundOnCooldown)
        {
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerHurt);
            _isHurtSoundOnCooldown = true;
            StartCoroutine(HurtSoundCooldown());
        }

        if (_isFlying) SetFly(false);
        
    }

    public void Stun()
    {
        StartCoroutine(IStun());
    }

    IEnumerator IStun()
    {
        ResetCombo();
        ChangeStance();
        VibrationHandler.Instance.MediumVibrate();
        _isStunned = true;
        _stunVFX.SetActive(true);
        yield return new WaitForSeconds(_stunDuration);
        _isStunned = false;
        _stunVFX.SetActive(false);
        StartCoroutine(IDiveInterruption());

    }

    IEnumerator HurtSoundCooldown()
    {
        yield return new WaitForSeconds(1f);
        _isHurtSoundOnCooldown = false;
    }

    private void OnReloadLevel(InputValue value)
    {
        _gameManager.ReloadLevel();
    }

    private void OnPause(InputValue value)
    {
        _gameManager.PauseKeyPress();
    }

    private void OnSkipLevel(InputValue value)
    {
        _gameManager.NextLevel();
    }

    private void OnExitLevel(InputValue value)
    {
        _gameManager.ExitLevel();
    }

    private void OnTogglePostProcessing(InputValue value)
    {
        PostProcessing.Instance.Toggle();
    }

    public void Die()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable");
        VibrationHandler.Instance.MediumVibrate();
        _particleSystem.Stop();
        IsDead = true;
        _animator.SetTrigger("_die");
        GetComponent<SpriteRenderer>().sortingLayerName = "Water";
    }

 
    public void Win()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable");
        _particleSystem.Stop();
        IsDead = true;
        _fallingStance = 2;
        _animator.SetBool("_isDiving", false);
        _animator.SetBool("_isFalling", !IsOnFloor);
        _animator.SetInteger("_fallingStance", _fallingStance);
    }

    void IncreaseCombo()
    {
        PlayerCombo++;
        PlayerCombo = (int)Mathf.Clamp(PlayerCombo, 0, 3);
        if (PlayerCombo == 3)
        {
            MusicPlayer.Instance._isSecondTrackEnabled = true;
            _particleSystem.Play();
            _currentAirAcceleration = _upgradedAirAcceleration;
            _currentMoveSpeed = _upgradedMoveSpeed;
            _gameManager.OnAchievementUnlocked(4);
        }
    }

    void ResetCombo()
    {
        if (PlayerCombo == 3) SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._comboLost);
        PlayerCombo = 0;
        _currentAirAcceleration = _defaultAirAcceleration;
        _currentMoveSpeed = _defaultMoveSpeed;
        _particleSystem.Stop();
        MusicPlayer.Instance._isSecondTrackEnabled = false;
    }

    private void SetFly(bool boolean)
    {
        _animator.SetBool("_fly", boolean);
        _isFlying = boolean;
        _currentGravity = boolean ? _gravityAcceleration/2 : _gravityAcceleration;
        if (boolean)
        {
            StartCoroutine(IFlyInvulnerability());
            MusicPlayer.Instance.PlayWind();
        }    
        else MusicPlayer.Instance._beginFadeOutWind = true;
    }

    IEnumerator IFlyInvulnerability()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable");
        yield return new WaitForSeconds(_invulnerabilityDuration);
        _animator.SetTrigger("_invulnerabilityEnd");
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    IEnumerator IDiveInterruption()
    {
        _diveAvailable = false;
        yield return new WaitForSeconds(_diveCooldown);
        _diveAvailable = true;
    }

    public void ToggleGravity()
    {
        _currentGravity = _currentGravity == 0f ? _gravityAcceleration : 0f;
    }

}
