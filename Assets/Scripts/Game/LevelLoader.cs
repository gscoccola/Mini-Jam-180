using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _animationDuration = 0.3f;

    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        StartCoroutine(ILoadLevel(levelIndex));
    }

    IEnumerator ILoadLevel(int levelIndex)
    {
        _animator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(_animationDuration);
        SceneManager.LoadScene(levelIndex);
    }
}
