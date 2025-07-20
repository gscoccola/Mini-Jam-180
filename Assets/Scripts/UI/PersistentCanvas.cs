using System;
using System.Collections;
using TMPro;
using UnityEngine;

// This Singleton class controls the persistent canvas, which allows popups to persist when changing scenes

public class PersistentCanvas : Singleton<PersistentCanvas>
{

    [SerializeField] float _delay = 3f;
    [SerializeField] GameObject _popupPrefab;

    public void OnLevelLoad()
    {
        GetComponent<Canvas>().worldCamera = FindAnyObjectByType<Camera>();
        GetComponent<Canvas>().sortingLayerName = "Popup";
    }

public void ShowPopup()
    {
        GameObject popup = Instantiate(_popupPrefab, transform.position, Quaternion.identity, transform);
        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(300f, 85f);
        popup.GetComponent<Animator>().SetBool("IsActive", true);
        StartCoroutine(IRemovePopup(popup));
    }

    IEnumerator IRemovePopup(GameObject popup)
    {
        yield return new WaitForSecondsRealtime(_delay);
        popup.GetComponent<Animator>().SetBool("IsActive", false);
        yield return new WaitForSecondsRealtime(_delay);
        Destroy(popup);
    }
}
