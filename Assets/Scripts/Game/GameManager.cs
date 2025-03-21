using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //[SerializeField] InGameMenu _inGameMenu;
    [SerializeField] float _endDelay;
    [SerializeField] InGameMenu _loseMenu;
    [SerializeField] InGameMenu _winMenu;
    SFXPlayer _SFXPlayer;
    AudioPlayer _audioPlayer;

    float _musicStartDelay = 0.3f;
    

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) StartCoroutine(StartMusic());
    }


    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(_musicStartDelay);
        _audioPlayer = FindAnyObjectByType<AudioPlayer>();
        _audioPlayer.Play();
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
        StopAllCoroutines();
        StartCoroutine(ILose());
    }

    public void Win()
    {
        StopAllCoroutines();
        StartCoroutine(IWin());
    }

    IEnumerator ILose()
    {
        _audioPlayer.Pause();
        yield return new WaitForSeconds(_endDelay);
        _loseMenu.gameObject.SetActive(true);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._defeat);
    }

    IEnumerator IWin()
    {
        _audioPlayer.Pause();
        yield return new WaitForSeconds(_endDelay);
        _winMenu.gameObject.SetActive(true);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._victory);
        yield return new WaitForSeconds(3f);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._winDialog);
    }
}
