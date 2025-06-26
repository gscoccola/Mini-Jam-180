using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Background : MonoBehaviour
{
    [SerializeField] float displacementRatio = 0.2f;
    Camera _camera;
    [SerializeField] Vector3 offset;
    Vector3 virtualPosition;
    bool _isSceneZero;
    PixelPerfectCamera _pixelPerfectCamera;
    public bool _printPosition;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _pixelPerfectCamera = _camera.GetComponent<PixelPerfectCamera>();
    }

    private void Start()
    {
        virtualPosition = Vector3.zero;
        _isSceneZero = SceneManager.GetActiveScene().buildIndex == 0;
    }

    private void Update()
    {
        //Vector3 target = _isSceneZero ? _camera.transform.position : Vector3.zero;

        if (!_isSceneZero)
        {
            transform.position = UpdatedPosition(_pixelPerfectCamera.RoundToPixel(_camera.transform.position));
            //transform.position = _camera.transform.position;
            return;
        }

        virtualPosition += new Vector3(10 * Time.deltaTime, 0f, 0f);
        transform.position = UpdatedPosition(virtualPosition);

    }

    private Vector3 UpdatedPosition(Vector3 target)
    {
        Vector3 outputPosition = (1 - displacementRatio) * target + offset;
        outputPosition = new Vector3(outputPosition.x, outputPosition.y, 0f);
        return outputPosition;
    }
}
