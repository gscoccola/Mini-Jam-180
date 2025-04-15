using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEndlessTeleporter : MonoBehaviour
{
    Transform _playerTransform;
    bool _isGameOver;
    [SerializeField] InGameMenu _credits;
    CinemachineCamera _camera;
    float velocity;
    CinemachineConfiner2D _confiner;
    PlayerMovement _playerMovement;
    GameManager _gameManager;
    [SerializeField] GameObject _confetti;
     FireworkSpawner _fireworkSpawner;
    const float period = 8.64864864865f;

    private void Awake()
    {
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
         _playerTransform = _playerMovement.transform;
        _camera = FindAnyObjectByType<CinemachineCamera>();
        _confiner = _camera.gameObject.GetComponent<CinemachineConfiner2D>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _fireworkSpawner = FindAnyObjectByType<FireworkSpawner>();
    }

    private void Update()
    {
        if (!_isGameOver  && _playerTransform.position.y < 17f)
        {
            _isGameOver = true;
            StartCoroutine(IGameOver());
            MusicPlayer.Instance._beginFadeOutWind = true;
        }

        if (_isGameOver)
        {
            _confiner.InvalidateBoundingShapeCache();
            _camera.Lens.FieldOfView = Mathf.SmoothDamp(_camera.Lens.FieldOfView, 44, ref velocity, 17.5f/2);
        }

        if (_playerTransform.position.y < -1200f)
        {
            _playerTransform.position = new Vector3(_playerTransform.position.x, -100f, 0f);
        }
    }

    void Start()
    {
       // _fireworkSpawner.Begin();
    }

    IEnumerator IGameOver()
    {
        yield return new WaitForSeconds(3f);
        MusicPlayer.Instance.Play();

        yield return new WaitForSeconds(period -2f);
        _fireworkSpawner.Begin();

        yield return new WaitForSeconds(period +2);
        _credits.gameObject.SetActive(true);
    }
}
