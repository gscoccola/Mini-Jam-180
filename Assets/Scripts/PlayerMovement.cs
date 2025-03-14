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
    [FoldoutGroup("Parameters")][SerializeField] float _moveSpeed;
    [FoldoutGroup("Parameters")][SerializeField] float _groundAcceleration;
    [FoldoutGroup("Parameters")][SerializeField] float _airAcceleration;

    [Header("Vertical")]
    [FoldoutGroup("Parameters")][SerializeField] float _fallSpeed;
    [FoldoutGroup("Parameters")][SerializeField] float _gravityAcceleration;

    #endregion

    #region Global Variables
    Animator _animator;
    BoxCollider2D _feetCollider;
    bool _isOnFloor;
    bool _isRunning;
    Vector2 _moveInput;
    Rigidbody2D _rigidBody;
    #endregion

    private void Awake()
    {
        _feetCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
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
            _animator.SetBool("_isRunning", playerHasHorizontalSpeed);
            if (playerHasHorizontalSpeed)
            {
                _isRunning = true;
            }
            else { _isRunning = false; }
        }
    }

    private void MoveVertically()
    {
        _animator.SetBool("_isFalling", _rigidBody.linearVelocityY < -1f);
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
}
