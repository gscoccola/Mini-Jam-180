using UnityEngine;


// This class handles the interaction between enemy and the player
public class EnemyBounceable : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    public EnemyType _enemytype;
    bool _isDestroyed;
    bool _isRedirected;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (!_isDestroyed)
        {
            if (collider.gameObject.tag == "Boss") collider.gameObject.GetComponent<BossBehavior>().TakeDamage();
            else
            {
                if (Vector3.Dot(collider.transform.position - transform.position, Vector3.left) > 0.2f)
                {
                    collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype, false);
                }
                else
                {
                    collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype);
                }
            }
            
            PlayDeathSFX();
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isDestroyed)
        {
            
            collision.collider.gameObject.GetComponent<PlayerMovement>().Boost(_enemytype);
            PlayDeathSFX();
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerBounce);
            Die(true);
        }
    }

    void Die(bool redirect = false) 
    {
        if (_enemytype == EnemyType.Cigarette)
        {
            if (redirect && !_isRedirected)
            {
                _isRedirected = true;
                transform.localScale = new Vector3(-1f, 1f, 1f);
                GetComponent<Rigidbody2D>().linearVelocity *= -2.5f;
                GetComponent<CapsuleCollider2D>().enabled = false;
                gameObject.layer = LayerMask.NameToLayer("RedirectedProyectile");
                return;
            }
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _isDestroyed = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject, 5f);
            return;
        }
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _isDestroyed = true;
        Destroy(gameObject, 0.01f);
    }

    void PlayDeathSFX()
    {
        switch (_enemytype)
        {
            case EnemyType.Bird:
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._birdDeath);
                break;

            case EnemyType.Drone:
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._droneDeath);
                break;

            case EnemyType.DronePink:
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._droneShock);
                break;

            case EnemyType.Balloon:
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._balloonDeath);
                break;
            case EnemyType.Cigarette:
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossCigarHit);
                break;
        }
    }

}

public enum EnemyType
{
    Bird,
    Drone,
    DronePink,
    Balloon,
    GoldenBalloon,
    Cigarette,
}
