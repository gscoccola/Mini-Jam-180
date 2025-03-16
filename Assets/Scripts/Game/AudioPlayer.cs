using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer : MonoBehaviour
{
    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        int numGameManagers = FindObjectsByType<AudioPlayer>(FindObjectsSortMode.None).Length;
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
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            Destroy(this.gameObject);
        }
    }
}
