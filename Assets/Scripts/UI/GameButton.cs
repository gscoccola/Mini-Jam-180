using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] ButtonType _type;
    [SerializeField] bool _isInstant;
    [SerializeField] int _level = 1;
    GameManager _gameManager;
    float _menuClickDelay = 0.35f;
    Animator _animator;
    bool _isOnCooldown;
    public event EventHandler<PopupEventHandler> PopupClose;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isOnCooldown) return;
        _isOnCooldown = true;
        _animator.SetBool("isPressed", !_animator.GetBool("isPressed")) ;
        if (!_isInstant) StartCoroutine(ILoadScene());
        else 
        {
            PerfomButtonAction();
            StartCoroutine(IButtonCooldown());
        }
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._button);
    }

    IEnumerator IButtonCooldown()
    {

        yield return new WaitForSecondsRealtime(0.15f);
        _isOnCooldown = false;
    }


    IEnumerator ILoadScene()
    {
        
        yield return new WaitForSecondsRealtime(_menuClickDelay);
        PerfomButtonAction();
        _isOnCooldown = false;
    }


    private void PerfomButtonAction()
    {
        switch (_type)
        {
            case ButtonType.Tutorial:
                _gameManager.ShowTutorial();
                break;
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
                _gameManager.ToggleCredits();
                break;
            case ButtonType.Achievements:
                _gameManager.ToggleAchievements();
                break;
            case ButtonType.BackFromAchievements:
                _gameManager.ToggleAchievements();
                PopupClose.Invoke(this, new PopupEventHandler());
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
    Tutorial,
    Achievements,
    BackFromAchievements,
}
