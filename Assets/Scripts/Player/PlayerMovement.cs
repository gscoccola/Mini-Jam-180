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
    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultMoveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultGroundSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _upgradedMoveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _defaultAirAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _upgradedAirAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _groundAcceleration;
    

    [Header("Vertical")]
    [FoldoutGroup("Player Parameters")][SerializeField] float _jumpSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _fallSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _gravityAcceleration;

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

    [Header("Is this the epilogue?")]
    [FoldoutGroup("Epilogue")][SerializeField] bool _isEpilogue;

    #endregion

    #region Global Variables
    Animator _animator;
    BoxCollider2D _feetCollider;
    public bool IsOnFloor {  get; private set; }
    Vector2 _moveInput;
    Rigidbody2D _rigidBody;
    int _fallingStance;
    GameManager _gameManager;
    public bool IsDead { get; private set; }
    bool _isHurtSoundOnCooldown;
    public bool IsHorizonalMovementDisabled = false;
    public bool IsHorizonalInputDisabled = false;
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

        _enemyBoosts = new Dictionary<EnemyType, float[]>
        {
            [EnemyType.Bird] = new float[2]{ _birdDeviation, _birdBoost },
            [EnemyType.Drone] = new float[2]{ _droneDeviation, _droneBoost },
            [EnemyType.Balloon] = new float[2]{ _balloonDeviation, _balloonBoost },
            [EnemyType.GoldenBalloon] = new float[2]{ _goldenBalloonDeviation, _goldenBalloonBoost },
        };

        CalculateBounds();
    }

    void FixedUpdate()
    {
        CalculateIsOnFloor();
        MoveVertically();
        HandleOutOfBounds();
        if (IsDead)
        {
            if (!_isOutOfBounds)_rigidBody.linearVelocityX = 0f;
            return;
        }  
        MoveHorizontally();
        
    }

    public void CalculateBounds()
    {
        worldMin = confiner.bounds.min.x;
        worldMax = confiner.bounds.max.x;
    }

    private void HandleOutOfBounds()
    {
        if ( !IsOnFloor && transform.position.x < worldMin )
        {
            _rigidBody.linearVelocityY = 0f;
            _rigidBody.linearVelocityX = -3f;
            if (!_isOutOfBounds) GoOutOfBounds();

        }
        else if (!IsOnFloor && transform.position.x > worldMax)
        {
            _rigidBody.linearVelocityY = 0f;
            _rigidBody.linearVelocityX = 3f;
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
        if (IsHorizonalMovementDisabled)
        {
            _rigidBody.linearVelocityX = 0f;
            return;
        }

        Vector2 playerVelocity = IsHorizonalInputDisabled ? Vector2.zero : new Vector2(_moveInput.x, 0f);


        if (!IsOnFloor)
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _currentMoveSpeed, _currentAirAcceleration * Time.deltaTime).x;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _defaultGroundSpeed, _groundAcceleration * Time.deltaTime).x;
            bool playerHasHorizontalSpeed = Mathf.Abs(_moveInput.x) > Mathf.Epsilon;
            if (playerHasHorizontalSpeed)
            {
                transform.localScale = new Vector3(MathF.Sign(_moveInput.x), 1f, 1f);
            }
            playerHasHorizontalSpeed = (playerHasHorizontalSpeed && Mathf.Abs(transform.position.x - _lastX) > Mathf.Epsilon);
            _isRunning = playerHasHorizontalSpeed;
            _animator.SetBool("_isRunning", playerHasHorizontalSpeed);
            _lastX = transform.position.x;
        }
        HandleFootsteps();
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
        if (_isLeftFootstepNext) SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerLeftFootsteps);
        else SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerRightFootsteps);
        _isLeftFootstepNext = !_isLeftFootstepNext;
        yield return new WaitForSeconds(_footstepsDelay);
        _isPlayingFootsteps = false;
    }

    private void MoveVertically()
    {
        _animator.SetBool("_isFalling", !IsOnFloor);
        if (IsOnFloor) return;
        _rigidBody.linearVelocityY = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector2.down * _fallSpeed, _currentGravity * Time.deltaTime).y;
    }

    void CalculateIsOnFloor()
    {
        IsOnFloor = _feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!IsOnFloor) _isRunning = false;
        if (IsOnFloor && PlayerCombo!=0)
        {
            ResetCombo();
        }
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public bool Jump()
    {
        if (!IsOnFloor) return false;
        _rigidBody.linearVelocityY += _jumpSpeed;
        _fallingStance = 0;
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerJump);
        VibrationHandler.Instance.LightVibrate();
        if (_isEpilogue) IsHorizonalInputDisabled = true;
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

        if (type == EnemyType.GoldenBalloon)
        {
            PlayerCombo = 3;
            IncreaseCombo();
            SetFly(true);
            VibrationHandler.Instance.HeavyVibrate();
            return;
        }

        if (_isFlying) SetFly(false);
        IncreaseCombo();
        ChangeStance();
        VibrationHandler.Instance.LightVibrate();
    }

    public void Deviate(EnemyType type, bool appliesBackwardsMomentunm = true)
    {
        if (appliesBackwardsMomentunm) _rigidBody.linearVelocityX += _enemyBoosts[type][0];
        else _rigidBody.linearVelocityX = 0f;
        if (!_isHurtSoundOnCooldown)
        {
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerHurt);
            _isHurtSoundOnCooldown = true;
            StartCoroutine(HurtSoundCooldown());
        }   
        ResetCombo();
        ChangeStance();
        if (_isFlying) SetFly(false);
        VibrationHandler.Instance.MediumVibrate();
    }

    public void MissileHit(float downwardVelocity)
    {
        _rigidBody.linearVelocityX = 0f;
        _rigidBody.linearVelocityY -= downwardVelocity;

        if (!_isHurtSoundOnCooldown)
        {
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerHurt);
            _isHurtSoundOnCooldown = true;
            StartCoroutine(HurtSoundCooldown());
        }

        if (_isFlying) SetFly(false);
        ResetCombo();
        ChangeStance();
        VibrationHandler.Instance.MediumVibrate();
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

    public void Die()
    {
        VibrationHandler.Instance.MediumVibrate();
        _particleSystem.Stop();
        IsDead = true;
        _animator.SetTrigger("_die");
        GetComponent<SpriteRenderer>().sortingLayerName = "Water";
    }

 
    public void Win()
    {
        _particleSystem.Stop();
        IsDead = true;
        _fallingStance = 2;
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
        if (PlayerCombo == 3) SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._comboLost);
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
    }
}
