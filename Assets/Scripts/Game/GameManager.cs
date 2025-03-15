using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] InGameMenu _inGameMenu;

    private void Awake()
    {
        //ManageSingleton();
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
        if (levelIndex > 0) _inGameMenu = FindAnyObjectByType<InGameMenu>();
    }

    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(currentSceneIndex);
    }

    public void GameOver()
    {
        _inGameMenu.gameObject.SetActive(true);
    }
}
