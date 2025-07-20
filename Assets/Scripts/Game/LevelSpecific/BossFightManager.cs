using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] float _initialWait;
    [SerializeField] float _waitToCameraMovement;
    [SerializeField] float minScreenSpeed;
    [SerializeField] float maxDistanceFromEdge;

    [SerializeField] float _yMin;
    [SerializeField] float _yMax;

    [SerializeField] float ShakeAmplitude;
    [SerializeField] float ShakeDuration;

    public float _cameraEdgeX = 0f;
    PolygonCollider2D _polygonCollider2D;
    PlayerMovement _playerMovement;
    BossBehavior _behavior;
    private bool _levelStarted;
    [SerializeField] CinemachineCamera _playerCamera;
    [SerializeField] GameObject _pauseButton;
    [SerializeField] GameObject _finalTarget;
    [SerializeField] GameObject _dynamicBackground;
    [SerializeField] GameObject _cigarPrefab;
    [SerializeField] float _playerStartingY;
    GameManager _gameManager;
    CinemachineBasicMultiChannelPerlin _cameraShake;
    CinemachineConfiner2D _confiner;
    bool _amplitudeIncreasing;
    bool _amplitudeDecreasing;
    bool _isGameOver;
    bool _isPlayerDead;
    public bool _shortStart;

    private void Awake()
    {
        _polygonCollider2D = FindAnyObjectByType<PolygonCollider2D>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _behavior = FindAnyObjectByType<BossBehavior>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _shortStart = PersistentInfo.Instance.LevelCompletionInfo[SceneManager.GetActiveScene().buildIndex].WasShown;
    }

    private void Start()
    {
        _cameraShake = _playerCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        _confiner = _playerCamera.GetComponent<CinemachineConfiner2D>();
        _playerMovement._isInputDisabled = true;

        if (_shortStart)
        {
            StartCoroutine(IBossStandBy(true));
            Instantiate(_cigarPrefab, new Vector3((_playerMovement.transform.position.x + _behavior.transform.position.x)/2f,
                _behavior.transform.position.y, 0f), Quaternion.identity);
            _playerMovement.transform.position = new Vector3(_playerMovement.transform.position.x, _playerStartingY, 0f);
            return;
        }
        StartCoroutine(IBossStandBy());
    }

    private void Update()
    {
        if (!_levelStarted) return;
        _cameraEdgeX = CameraEdgeX(_cameraEdgeX);
        if (!_isGameOver && !_isPlayerDead) UpdateBoundary(_cameraEdgeX, _yMax, _yMin);

        if (_amplitudeDecreasing) _cameraShake.AmplitudeGain = Mathf.Lerp(_cameraShake.AmplitudeGain, 0f, ShakeDuration / 2f);
        else if (_amplitudeIncreasing) _cameraShake.AmplitudeGain = Mathf.Lerp(_cameraShake.AmplitudeGain, ShakeAmplitude, ShakeDuration / 2f);

        if (_isGameOver)
        {
            _confiner.InvalidateBoundingShapeCache();
            _confiner.InvalidateBoundingShapeCache();
        }
    }

    private IEnumerator IBossStandBy(bool skip = false)
    {
        if (skip) { _levelStarted = true; yield break; }
        yield return new WaitForSeconds(_initialWait);
        _behavior.StartCoroutine(_behavior.ICigaretteAttack(true));
        yield return new WaitForSeconds(_waitToCameraMovement);
        _levelStarted = true;
    }


    private void UpdateBoundary(float xMax, float yMax, float yMin)
    {
        float xMin = xMax - 100f;

        _polygonCollider2D.points = new Vector2[4]
        {
            new Vector2(xMin, yMin),
            new Vector2(xMax, yMin),
            new Vector2(xMax, yMax),
            new Vector2(xMin, yMax),
        };
        _confiner.InvalidateBoundingShapeCache();
        _playerMovement.CalculateBounds();
    }

    private float CameraEdgeX(float previousX)
    {
        return Mathf.Min(previousX - minScreenSpeed * Time.deltaTime, _playerMovement.transform.position.x + maxDistanceFromEdge);
    }

    public IEnumerator IVibrateScreen()
    {
        _amplitudeIncreasing = true;
        yield return new WaitForSeconds(ShakeDuration / 2f);
        _amplitudeIncreasing = false;
        _amplitudeDecreasing = true;
        yield return new WaitForSeconds(ShakeDuration / 2f);
        _amplitudeDecreasing = false;
    }

    public void OnBossDying()
    {
        MusicPlayer.Instance.Pause();
        _pauseButton.gameObject.SetActive(false);
        _playerCamera.Target.TrackingTarget = _behavior.transform;
        StartCoroutine(IVibrateScreen());
        UpdateBoundary(_cameraEdgeX + 20f, _yMax, _yMin);

        _isGameOver = true;
        _playerMovement._isInputDisabled = true;
        _playerMovement.ToggleGravity();
    }

    public void OnBossDead()
    {
        StartCoroutine(ISetupEnding());
    }

    private IEnumerator ISetupEnding()
    {
        yield return new WaitForSeconds(5f);

        _gameManager.NextLevel();
        /*yield return new WaitForSeconds(_levelLoader.GetAnimationDuration());
        MusicPlayer.Instance.PlayWind();
        _behavior.gameObject.SetActive(false);
        _dynamicBackground.gameObject.SetActive(false);
        _playerCamera.CancelDamping();
        _playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        _playerMovement.transform.position = new Vector3(-9.54f, -70f, 0f);
        _playerMovement.ToggleGravity();
        _playerCamera.Target.TrackingTarget = _finalTarget.transform;
        UpdateBoundary(0f, 200f , -103.3f);*/
    }

    public void OnPlayerDead()
    {
        _isPlayerDead = true;
    }
}
