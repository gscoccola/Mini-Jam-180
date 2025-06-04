using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

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
    CinemachineConfiner2D _cameraConfiner;
    PlayerMovement _playerMovement;
    BossBehavior _behavior;
    private bool _levelStarted;
    CinemachineBasicMultiChannelPerlin _playerCameraShake;
    bool _amplitudeIncreasing;
    bool _amplitudeDecreasing;


    private void Awake()
    {
        _polygonCollider2D = FindAnyObjectByType<PolygonCollider2D>();
        _cameraConfiner = FindAnyObjectByType<CinemachineConfiner2D>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _behavior = FindAnyObjectByType<BossBehavior>();
        _playerCameraShake = FindAnyObjectByType<CinemachineCamera>().GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        _behavior._stunnedXSpeed = minScreenSpeed;
        _playerMovement._isInputDisabled = true;
        StartCoroutine(IBossStandBy());
    }

    private void Update()
    {
        if (!_levelStarted) return;
        _cameraEdgeX = CameraEdgeX(_cameraEdgeX);
        UpdateBoundary(_cameraEdgeX);

        if (_amplitudeDecreasing) _playerCameraShake.AmplitudeGain = Mathf.Lerp(_playerCameraShake.AmplitudeGain, 0f, ShakeDuration / 2f);
        else if (_amplitudeIncreasing) _playerCameraShake.AmplitudeGain = Mathf.Lerp(_playerCameraShake.AmplitudeGain, ShakeAmplitude, ShakeDuration / 2f);
    }

    private IEnumerator IBossStandBy()
    {
        yield return new WaitForSeconds(_initialWait);
        _behavior.StartCoroutine(_behavior.ICigaretteAttack(true));
        yield return new WaitForSeconds(_waitToCameraMovement);
        _levelStarted = true;
    }


    private void UpdateBoundary(float xMax)
    {
        float xMin = xMax - 100f;

        _polygonCollider2D.points = new Vector2[4]
        {
            new Vector2(xMin, _yMin),
            new Vector2(xMax, _yMin),
            new Vector2(xMax, _yMax),
            new Vector2(xMin, _yMax),
        };
        _cameraConfiner.InvalidateBoundingShapeCache();
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
}
