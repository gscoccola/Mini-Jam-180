using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    Transform _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        transform.position = _player.position;

        if (transform.position.x < 0.2f)
        {
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            float angle = Mathf.Atan(transform.position.y / transform.position.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90 );
        }
    }
}
