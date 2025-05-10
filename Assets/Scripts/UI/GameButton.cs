using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IButton
{
    [SerializeField] ButtonType _type;
    [SerializeField] bool _isInstant;
    [SerializeField] int _level = 1;
    GameManager _gameManager;
    float _menuClickDelay = 0.35f;
    Animator _animator;
    bool _isOnCooldown;
    public event EventHandler<ClosePopupEventArgs> PopupClose;
    private bool _isDisabled;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (_animator != null && (_type == ButtonType.Achievements || _type == ButtonType.AchievementMenuInGame))
        {
            _animator.SetBool("isVibrating", PersistentInfo.Instance._newAchievement);
        }
    }


    public void OnClick()
    {
        if (_isOnCooldown || _isDisabled) return;
        if (_type == ButtonType.BackFromAchievements || _type == ButtonType.BackFromAchievementMenuInGame|| _type == ButtonType.DeleteSavedData)
        {
            RewardManager rewardManager = FindAnyObjectByType<RewardManager>();
            if (rewardManager != null && rewardManager.DropNextPadlock()) return;
        }
        VibrationHandler.Instance.LightVibrate();
        _isOnCooldown = true;
        if (_animator != null) _animator.SetBool("isPressed", !_animator.GetBool("isPressed"));
        if (!_isInstant) StartCoroutine(IPerformAction());
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


    IEnumerator IPerformAction()
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
                if (PopupClose != null) PopupClose.Invoke(this, new ClosePopupEventArgs());
                break;
            case ButtonType.Next:
                _gameManager.NextLevel();
                break;
            case ButtonType.DeleteSavedData:
                if (PopupClose != null) PopupClose.Invoke(this, new ClosePopupEventArgs());
                _gameManager.DeleteData();
                break;
            case ButtonType.SetLocaleArg:
                _gameManager.SetLocale("es");
                break;
            case ButtonType.SetLocaleEng:
                _gameManager.SetLocale("en");
                break;
            case ButtonType.AchievementMenuInGame:
                _gameManager.AchievementMenuInGame();
                break;
            case ButtonType.BackFromAchievementMenuInGame:
                _gameManager.BackFromAchievementMenuInGame();
                if (PopupClose != null) PopupClose.Invoke(this, new ClosePopupEventArgs());
                break;
        }
    }

    public void SetPressed()
    {
        _animator.SetBool("isPressed", true);
    }

    public void Enable()
    {
        _isDisabled = false;
    }

    public void Disable()
    {
        _isDisabled = true;
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
    DeleteSavedData,
    SetLocaleArg,
    SetLocaleEng,
    AchievementMenuInGame,
    BackFromAchievementMenuInGame,
}
