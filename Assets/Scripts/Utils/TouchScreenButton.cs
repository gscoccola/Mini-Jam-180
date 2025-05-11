using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class TouchScreenButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] TouchScreenButtonType Direction;
    PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _playerMovement.TouchMove(Direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _playerMovement.TouchMove(TouchScreenButtonType.None);
    }
}

public enum TouchScreenButtonType
{
    Left,
    Right,
    None,
}
