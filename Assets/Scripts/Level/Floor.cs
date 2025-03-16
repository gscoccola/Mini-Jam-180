using UnityEngine;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (collision.collider.transform.position.x > 0f)
            {
                _gameManager.Lose();
                _SFXPlayer.PlayRandomClip(_SFXPlayer._playerDeath);
                return;
            }
            _gameManager.Win();
            _SFXPlayer.PlayRandomClip(_SFXPlayer._playerWin);
        }
    }
}
