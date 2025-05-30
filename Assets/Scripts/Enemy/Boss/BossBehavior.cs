using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float Acceleration;
    [SerializeField] float TargetXOffset;

    [SerializeField] float AttackDelay;
    [SerializeField] float AttackMovementPause;
    [SerializeField] float MaxYOffsetToAttack;


    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] GameObject Projectile;

    Animator _animator;
    Transform _playerTransform;
    Rigidbody2D _rigidbody;
    GameObject _enemies;
    bool _attackOnCooldown;

    public bool _isActive;
    bool _isShooting;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public IEnumerator IAttack()
    {
        _animator.SetTrigger("Shoot");
        _isShooting = true;
        _attackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        Instantiate(Projectile, transform.position + new Vector3(-xOffset, -yOffset, 0f), Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(AttackMovementPause - 0.2f);
        _isShooting = false;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _attackOnCooldown = false;
    }

    private void Update()
    {
        if (!_isActive) return;
        if (!_attackOnCooldown &&
            Mathf.Abs(transform.position.y - yOffset - _playerTransform.position.y) <= MaxYOffsetToAttack)
            StartCoroutine(IAttack());
        Move();
    }

    private void Move()
    {
        if (_isShooting)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.zero, 6f * Acceleration * Time.deltaTime);
            return;
        }
        _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (_playerTransform.position + TargetXOffset * Vector3.left - transform.position)
            * MoveSpeed, Acceleration * Time.deltaTime);
    }

}
