using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _animationDuration = 0.3f;


    private void Awake()
    {
        transform.GetChild(0).GetComponent<Canvas>().worldCamera = FindAnyObjectByType<Camera>();
    }

    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        StartCoroutine(ILoadLevel(levelIndex));
    }

    public void Transition()
    {
        StartCoroutine(ITransition());
    }

    public float GetAnimationDuration()
    {
        return _animationDuration;
    }

    IEnumerator ILoadLevel(int levelIndex)
    {
        _animator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(_animationDuration);
        SceneManager.LoadScene(levelIndex);
    }

    IEnumerator ITransition()
    {
        _animator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(_animationDuration);
        _animator.SetTrigger("End");
    }
}
