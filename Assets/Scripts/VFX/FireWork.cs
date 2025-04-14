using UnityEngine;
using UnityEngine.SceneManagement;

public class FireWork : MonoBehaviour
{
    [SerializeField] float displacementRatio = 0.2f;
    Camera _camera;
    [SerializeField] Vector3 offset;
    Vector3 virtualPosition;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Update()
    {
        transform.position = (1 - displacementRatio) * _camera.transform.position + offset;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }
}
