using UnityEngine;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;
    PlayerMovement _playerMovement;
    [SerializeField] GameObject _bloodSplatter;
    [SerializeField] GameObject _waterSplash;

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (collision.collider.transform.position.x > 0f)
            {
                _gameManager.Lose();
                _playerMovement.Die();
                _SFXPlayer.PlayRandomClip(_SFXPlayer._playerDeath);
                Instantiate(_bloodSplatter, _playerMovement.transform.position, Quaternion.identity);

                return;
            }
            _gameManager.Win();
            _SFXPlayer.PlayRandomClip(_SFXPlayer._playerWin);
            Instantiate(_waterSplash, _playerMovement.transform.position, Quaternion.identity);
        }
    }
}
