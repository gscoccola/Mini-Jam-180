using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementButton : MonoBehaviour
{
    public event EventHandler<ClosePopupEventArgs> PopupClose;

    [SerializeField] AchievementInfoSO _achievement;
    [SerializeField] GameObject _popup;
    [SerializeField] TextMeshProUGUI _achievementTitle;
    [SerializeField] TextMeshProUGUI _achievementDescription;
    [SerializeField] GameObject _confettiPrefab;

    Camera _camera;
    Animator _animator;
    bool _isOnCooldown;

    int Id;
    string Title;

    bool _wasUnlocked;
    bool _wasShown;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _camera = FindAnyObjectByType<Camera>();
    }

    private void OnEnable()
    {
        Id = _achievement.GetId();
        Title = _achievement.GetName();

        _wasUnlocked = PersistentInfo.Instance.AchievementCompletionInfo[Id].WasUnlocked;
        _wasShown = PersistentInfo.Instance.AchievementCompletionInfo[Id].WasShown;

        _animator.SetBool("wasShown", _wasShown);
        _animator.SetBool("wasUnlocked", _wasUnlocked);

        _achievementTitle.text = Title;
        if (_wasUnlocked)
        {
            _achievementDescription.text = _achievement.GetDescription();
            _achievementDescription.fontSize = 62.5f;
            return;
        }
        _achievementDescription.text = "????";
        _achievementDescription.fontSize = 125f;
    }


    public void AdjustPopup()
    {
        if (transform.position.x > Screen.width /2)
        _popup.transform.localPosition = new Vector3(- Mathf.Abs(_popup.transform.localPosition.x), _popup.transform.localPosition.y, 0f);
    }

    public int GetId()
    {
        return _achievement.GetId();
    }

    public string GetTitle()
    {
        return _achievement.GetName();
    }

    public void OnClick()
    {
        if (_isOnCooldown) return;
        bool wasPressed = _popup.gameObject.activeSelf;
        StartCoroutine(IButtonCooldown());
        _isOnCooldown = true;

        AdjustPopup();
        PopupClose.Invoke(this, new ClosePopupEventArgs());
        SetPopup(!wasPressed);

        if (_wasUnlocked && !_wasShown)
        {
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._achievement);
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._button);
            VibrationHandler.Instance.HeavyVibrate();
            Instantiate(_confettiPrefab, new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f), Quaternion.identity)
                .transform.localScale = 2.8296f *(_camera.orthographicSize / 5) * Vector3.one;

            _wasShown = true;
            _animator.SetBool("wasShown", true);

            PersistentInfo.Instance.AchievementCompletionInfo[Id].WasShown = true;
            PersistentInfo.Instance.CheckUnseenAchievements();
            PersistentInfo.Instance.SaveData();
            return;
        }

        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._button);
        VibrationHandler.Instance.LightVibrate();
    }

    IEnumerator IButtonCooldown()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        _isOnCooldown = false;
    }

    public void SetPopup(bool boolean)
    {
        _animator.SetBool("isPressed", boolean);
        _popup.gameObject.SetActive(boolean);
    }

}
