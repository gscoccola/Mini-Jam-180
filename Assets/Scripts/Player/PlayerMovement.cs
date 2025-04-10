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
    [FoldoutGroup("Boost Parameters")][SerializeField] float _tenderDeviation;

    [Header("Vertical")]
    [FoldoutGroup("Boost Parameters")][SerializeField] float _birdBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _droneBoost;
    [FoldoutGroup("Boost Parameters")][SerializeField] float _tenderBoost;

    #endregion

    #region Global Variables
    Animator _animator;
    BoxCollider2D _feetCollider;
    public bool IsOnFloor {  get; private set; }
    Vector2 _moveInput;
    Rigidbody2D _rigidBody;
    int _fallingStance;
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;
    MusicPlayer _musicPlayer;
    public bool IsDead { get; private set; }
    bool _isHurtSoundOnCooldown;
    public bool IsHorizonalMovementDisabled = false;
    float _currentMoveSpeed;
    float _currentAirAcceleration;
    float worldMin;
    float worldMax;
    ParticleSystem _particleSystem;
    CinemachineCamera _camera;
    Dictionary<EnemyType, float[]> _enemyBoosts = new Dictionary<EnemyType, float[]>();
    public int PlayerCombo { get; private set; } = 0;
    bool _isOutOfBounds;
    #endregion

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _musicPlayer = FindAnyObjectByType<MusicPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _feetCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _particleSystem = GetComponent<ParticleSystem>();
        _camera = FindAnyObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        _currentAirAcceleration = _defaultAirAcceleration;
        _currentMoveSpeed = _defaultMoveSpeed;

        _enemyBoosts = new Dictionary<EnemyType, float[]>
        {
            [EnemyType.Bird] = new float[2]{ _birdDeviation, _birdBoost },
            [EnemyType.Drone] = new float[2]{ _droneDeviation, _droneBoost },
            [EnemyType.Balloon] = new float[2]{ _tenderDeviation, _tenderBoost },
        };

        CalculateBounds();
    }


    // Update is called once per frame
    void Update()
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

    private void CalculateBounds()
    {
        PolygonCollider2D confiner = FindAnyObjectByType<PolygonCollider2D>();
        worldMin = confiner.bounds.min.x - 0.6f;
        worldMax = confiner.bounds.max.x + 0.6f;
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
        _camera.gameObject.SetActive(false);
    }


    private void MoveHorizontally()
    {
        if (IsHorizonalMovementDisabled)
        {
            _rigidBody.linearVelocityX = 0f;
            return;
        }
        Vector2 playerVelocity = new Vector2(_moveInput.x, 0f);
        if (!IsOnFloor)
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _defaultMoveSpeed, _defaultAirAcceleration * Time.deltaTime).x;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _defaultMoveSpeed, _groundAcceleration * Time.deltaTime).x;
            bool playerHasHorizontalSpeed = Mathf.Abs(_moveInput.x) > Mathf.Epsilon;
            if (playerHasHorizontalSpeed)
            {
                transform.localScale = new Vector3(MathF.Sign(_moveInput.x), 1f, 1f);
            }
            
            _animator.SetBool("_isRunning", playerHasHorizontalSpeed);
        }
    }

    private void MoveVertically()
    {
        _animator.SetBool("_isFalling", !IsOnFloor);
        if (IsOnFloor) return;
        _rigidBody.linearVelocityY = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector2.down * _fallSpeed, _gravityAcceleration * Time.deltaTime).y;
    }

    void CalculateIsOnFloor()
    {
        IsOnFloor = _feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void Jump()
    {
        if (!IsOnFloor) return;
        _rigidBody.linearVelocityY += _jumpSpeed;
        _fallingStance = 0;
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerJump);
    }

    void ChangeStance()
    {
        _fallingStance = (_fallingStance + 1) % 3;
        _animator.SetInteger("_fallingStance", _fallingStance);
    }

    public void Boost(EnemyType type)
    {
        _rigidBody.linearVelocityY = _enemyBoosts[type][1];
        IncreaseCombo();
        ChangeStance();
    }

    public void Deviate(EnemyType type, bool appliesBackwardsMomentunm = true)
    {
        if (appliesBackwardsMomentunm) _rigidBody.linearVelocityX += _enemyBoosts[type][0];
        else _rigidBody.linearVelocityX = 0f;
        if (!_isHurtSoundOnCooldown)
        {
            _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerHurt);
            _isHurtSoundOnCooldown = true;
            StartCoroutine(HurtSoundCooldown());
        }   
        ResetCombo();
        ChangeStance();
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

    private void OnSkipLevel(InputValue value)
    {
        _gameManager.NextLevel();
    }

    public void Die()
    {
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
            _musicPlayer._isSecondTrackEnabled = true;
            _particleSystem.Play();
            _currentAirAcceleration = _upgradedAirAcceleration;
            _currentMoveSpeed = _upgradedMoveSpeed;
        }
    }

    void ResetCombo()
    {
        if (PlayerCombo == 3) _SFXPlayer.PlayClipFromArray(_SFXPlayer._comboLost);
        PlayerCombo = 0;
        _currentAirAcceleration = _defaultAirAcceleration;
        _currentMoveSpeed = _defaultMoveSpeed;
        _particleSystem.Stop();
        _musicPlayer._isSecondTrackEnabled = false;
    }
}
