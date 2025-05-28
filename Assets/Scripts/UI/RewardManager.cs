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
        VibrationHandler.Instance.MediumVibrate();
        StartCoroutine(IDisableControls(_padlockInputDisable));
        nextPadlock++;
        return true;
    }

    private void OpenChest()
    {
        _chestAnim.SetTrigger("Open");
        VibrationHandler.Instance.MediumVibrate();
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._chestOpen);
    }

    private IEnumerator IDisableControls(float delay)
    {
        SFXPlayer.Instance.PlayClipFromArrayAtIndex(SFXPlayer.Instance._padlock, nextPadlock);
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
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._balloonRelease);
            VibrationHandler.Instance.HeavyVibrate();
            yield return new WaitForSecondsRealtime(_balloonDelay + 1f);
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
