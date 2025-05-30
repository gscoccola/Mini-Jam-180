using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float Acceleration;
    [SerializeField] float TargetXOffset;

    [SerializeField] float AttackDelay;
    [SerializeField] float AttackMovementPause;
    [SerializeField] float MaxYOffsetToAttack;
    [SerializeField] float MaxXOffsetToCigaretteAttack;
    [SerializeField] float MaxXOffsetToPunchAttack;


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

    Animator _animator;
    Transform _playerTransform;
    Rigidbody2D _rigidbody;
    GameObject _enemies;
    bool _attackOnCooldown;

    BossState _bossState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(IRandomizeHandTarget());
        _bossState = BossState.Idle;
    }

    private IEnumerator ICigaretteAttack()
    {

        _bossState = BossState.CigarAttack;
        _animator.SetTrigger("Shoot");
        _attackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        Instantiate(Projectile, transform.position + new Vector3(-hoverXOffset, -hoverYOffset, 0f), Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(AttackMovementPause - 0.2f);
        _bossState = BossState.Idle;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _attackOnCooldown = false;
    }

    private IEnumerator IPunchAttack()
    {
        Debug.Log("punch");
        _bossState = BossState.PunchAttack;
        //_animator.SetTrigger("PunchFace");
        _attackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(AttackMovementPause + 3f - 0.2f);
        _bossState = BossState.Idle;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _attackOnCooldown = false;
    }

    private void Update()
    {
        HandFollowTarget(ref RightHand, _rightHandTarget);
        HandFollowTarget(ref LeftHand, _leftHandTarget);

        if (_bossState == BossState.Dead) return;

        Attack();

        Move();
    }

    private void Attack()
    {
        Vector2 distanceToPlayer = new Vector2(_playerTransform.position.x - transform.position.x,
                    Mathf.Abs(transform.position.y - hoverYOffset - _playerTransform.position.y));

        if (!_attackOnCooldown && _bossState == BossState.Idle && distanceToPlayer.y <= MaxYOffsetToAttack &&
            Mathf.Abs(distanceToPlayer.x) <= MaxXOffsetToPunchAttack)
            StartCoroutine(IPunchAttack());

        else if (!_attackOnCooldown && _bossState == BossState.Idle && distanceToPlayer.y <= MaxYOffsetToAttack &&
            distanceToPlayer.x > 0 && distanceToPlayer.x <= MaxXOffsetToCigaretteAttack)
            StartCoroutine(ICigaretteAttack());
    }

    private void Move()
    {
        if (_bossState == BossState.CigarAttack || _bossState == BossState.PunchAttack)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            Vector3.zero, 6f * Acceleration * Time.deltaTime);
            return;
        }
        else if (_bossState == BossState.Idle)
        {
            _rigidbody.linearVelocity = Vector2.MoveTowards(_rigidbody.linearVelocity,
            (_playerTransform.position + TargetXOffset * Vector3.left - transform.position)
            * MoveSpeed, Acceleration * Time.deltaTime);
        }
        
    }

    private void HandFollowTarget(ref Rigidbody2D hand, Vector3 center)
    {
        if (_bossState == BossState.PunchAttack) center += 8f * Vector3.right;
        Vector3 forceVector = center + transform.position - hand.transform.position;
        hand.AddForce(forceVector * _elasticConstant);
    }

    private IEnumerator IRandomizeHandTarget()
    {
        while (true)
        {
            _leftHandTarget = RandomizeCenter(_leftHandCenter);
            _rightHandTarget = RandomizeCenter(_rightHandCenter);
            yield return new WaitForSeconds(_swayRandomizePeriod);
        }
    }

    private Vector3 RandomizeCenter(Vector3 center)
    {
        Vector2 newCenter = Random.insideUnitCircle * _swayCenterAmplitude;
        return center + new Vector3(newCenter.x, newCenter.y, 0f);
    }

}


public enum BossState
{
    Idle,
    CigarAttack,
    PunchAttack,
    Stunned,
    Dead,
}