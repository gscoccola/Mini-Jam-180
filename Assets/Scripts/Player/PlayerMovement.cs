using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using System;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    #region Parameters

    [Header("Horizontal")]
    [FoldoutGroup("Player Parameters")][SerializeField] float _moveSpeed;
    [FoldoutGroup("Player Parameters")][SerializeField] float _groundAcceleration;
    [FoldoutGroup("Player Parameters")][SerializeField] float _airAcceleration;

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
    bool _isOnFloor;
    Vector2 _moveInput;
    Rigidbody2D _rigidBody;
    int _fallingStance;
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;

    Dictionary<EnemyType, float[]> _enemyBoosts = new Dictionary<EnemyType, float[]>();
    #endregion

    private void Awake()
    {
        _SFXPlayer = GetComponent<SFXPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _feetCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _enemyBoosts = new Dictionary<EnemyType, float[]>
        {
            [EnemyType.Bird] = new float[2]{ _birdDeviation, _birdBoost },
            [EnemyType.Drone] = new float[2]{ _droneDeviation, _droneBoost },
            [EnemyType.Balloon] = new float[2]{ _tenderDeviation, _tenderBoost },
        };
    }


    // Update is called once per frame
    void Update()
    {
        IsOnFloor();
        MoveHorizontally();
        MoveVertically();
    }

    private void MoveHorizontally()
    {
        Vector2 playerVelocity = new Vector2(_moveInput.x, 0f);
        if (!_isOnFloor)
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _moveSpeed, _airAcceleration * Time.deltaTime).x;
        }
        else
        {
            _rigidBody.linearVelocityX = Vector2.MoveTowards(_rigidBody.linearVelocity, playerVelocity * _moveSpeed, _groundAcceleration * Time.deltaTime).x;
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
        //_animator.SetBool("_isFalling", Mathf.Abs(_rigidBody.linearVelocityY) > 0.5f);
        _animator.SetBool("_isFalling", !_isOnFloor);
        if (_isOnFloor) return;
        _rigidBody.linearVelocityY = Vector2.MoveTowards(_rigidBody.linearVelocity, Vector2.down * _fallSpeed, _gravityAcceleration * Time.deltaTime).y;
    }

    void IsOnFloor()
    {
        _isOnFloor = _feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!_isOnFloor) return;
        _rigidBody.linearVelocityY += _jumpSpeed;
        _fallingStance = 0;
        _SFXPlayer.PlayRandomClip(_SFXPlayer._playerJump);
    }

    void ChangeStance()
    {
        _fallingStance = (_fallingStance + 1) % 3;
        _animator.SetInteger("_fallingStance", _fallingStance);
    }

    public void Boost(EnemyType type)
    {
        _rigidBody.linearVelocityY = _enemyBoosts[type][1];
        ChangeStance();
    }

    public void Deviate(EnemyType type)
    {
        _rigidBody.linearVelocityX += _enemyBoosts[type][0];
        ChangeStance();
    }

    private void OnReloadLevel(InputValue value)
    {
        _gameManager.ReloadLevel();
    }
}
