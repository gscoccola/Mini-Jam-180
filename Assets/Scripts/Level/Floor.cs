using UnityEngine;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (collision.collider.transform.position.x > 0f)
            {
                _gameManager.Lose();
                return;
            }
            _gameManager.Win();
        }
    }
}
