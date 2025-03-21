using UnityEngine;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;
    PlayerMovement _playerMovement;
    [SerializeField] GameObject _bloodSplatter;
    [SerializeField] GameObject _waterSplash;
    bool _isTriggered;

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isTriggered && collision.tag == "Player")
        {
            _isTriggered = true;
            if (collision.transform.position.x > 0f)
            {
                _gameManager.Lose();
                _playerMovement.Die();
                _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerDeath);
                Instantiate(_bloodSplatter, _playerMovement.transform.position, Quaternion.identity);

                return;
            }
            _gameManager.Win();
            _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerSplash);
            _playerMovement.Win();
            Instantiate(_waterSplash, _playerMovement.transform.position, Quaternion.identity);
        }
    }
}
