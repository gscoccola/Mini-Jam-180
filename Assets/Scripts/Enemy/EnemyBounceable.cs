using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class EnemyBounceable : MonoBehaviour
{
    [SerializeField] GameObject _explosionPrefab;
    public EnemyType _enemytype;
    //public float _maxXValue = 200f;
    bool _isDestroyed;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_enemytype == EnemyType.Drone) GetComponent<SpriteRenderer>().color = Color.grey;
        if (_enemytype == EnemyType.Balloon) GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDestroyed && collider.tag == "Player" )
        {
            collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype);
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isDestroyed && collision.collider.tag == "Player")
        {
            collision.collider.gameObject.GetComponent<PlayerMovement>().Boost(_enemytype);
            Die();
        }
        else if (!_isDestroyed && collision.collider.tag == "Ground")
        {
            Die();
        }
    }

    private void Update()
    {
        //if (transform.position.x > _maxXValue) Destroy(gameObject);
    }

    void Die()
    {
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        _isDestroyed = true;
        Destroy(gameObject, 0.01f);
    }

}

public enum EnemyType
{
    Bird,
    Drone,
    Balloon,
}
