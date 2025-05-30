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

    public bool _isActive;
    bool _isShooting;

    int a = 0;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _enemies = GameObject.Find("Enemies");
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(IRandomizeCenters());
    }

    public IEnumerator ICigaretteAttack()
    {
        _animator.SetTrigger("Shoot");
        _isShooting = true;
        _attackOnCooldown = true;
        yield return new WaitForSeconds(0.2f);
        Instantiate(Projectile, transform.position + new Vector3(-hoverXOffset, -hoverYOffset, 0f), Quaternion.identity, _enemies.transform);
        yield return new WaitForSeconds(AttackMovementPause - 0.2f);
        _isShooting = false;
        yield return new WaitForSeconds(AttackDelay - AttackMovementPause - 0.2f);
        _attackOnCooldown = false;
    }

    private void Update()
    {
        HandFollowBody(ref RightHand, _rightHandTarget);
        HandFollowBody(ref LeftHand, _leftHandTarget);

        if (!_isActive) return;
        if (!_attackOnCooldown &&
            Mathf.Abs(transform.position.y - hoverYOffset - _playerTransform.position.y) <= MaxYOffsetToAttack)
            StartCoroutine(ICigaretteAttack());
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

    private void HandFollowBody(ref Rigidbody2D hand, Vector3 center)
    {
        /*hand.transform.position = transform.position + center 
            + _swayAmplitude * Mathf.Cos( 2 * Mathf.PI * _swayFrequency * Time.time) * Vector3.left  ;*/

        /*Vector3 target = Random.insideUnitSphere * _swayAmplitude;*/

        Vector3 forceVector = center + transform.position - hand.transform.position;
        hand.AddForce( /*forceVector.magnitude * */forceVector * _elasticConstant);
    }

    private IEnumerator IRandomizeCenters()
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
