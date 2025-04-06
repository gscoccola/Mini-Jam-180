using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class EnemyBounceable : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    public EnemyType _enemytype;
    float _maxXValue = 250f;
    bool _isDestroyed;
    Animator _animator;
    SFXPlayer _SFXPlayer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDestroyed && collider.tag == "Player" )
        {
            if ( Vector3.Dot(collider.transform.position - transform.position, Vector3.left) > 0.2f)
            {
                collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype, false);
            }
            else
            {
                collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype);
            }
            _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerHurt);
            PlayDeathSFX();
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isDestroyed && collision.collider.tag == "Player")
        {
            collision.collider.gameObject.GetComponent<PlayerMovement>().Boost(_enemytype);
            PlayDeathSFX();
            _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerBounce);
            Die();
        }
    }

    private void Update()
    {
        if (transform.position.x > _maxXValue) Destroy(gameObject);
    }

    void Die()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _isDestroyed = true;
        Destroy(gameObject, 0.01f);
    }

    void PlayDeathSFX()
    {
        switch (_enemytype)
        {
            case EnemyType.Bird:
                _SFXPlayer.PlayClipFromArray(_SFXPlayer._birdDeath);
                break;

            case EnemyType.Drone:
                _SFXPlayer.PlayClipFromArray(_SFXPlayer._droneDeath);
                break;

            case EnemyType.Balloon:
                _SFXPlayer.PlayClipFromArray(_SFXPlayer._balloonDeath);
                break;
        }
    }

}

public enum EnemyType
{
    Bird,
    Drone,
    Balloon,
}
