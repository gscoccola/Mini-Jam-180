using Alchemy.Inspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



public class EnemyBounceable : MonoBehaviour
{

    public EnemyType _enemytype;
    public float _maxXValue = 200f;
    bool _isDestroyed;

    private void Awake()
    {
        if (_enemytype == EnemyType.Drone) GetComponent<SpriteRenderer>().color = Color.grey;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isDestroyed && collider.tag == "Player" )
        {
            collider.gameObject.GetComponent<PlayerMovement>().Deviate(_enemytype);
            _isDestroyed = true;
            Destroy(gameObject, 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isDestroyed && collision.collider.tag == "Player")
        {
            collision.collider.gameObject.GetComponent<PlayerMovement>().Boost(_enemytype);
            _isDestroyed = true;
            Destroy(gameObject, 0.1f);
        }
    }

    private void Update()
    {
        if (transform.position.x > _maxXValue) Destroy(gameObject);
    }

}

public enum EnemyType
{
    Bird,
    Drone,
    Tender,
}
