using System.Collections;
using UnityEngine;

public class PersistentCanvas : Singleton<PersistentCanvas>
{

    [SerializeField] float _delay = 3f;
    [SerializeField] Animator _animator;

    public void ShowPopup()
    {
        _animator.SetBool("IsActive", true);
        StartCoroutine(IRemovePopup());
    }

    IEnumerator IRemovePopup()
    {
        yield return new WaitForSecondsRealtime(_delay);
        _animator.SetBool("IsActive", false);
    }
}
