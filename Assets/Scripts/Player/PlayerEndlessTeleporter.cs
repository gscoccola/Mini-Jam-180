using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerEndlessTeleporter : MonoBehaviour
{
    Transform _playerTransform;
    MusicPlayer _musicPlayer;
    bool _isGameOver;
    [SerializeField] InGameMenu _credits;
    CinemachineCamera _camera;
    float velocity;
    CinemachineConfiner2D _confiner;
    PlayerMovement _playerMovement;
    GameManager _gameManager;

    private void Awake()
    {
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
         _playerTransform = _playerMovement.transform;
        _camera = FindAnyObjectByType<CinemachineCamera>();
        _confiner = _camera.gameObject.GetComponent<CinemachineConfiner2D>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _musicPlayer = FindAnyObjectByType<MusicPlayer>();
    }

    private void Update()
    {
        if (!_isGameOver  && _playerTransform.position.y < 17f)
        {
            _isGameOver = true;
            _gameManager.DisableMobileInput();
            _playerMovement.IsHorizonalMovementDisabled = true;
            StartCoroutine(IGameOver());
            _musicPlayer._beginFadeOutWind = true;
        }

        if (_isGameOver)
        {
            _confiner.InvalidateBoundingShapeCache();
            _camera.Lens.FieldOfView = Mathf.SmoothDamp(_camera.Lens.FieldOfView, 44, ref velocity, 17.5f);
        }

        if (_playerTransform.position.y < -1200f)
        {
            _playerTransform.position = new Vector3(_playerTransform.position.x, -100f, 0f);
        }
    }

    IEnumerator IGameOver()
    {
        yield return new WaitForSeconds(3f);
        _musicPlayer.Play();

        yield return new WaitForSeconds(17.35f);
        _credits.gameObject.SetActive(true);
    }
}
