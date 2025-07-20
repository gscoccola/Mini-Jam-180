using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody2D _rigidBody2D;
    [SerializeField] float _speed = 10f;

    [SerializeField] GameObject _explosionPrefab;
    [SerializeField] float _downwardVelocity = 10f;

    bool _isDestroyed;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
         _rigidBody2D.linearVelocity = _speed * Vector3.down;
    }

    private void Update()
    {
        if (transform.position.y < 0f && !_isDestroyed) Die();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDestroyed && !collider.GetComponent<PlayerMovement>().IsDead)
        {
            collider.gameObject.GetComponent<PlayerMovement>().MissileHit(_downwardVelocity);
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._missileHit);
            VibrationHandler.Instance.MediumVibrate();
            Die();
        }
    }

    void Die()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _isDestroyed = true;
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._fireworkExplosion);
        Destroy(gameObject, 0.01f);
    }
}
