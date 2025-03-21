using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    Transform _player;
    float _minBound;
    float _maxBound;

    public void SetBounds(float minBound, float maxBound)
    {
        _maxBound = maxBound;
        _minBound = minBound;
    }

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        transform.position = _player.position;

        if (transform.position.x < _minBound)
        {
            float angle = Mathf.Atan(transform.position.y / (transform.position.x- _minBound)) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle - 270);
            return;
        }

        if ( transform.position.x > _maxBound)
        {
            float angle = Mathf.Atan(transform.position.y / (transform.position.x - _maxBound)) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90);
            return;
        }

        transform.localRotation = Quaternion.identity;
    }
}
