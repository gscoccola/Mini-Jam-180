using UnityEngine;



public class EnemyBounceable : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    public EnemyType _enemytype;
    bool _isDestroyed;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDestroyed && !collider.GetComponent<PlayerMovement>().IsDead)
        {
            if ( Vector3.Dot(collider.transform.position - transform.position, Vector3.left) > 0.2f)
            {
                collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype, false);
            }
            else
            {
                collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype);
            }
            PlayDeathSFX();
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isDestroyed && !collision.collider.GetComponent<PlayerMovement>().IsDead)
        {
            collision.collider.gameObject.GetComponent<PlayerMovement>().Boost(_enemytype);
            PlayDeathSFX();
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._playerBounce);
            Die();
        }
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
                SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._birdDeath);
                break;

            case EnemyType.Drone:
                SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._droneDeath);
                break;

            case EnemyType.Balloon:
                SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._balloonDeath);
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
