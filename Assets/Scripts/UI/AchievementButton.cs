using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour, IPointerDownHandler
{
    public event EventHandler<PopupEventHandler> PopupClose;

    [SerializeField] AchievementInfoSO _achievement;
    [SerializeField] GameObject _popup;
    [SerializeField] TextMeshProUGUI _achievementTitle;
    [SerializeField] TextMeshProUGUI _achievementDescription;

    bool _isUnlocked;
    Animator _animator;
    bool _isOnCooldown;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
         _achievementTitle.text = _achievement.GetName();
        if (_isUnlocked)
        {
            _achievementDescription.text = _achievement.GetDescription();
            _achievementDescription.fontSize = 80f;
            return;
        }
        _achievementDescription.text = "????";
        _achievementDescription.fontSize = 180f;

        StartCoroutine(AdjustPopup());
    }

    IEnumerator AdjustPopup()
    {
        yield return new WaitForEndOfFrame();
        if (transform.position.x > Screen.width /2)
        _popup.transform.localPosition = new Vector3(-_popup.transform.localPosition.x, _popup.transform.localPosition.y, 0f);
    }

    public int GetId()
    {
        return _achievement.GetId();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isOnCooldown) return;
        _isOnCooldown = true;
        bool wasPressed = _popup.gameObject.activeSelf;
        PopupClose.Invoke(this, new PopupEventHandler());
        SetPopup(!wasPressed);
        StartCoroutine(IButtonCooldown());
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._button);
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

    public void Unlock()
    {
        _isUnlocked = true;
    }

}
