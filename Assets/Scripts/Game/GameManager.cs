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
    [SerializeField] InGameMenu _creditsMenu;
    [SerializeField] InGameMenu _startMenu;
    [SerializeField] GameObject _confetti;
    Camera _camera;
    SFXPlayer _SFXPlayer;
    MusicPlayer _musicPlayer;

    float _musicStartDelay = 0.3f;
    

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _musicPlayer = FindAnyObjectByType<MusicPlayer>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) StartCoroutine(StartMusic());
    }


    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(_musicStartDelay);
        
        _musicPlayer.Play();
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
        _musicPlayer.Pause();
        yield return new WaitForSeconds(_endDelay);
        _loseMenu.gameObject.SetActive(true);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._defeat);
    }

    IEnumerator IWin()
    {
        _musicPlayer.Pause();
        yield return new WaitForSeconds(_endDelay);
        _winMenu.gameObject.SetActive(true);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._victory);
        yield return new WaitForSeconds(1.2f);
        Instantiate(_confetti, new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f), Quaternion.identity);
        yield return new WaitForSeconds(1.8f);
        _SFXPlayer.PlayClipFromArray(_SFXPlayer._winDialog);
    }

    internal void ToggleCredits()
    {
        _creditsMenu.gameObject.SetActive(!_creditsMenu.gameObject.activeSelf);
        _startMenu.gameObject.SetActive(!_startMenu.gameObject.activeSelf);
    }
}
