using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int _epilogueLevelIndex = 6;
    [SerializeField] float _endDelay;
    [SerializeField] InGameMenu _loseMenu;
    [SerializeField] InGameMenu _winMenu;
    [SerializeField] InGameMenu _pauseMenu;
    [SerializeField] GameButton _pauseButton;
    [SerializeField] InGameMenu _mobileInputButtons;
    [SerializeField] InGameMenu _creditsMenu;
    [SerializeField] InGameMenu _achievementsMenu;
    [SerializeField] InGameMenu _startMenu;
    [SerializeField] InGameMenu _tutorialMenu;
    [SerializeField] InGameMenu _levelSelectMenu;
    [SerializeField] GameObject _confetti;

    CameraScript _cameraScript;
    Camera _camera;

    LevelLoader _levelLoader;
    Spawner _spawner;
    bool _isPlayingOnMobile;
    int activeSceneIndex;
    bool _isPaused;



    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _levelLoader = FindAnyObjectByType<LevelLoader>();
        _spawner = FindAnyObjectByType<Spawner>();
        _cameraScript = FindAnyObjectByType<CameraScript>();

        _isPlayingOnMobile = Application.isMobilePlatform;
        activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        
    }

    private void Start()
    {
        PersistentInfo.Instance.SaveData();
        if (activeSceneIndex == 0 && _achievementsMenu is not null) _achievementsMenu.gameObject.SetActive(false);
        SFXPlayer.Instance.OnLevelLoad(activeSceneIndex);
        MusicPlayer.Instance.OnLevelLoad(activeSceneIndex);


        if (activeSceneIndex==0) ActivateProperTutorial();

        if (activeSceneIndex != 0 && _isPlayingOnMobile)
            _mobileInputButtons.gameObject.SetActive(true);

        if (activeSceneIndex == 0 || activeSceneIndex == _epilogueLevelIndex)
        {
            if (!MusicPlayer.Instance._isPlayingWind) MusicPlayer.Instance.PlayWind();
            MusicPlayer.Instance.Pause();
            return;
        }

        else if (!PersistentInfo.Instance.LevelCompletionInfo[activeSceneIndex].WasShown)
        {
            PersistentInfo.Instance.LevelCompletionInfo[activeSceneIndex].WasShown = true;
            if (_cameraScript != null) StartCoroutine(_cameraScript.ICameraPan());
            if (!MusicPlayer.Instance._isPlayingWind) MusicPlayer.Instance.PlayWind();
            MusicPlayer.Instance.Pause();
        }
        else
        {
            if (!MusicPlayer.Instance._isPlayingMusic) MusicPlayer.Instance.Play();
            MusicPlayer.Instance._beginFadeOutWind = true;
            if (_cameraScript != null) _cameraScript.SwitchToPlayerCam();
            StartLevel();
        }
    }

    public void AlmostStartLevel()
    {
        MusicPlayer.Instance._beginFadeOutWind = true;
    }

    public void Pause()
    {
        if (_isPaused)
        {
            _isPaused = false;
            Time.timeScale = 1f;
            _pauseMenu.gameObject.SetActive(false);
            if (_isPlayingOnMobile) _mobileInputButtons.gameObject.SetActive(true);
            return;
        }
        _isPaused = true;
        Time.timeScale = 0f;
        _pauseMenu.gameObject.SetActive(true);
        if (_isPlayingOnMobile) _mobileInputButtons.gameObject.SetActive(false);
    }

    public void StartLevel()
    {
        if(!MusicPlayer.Instance._isPlayingMusic) MusicPlayer.Instance.Play();
        if (_spawner != null) _spawner.StartSpawning();
        FindAnyObjectByType<PlayerMovement>().IsHorizonalMovementDisabled = false;
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
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
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
        OnAchievementUnlocked(3);
        StopAllCoroutines();
        StartCoroutine(ILose());
    }

    public void Win()
    {
        _mobileInputButtons.gameObject.SetActive(false);
        PersistentInfo.Instance.LevelCompletionInfo[activeSceneIndex].WasBeaten = true;
        if (activeSceneIndex == 1) OnAchievementUnlocked(1);
        StopAllCoroutines();
        StartCoroutine(IWin());
    }

    IEnumerator ILose()
    {
        _pauseButton.gameObject.SetActive(false);
        MusicPlayer.Instance.Pause();
        yield return new WaitForSeconds(_endDelay);
        _loseMenu.gameObject.SetActive(true);
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._defeat);
    }

    IEnumerator IWin()
    {
        _pauseButton.gameObject.SetActive(false);
        MusicPlayer.Instance.Pause();
        yield return new WaitForSeconds(_endDelay);
        _winMenu.gameObject.SetActive(true);
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._victory);
        yield return new WaitForSeconds(1.2f);
        Instantiate(_confetti, new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f), Quaternion.identity);
        yield return new WaitForSeconds(1.8f);
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._winDialog);
    }

    internal void ToggleCredits()
    {
        _creditsMenu.gameObject.SetActive(!_creditsMenu.gameObject.activeSelf);
        _startMenu.gameObject.SetActive(!_startMenu.gameObject.activeSelf);
    }
    
    internal void ToggleAchievements()
    {
        _achievementsMenu.gameObject.SetActive(!_achievementsMenu.gameObject.activeSelf);
        _startMenu.gameObject.SetActive(!_startMenu.gameObject.activeSelf);
    }

    internal void TutorialOrLevelsMenu()
    {
        if (PersistentInfo.Instance.LevelCompletionInfo[1].WasShown)
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

        else if (_tutorialMenu.gameObject.activeSelf)
        {
            LoadLevel(1);
            return;
        }

        _tutorialMenu.gameObject.SetActive(true);
        _tutorialMenu.transform.GetChild(2).gameObject.SetActive(true);
        _tutorialMenu.transform.GetChild(3).gameObject.SetActive(false);
        _startMenu.gameObject.SetActive(false);
    }

    private void ActivateProperTutorial()
    {
        if (!_isPlayingOnMobile) _tutorialMenu.transform.GetChild(0).gameObject.SetActive(true);
        else _tutorialMenu.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ExitLevel()
    {
        if (activeSceneIndex != 0)
        LoadLevel(0);
    }

    public void ShowTutorial()
    {
        _tutorialMenu.gameObject.SetActive(!_tutorialMenu.gameObject.activeSelf);
        _levelSelectMenu.gameObject.SetActive(!_levelSelectMenu.gameObject.activeSelf);
    }

    public void OnAchievementUnlocked(int achievementIndex)
    {
        if (PersistentInfo.Instance.AchievementTitles.ContainsKey(achievementIndex) == false)
        {
            Debug.LogError($"Achievement with index {achievementIndex} does not exist.");
            return;
        }
        if (PersistentInfo.Instance.AchievementCompletionInfo[achievementIndex].WasUnlocked) return;
        PersistentCanvas.Instance.ShowPopup(PersistentInfo.Instance.AchievementTitles[achievementIndex]);
        PersistentInfo.Instance.AchievementCompletionInfo[achievementIndex].WasUnlocked = true;
    }
}
