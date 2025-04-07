using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ButtonType _type;
    GameManager _gameManager;
    float _menuClickDelay = 0.6f;
    Animator _animator;
    SFXPlayer _SFXPlayer;
    MusicPlayer _musicPlayer;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _animator = GetComponent<Animator>();
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _musicPlayer = FindAnyObjectByType<MusicPlayer>();
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
                _gameManager.TutorialOrStart();
                break;
            case ButtonType.Quit:
                _gameManager.LoadLevel(0);
                break;
            case ButtonType.Restart:
                _gameManager.LoadLevel(SceneManager.GetActiveScene().buildIndex);
                break;
            case ButtonType.Credits:
                _gameManager.ToggleCredits(); ;
                break;
            case ButtonType.Next:
                {
                    _gameManager.NextLevel();
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
