using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ButtonType _type;
    GameManager _gameManager;
    float _menuClickDelay = 1.0f;
    bool _isBeingPressed;
    Animator _animator;
    Camera _camera;
    SFXPlayer _SFXPlayer;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _animator = GetComponent<Animator>();
        _camera = FindAnyObjectByType<Camera>();
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(ILoadScene());
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._button);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    IEnumerator ILoadScene()
    {
        _animator.SetBool("isPressed", true);
        yield return new WaitForSeconds(_menuClickDelay);
        LoadScene();
    }


    private void LoadScene()
    {
        switch (_type)
        {
            case ButtonType.Start:
                _gameManager.LoadLevel(1);
                break;
            case ButtonType.Quit:
                _gameManager.LoadLevel(0);
                break;
            case ButtonType.Restart:
                _gameManager.LoadLevel(SceneManager.GetActiveScene().buildIndex);
                break;
            case ButtonType.Next:
                {
                    int i = SceneManager.GetActiveScene().buildIndex;
                    i++;
                    i = i % 5;
                    _gameManager.LoadLevel(i);
                    break;
                }
        }
    }

}

public enum ButtonType
{
    Start,
    Quit,
    Next,
    Restart,
    Credits,
}
