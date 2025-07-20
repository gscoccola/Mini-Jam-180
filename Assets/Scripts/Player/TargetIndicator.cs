using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    [SerializeField] float TargetOffset = 1f;
    [SerializeField] SpriteRenderer _spriteRenderer;
    Transform _player;
    float _minBound;
    float _maxBound;

    public void SetBounds(float minBound, float maxBound)
    {
        float center = (minBound + maxBound) / 2f;
        _maxBound = Mathf.Max( maxBound - TargetOffset, center);
        _minBound = Mathf.Min(minBound + TargetOffset, center);
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
            float angle = Mathf.Clamp( 
                Mathf.Atan( transform.position.y * 0.8f / (transform.position.x - _minBound)) * Mathf.Rad2Deg ,
                -90f, -5f);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle - 270);
            _spriteRenderer.enabled = new Vector2(transform.position.x - _minBound, transform.position.y).magnitude > 12f;
        }

        else if ( transform.position.x > _maxBound)
        {
            float angle = Mathf.Clamp(
                Mathf.Atan(transform.position.y * 0.8f / (transform.position.x - _maxBound)) * Mathf.Rad2Deg,
                5f, 90f);
            transform.localRotation = Quaternion.Euler(0f, 0f, angle - 90);
            _spriteRenderer.enabled = new Vector2(transform.position.x - _maxBound, transform.position.y).magnitude > 12f;
        }

        else
        {
            transform.localRotation = Quaternion.identity;
            _spriteRenderer.enabled = transform.position.y > 12f;
        }
            
    }
}
