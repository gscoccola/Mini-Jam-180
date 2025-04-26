using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PersistentCanvas : Singleton<PersistentCanvas>
{

    [SerializeField] float _delay = 3f;
    [SerializeField] GameObject _popupPrefab;

    public void ShowPopup(string text)
    {
        GameObject popup = Instantiate(_popupPrefab, transform.position, Quaternion.identity, transform);
        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(300f, 85f);
        popup.GetComponent<Animator>().SetBool("IsActive", true);
        popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Achievement"+ Environment.NewLine + "Unlocked: " + Environment.NewLine + text;
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
