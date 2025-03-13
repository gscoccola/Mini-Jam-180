using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private void Awake()
    {
        ManageSingleton();
    }

    void ManageSingleton()
    {
        int numGameManagers = FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length;
        if (numGameManagers > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ReloadLevel(int levelIndex)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(currentSceneIndex);
    }
}
