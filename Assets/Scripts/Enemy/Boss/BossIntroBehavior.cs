using UnityEngine;

public class BossIntroBehavior : MonoBehaviour
{
    [SerializeField] float BossStartSpeed;
    [SerializeField] float BossEndSpeed;
    [SerializeField] float _verticalBoost;
    [SerializeField] float _horizontalBoost;
    [SerializeField] float _deceleration;
    [SerializeField] float _maxHeight;

    bool _hasHitPlayer;
    Rigidbody2D _rigidBody;
    GameManager _gameManager;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    public void StartMovement()
    {
        _rigidBody.linearVelocityY = BossStartSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.GetComponent<PlayerMovement>()._allowHorizontalSpeed = true;
        
        collider.GetComponent<Rigidbody2D>().linearVelocity = new Vector3 (
            (collider.transform.position - transform.position).normalized.x * _horizontalBoost,
            _verticalBoost, 0f);
        collider.gameObject.layer = LayerMask.NameToLayer("PlayerInvulnerable");

        _hasHitPlayer = true;
    }

    private void Update()
    {
        if (!_hasHitPlayer) return;
        _rigidBody.linearVelocity = Vector3.MoveTowards(_rigidBody.linearVelocity, Vector3.up * BossEndSpeed, _deceleration * Time.deltaTime);
        if (transform.position.y >= _maxHeight) _gameManager.NextLevel();
    }
}
