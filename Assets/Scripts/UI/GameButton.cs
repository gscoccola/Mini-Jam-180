using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ButtonType _type;
    [SerializeField] bool _isInstant;
    [SerializeField] int _level = 1;
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
        if (!_isInstant) StartCoroutine(ILoadScene());
        else PerfomButtonAction();
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._button);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    IEnumerator ILoadScene()
    {
        _animator.SetBool("isPressed", true);
        yield return new WaitForSecondsRealtime(_menuClickDelay);
        PerfomButtonAction();
    }


    private void PerfomButtonAction()
    {
        switch (_type)
        {
            case ButtonType.LevelSelect:
                _gameManager.LoadLevel(_level);
                break;
            case ButtonType.Pause:
                _gameManager.Pause();
                break;
            case ButtonType.Start:
                _gameManager.TutorialOrLevelsMenu();
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
    LevelSelect,
    Pause,
}
