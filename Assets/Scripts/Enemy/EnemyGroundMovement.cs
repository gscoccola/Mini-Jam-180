using Alchemy.Inspector;
using UnityEngine;

public class EnemyGroundMovement : MonoBehaviour
{

    [Header("Horizontal")]
    [FoldoutGroup("Parameters")][SerializeField] float _moveSpeed;


    BoxCollider2D _wallCollider;

    Rigidbody2D _rigidBody;

    CircleCollider2D _collider;
    Animator _animator;

    float _initialDirection;

    private void Awake()
    {
        _wallCollider = GetComponent<BoxCollider2D>();
        _collider = GetComponent<CircleCollider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _initialDirection = transform.localScale.x;
    }


    private void Update()
    {
        // _animator.SetBool("IsRunning", true);
        // _rigidBody.linearVelocity = new Vector3(Vector3.MoveTowards(_rigidBody.linearVelocity, new Vector3(_moveSpeed, 0, 0), 10).x * _initialDirection, 0f, 0f);
        _rigidBody.linearVelocityX = _moveSpeed * _initialDirection;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {

            _moveSpeed = -_moveSpeed;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        
    }

    
}
