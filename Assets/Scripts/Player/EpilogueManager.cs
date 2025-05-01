using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueManager : MonoBehaviour
{
    Transform _playerTransform;
    bool _isGameOver;
    [SerializeField] InGameMenu _credits;
    [SerializeField] InGameMenu _postCreditButtons;
    [SerializeField] InGameMenu _mobileScreenButtons;
    [SerializeField] GameButton _pauseButton;
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
        if (!_isGameOver  && _playerMovement.IsHorizonalInputDisabled)
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

        if (_playerTransform.position.y < -2000f)
        {
            _playerTransform.position = new Vector3(_playerTransform.position.x, 0f, 0f);
        }
    }


    IEnumerator IGameOver()
    {
        _mobileScreenButtons.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(6f);
        MusicPlayer.Instance.Play();

        yield return new WaitForSeconds(period -2f);
        _fireworkSpawner.Begin();
        _gameManager.OnAchievementUnlocked(2);
        PersistentInfo.Instance.LevelCompletionInfo[SceneManager.GetActiveScene().buildIndex].WasBeaten = true;

        yield return new WaitForSeconds(period +2);
        _credits.gameObject.SetActive(true);

        yield return new WaitForSeconds(period/2);
        _postCreditButtons.gameObject.SetActive(true);
    }
}
