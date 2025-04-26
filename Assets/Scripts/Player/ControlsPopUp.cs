using UnityEngine;
using UnityEngine.Rendering;

public class ControlsPopUp : MonoBehaviour
{
    PlayerMovement _player;
    float alpha;
    [SerializeField] float _offset;
    [SerializeField] float _alphaRate;
    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        transform.position = _player.transform.position + Vector3.up * 2;


        alpha = _player.IsOnFloor ? Mathf.Clamp(alpha + _alphaRate * Time.deltaTime, 0, 1) 
            : Mathf.Clamp(alpha - _alphaRate * Time.deltaTime, 0, 1);

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, alpha);
    }
}
