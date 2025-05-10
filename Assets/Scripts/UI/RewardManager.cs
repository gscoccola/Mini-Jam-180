using System.Collections;
using System.Linq;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] float _padlockInputDisable;
    [SerializeField] float _chestInputDisable;
    [SerializeField] float _balloonDelay;


    [SerializeField] GameObject _padlockGrid;
    [SerializeField] GameObject _popup;
    [SerializeField] Animator[] _padlocksAnim;
    [SerializeField] Animator _chestAnim;
    [SerializeField] UIBalloons _UIBalloons;

    [SerializeField] GameButton[] _buttons1;
    [SerializeField] AchievementButton[] _buttons2;

    int shownCount;
    int nextPadlock = 0;

    private void OnEnable()
    {
        shownCount = PersistentInfo.Instance.AchievementCompletionInfo.Where(x => x.Value.WasShown).Count();
        nextPadlock = shownCount;
        if (shownCount == 0) return;
        if (shownCount == _padlocksAnim.Length)
        {
            _chestAnim.SetTrigger("WasOpened");
            _padlockGrid.SetActive(false);
            _popup.SetActive(true);
            return;
        }
        for (int i= 0; i < shownCount; i++)
        {
            _padlocksAnim[i].gameObject.SetActive(false);
        }
    }

    public bool DropNextPadlock()
    {
        shownCount = PersistentInfo.Instance.AchievementCompletionInfo.Where(x => x.Value.WasShown).Count();
        if (nextPadlock == shownCount) return false;
        _padlocksAnim[nextPadlock].SetTrigger("Open");
        StartCoroutine(IDisableControls(_padlockInputDisable));
        nextPadlock++;
        return true;
    }

    private void OpenChest()
    {
        _chestAnim.SetTrigger("Open");
        _padlockGrid.SetActive(false);
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._chestOpen);
    }

    private IEnumerator IDisableControls(float delay)
    {
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._padlock);
        foreach (var button in _buttons1)
        {
            button.Disable();
        }
        foreach (var button in _buttons2)
        {
            button.Disable();
        }

        yield return new WaitForSecondsRealtime(_padlockInputDisable);

        if (nextPadlock == _padlocksAnim.Length)
        {
            OpenChest();
            yield return new WaitForSecondsRealtime(_balloonDelay);
            _UIBalloons.gameObject.SetActive(true);
            SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._balloonRelease);
            yield return new WaitForSecondsRealtime(_balloonDelay);
            _popup.SetActive(true);
            yield return new WaitForSecondsRealtime(_chestInputDisable- 2*_balloonDelay);
        }

        foreach (var button in _buttons1)
        {
            button.Enable();
        }
        foreach (var button in _buttons2)
        {
            button.Enable();
        }
    }


}
