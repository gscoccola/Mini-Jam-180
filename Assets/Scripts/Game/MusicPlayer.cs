using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake()
    {
        //ManageSingleton();
        _audioSource = GetComponent<AudioSource>();
    }

    void ManageSingleton()
    {
        int numGameManagers = FindObjectsByType<MusicPlayer>(FindObjectsSortMode.None).Length;
        if (numGameManagers > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Pause()
    {
        _audioSource.Pause();
    }

    public void Play()
    {
        _audioSource.Play();
    }
}
