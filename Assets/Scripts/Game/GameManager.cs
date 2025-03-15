using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //[SerializeField] InGameMenu _inGameMenu;
    [SerializeField] InGameMenu _loseMenu;
    [SerializeField] InGameMenu _winMenu;

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
    }

    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(currentSceneIndex);
    }

    public void Lose()
    {
        _loseMenu.gameObject.SetActive(true);
    }

    public void Win()
    {
        _winMenu.gameObject.SetActive(true);
    }
}
