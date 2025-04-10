using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //[SerializeField] InGameMenu _inGameMenu;
    [SerializeField] float _endDelay;
    [SerializeField] InGameMenu _loseMenu;
    [SerializeField] InGameMenu _winMenu;
    [SerializeField] InGameMenu _mobileInputButtons;
    [SerializeField] InGameMenu _creditsMenu;
    [SerializeField] InGameMenu _startMenu;
    [SerializeField] InGameMenu _tutorialMenu;
    [SerializeField] InGameMenu _levelSelectMenu;
    [SerializeField] GameObject _confetti;

    CameraScript _cameraScript;
    Camera _camera;
    SFXPlayer _SFXPlayer;
    MusicPlayer _musicPlayer;
    LevelLoader _levelLoader;
    Spawner _spawner;
    bool _isPlayingOnMobile;
    int activeSceneIndex;



    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _musicPlayer = FindAnyObjectByType<MusicPlayer>();
        _levelLoader = FindAnyObjectByType<LevelLoader>();
        _spawner = FindAnyObjectByType<Spawner>();
        _cameraScript = FindAnyObjectByType<CameraScript>();
    }

    private void Start()
    {
        activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (activeSceneIndex == 0 || activeSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            _musicPlayer.PlayWind();
            return;
        }


        if (!PersistentInfo.Instance.LevelsInfo[activeSceneIndex].HasBeenShown)
        {
            PersistentInfo.Instance.LevelsInfo[activeSceneIndex].HasBeenShown = true;
            StartCoroutine(_cameraScript.ICameraPan());
            _musicPlayer.PlayWind();
        }
        else
        {
            _cameraScript.SwitchToPlayerCam();
            StartLevel();
        }

        _isPlayingOnMobile = Application.isMobilePlatform;
        if (activeSceneIndex != 0 && _isPlayingOnMobile)
            _mobileInputButtons.gameObject.SetActive(true);

    }
    public void AlmostStartLevel()
    {
        _musicPlayer._beginFadeOutWind = true;
    }

    public void StartLevel()
    {
        _musicPlayer.Play();
        _spawner.StartSpawning();
        FindAnyObjectByType<PlayerMovement>().IsHorizonalMovementDisabled = false;
    }

    public void DisableMobileInput()
    {
        _mobileInputButtons.gameObject.SetActive(false);
    }

    public void LoadLevel(int levelIndex)
    {
        _levelLoader.LoadLevel(levelIndex);
    }

    public void NextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LoadLevel(currentSceneIndex);
    }

    public void Lose()
    {
        _mobileInputButtons.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(ILose());
    }

    public void OutOfBounds()
    {
        _mobileInputButtons.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(ILose());
    }

    public void Win()
    {
        PersistentInfo.Instance.LevelsInfo[activeSceneIndex].HasBeenBeaten = true;
        _mobileInputButtons.gameObject.SetActive(false);
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

    internal void TutorialOrLevelsMenu()
    {
        if (PersistentInfo.Instance.LevelsInfo[1].HasBeenShown)
        { 
            if (!_levelSelectMenu.gameObject.activeSelf)
            {
                _levelSelectMenu.gameObject.SetActive(true);
                _startMenu.gameObject.SetActive(false);
                return;
            }
            _levelSelectMenu.gameObject.SetActive(false);
            _startMenu.gameObject.SetActive(true);
            return;
        }

        else if (_tutorialMenu.gameObject.activeSelf || _isPlayingOnMobile)
        {
            LoadLevel(1);
            return;
        }

        _tutorialMenu.gameObject.SetActive(true);
        _startMenu.gameObject.SetActive(false);
    }

    public void ExitLevel()
    {
        if (activeSceneIndex != 0)
        LoadLevel(0);
    }
}
