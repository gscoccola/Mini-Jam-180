using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour
{

    [SerializeField] float _startDelay = 2f;
    [SerializeField] float _panDuration = 5f;
    [SerializeField] float _cameraMaxSpeed = 25f;
    float _cameraHorizontalSpeed;
    float _cameraVerticalSpeed;
    //[SerializeField] float _endDelay = 1f;
    [SerializeField] Transform _virtualTarget;
    CinemachineCamera _playerCamera;
    CinemachineCamera _panCamera;
    bool _isPanningHorizontally;
    bool _isPanningVertically;
    PlayerMovement _playerMovement;
    GameManager _gameManager;

    Vector3 _startPosition;

    private void Awake()
    {
        _playerCamera = transform.GetChild(4).GetComponent<CinemachineCamera>();
        _panCamera = transform.GetChild(3).GetComponent<CinemachineCamera>();
        _playerMovement =FindAnyObjectByType<PlayerMovement>();
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        CalculateSpeed();
    }

    private void CalculateSpeed()
    {
        _cameraHorizontalSpeed = Mathf.Min (_cameraMaxSpeed, 2 * Mathf.Abs(_virtualTarget.position.x - _playerMovement.transform.position.x) / _panDuration);
        _cameraVerticalSpeed = Mathf.Min (_cameraMaxSpeed, 2 * Mathf.Abs(_virtualTarget.position.y - _playerMovement.transform.position.y) / _panDuration);
    }

    private void FixedUpdate()
    {
        if (_isPanningHorizontally) _virtualTarget.position =
        Vector2.MoveTowards(_virtualTarget.position,
        new Vector3(_playerMovement.transform.position.x, _virtualTarget.position.y, 0f), 
        _cameraHorizontalSpeed * Time.deltaTime);

        else if (_isPanningVertically) _virtualTarget.position =
        Vector2.MoveTowards(_virtualTarget.position,
        _playerMovement.transform.position,
        _cameraVerticalSpeed * Time.deltaTime);

        if ((_isPanningHorizontally || _isPanningVertically) && (_virtualTarget.position - _playerMovement.transform.position).magnitude < 8f )
        {
            _gameManager.AlmostStartLevel();
        }

        if (_isPanningHorizontally && Mathf.Abs(_virtualTarget.position.x - _playerMovement.transform.position.x) < 1f)
        {
            _isPanningVertically = true;
            _isPanningHorizontally = false;
        }

        else if (_isPanningVertically && Mathf.Abs(_virtualTarget.position.y - _playerMovement.transform.position.y) < 1f)
        {
            _isPanningVertically = false;
            _gameManager.StartLevel();
            SwitchToPlayerCam();
        }
    }

    public void SwitchToPlayerCam()
    {
        _playerCamera.Priority = 10;
        Destroy(_panCamera.transform.gameObject, 2f);
    }

    public void SetStartPosition(float xCoord)
    {
        _startPosition = new Vector3 (xCoord, 8f, 0f);
    }

    public IEnumerator ICameraPan()
    {
        _virtualTarget.position = _startPosition;
        _playerMovement.IsHorizonalMovementDisabled = true;
        yield return new WaitForSeconds(_startDelay);
        _isPanningHorizontally = true;
    }

}
