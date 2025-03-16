using Unity.Cinemachine;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] float displacementRatio = 0.2f;
    float _height;
    Camera _camera;
    [SerializeField] Vector3 offset;
    Vector3 _initialPosition;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Start()
    {
        _initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        transform.position = (1 - displacementRatio) * _camera.transform.position +  offset;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }
}
