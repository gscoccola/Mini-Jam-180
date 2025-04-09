using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Background : MonoBehaviour
{
    [SerializeField] float displacementRatio = 0.2f;
    float _height;
    Camera _camera;
    [SerializeField] Vector3 offset;
    Vector3 _initialPosition;
    Vector3 virtualPosition;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Start()
    {
        _initialPosition = transform.position;
        virtualPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            transform.position = (1 - displacementRatio) * _camera.transform.position + offset;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            return;
        }

        
        virtualPosition += new Vector3(10 * Time.deltaTime, 0f, 0f);
        transform.position = (1 - displacementRatio) * virtualPosition + offset;

    }
}
