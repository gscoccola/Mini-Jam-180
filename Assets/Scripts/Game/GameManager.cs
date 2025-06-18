using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float _endDelay;
    [SerializeField] InGameMenu _loseMenu;
    [SerializeField] InGameMenu _winMenu;
    [SerializeField] InGameMenu _pauseMenu;
    [SerializeField] GameButton _pauseButton;
    [SerializeField] InGameMenu _mobileInputButtons;
    [SerializeField] InGameMenu _creditsMenu;
    [SerializeField] InGameMenu _achievementsMenu;
    [SerializeField] InGameMenu _achievementsMenuInGame;
    [SerializeField] InGameMenu _startMenu;
    [SerializeField] InGameMenu _languageMenu;
    [SerializeField] InGameMenu _tutorialMenu;
    [SerializeField] InGameMenu _levelSelectMenu;
    [SerializeField] InGameMenu _deleteDataMenu;
    [SerializeField] InGameMenu _postCreditButtons;
    [SerializeField] GameObject _confetti;
    [SerializeField] GameObject _argFlag;
    [SerializeField] GameObject _usaFlag;
    [SerializeField] GameObject _loadingTextEsp;
    [SerializeField] GameObject _loadingTextEng;
    [SerializeField] HighscoreSubmitter _highscoreSumbitter;
    [SerializeField] InGameMenu _loadingScreen;
    [SerializeField] LeaderboardDisplay _leaderboardMenu;
    [SerializeField] InGameMenu _noNetworkMenu;

    CameraPan _cameraScript;
    Camera _camera;

    LevelLoader _levelLoader;
    Spawner _spawner;
    MissileSpawner _mSpawner;
    TargetIndicator _indicator;
    ScoreKeeper _scoreKeeper;

    bool _isPlayingOnMobile;
    int _activeSceneIndex;
    bool _isPaused;
    bool _wasLevelJustWon;

    InGameMenu _lastMenu;



    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _levelLoader = FindAnyObjectByType<LevelLoader>();
        _spawner = FindAnyObjectByType<Spawner>();
        _mSpawner = FindAnyObjectByType<MissileSpawner>();
        _cameraScript = FindAnyObjectByType<CameraPan>();
        _indicator = FindAnyObjectByType<TargetIndicator>();
        _isPlayingOnMobile = Application.isMobilePlatform;
        _activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _scoreKeeper = FindAnyObjectByType<ScoreKeeper>();
    }

    private void Start()
    {
        if (_activeSceneIndex == 0 && !PersistentInfo.Instance._wasLanguageSet) SelectDefaultButton(_argFlag);
        if (_activeSceneIndex == 0 && PersistentInfo.Instance._wasLanguageSet)
        {
            _languageMenu.gameObject.SetActive(false);
            _startMenu.gameObject.SetActive(true);
            SelectDefaultButton(_startMenu.transform.GetChild(0).gameObject);
        }


        PersistentInfo.Instance.SaveCompletionData();
        MusicPlayer.Instance.OnLevelLoad(_activeSceneIndex);
        SFXPlayer.Instance.OnLevelLoad(_activeSceneIndex);

        if (_activeSceneIndex==0) ActivateProperTutorial();

        if (_activeSceneIndex != 0 && _isPlayingOnMobile)
            _mobileInputButtons.gameObject.SetActive(true);

        if (_activeSceneIndex == 0 || FindAnyObjectByType<EpilogueManager>() != null)
        {
            if (!MusicPlayer.Instance._isPlayingWind) MusicPlayer.Instance.PlayWind();
            MusicPlayer.Instance.Pause();
            return;
        }

        else if (!PersistentInfo.Instance.LevelCompletionInfo[_activeSceneIndex].WasShown &&
            FindAnyObjectByType<BossBehavior>() == null)
        {
            PersistentInfo.Instance.LevelCompletionInfo[_activeSceneIndex].WasShown = true;
            if (_cameraScript != null) StartCoroutine(_cameraScript.ICameraPan());
            if (!MusicPlayer.Instance._isPlayingWind) MusicPlayer.Instance.PlayWind();
            MusicPlayer.Instance.Pause();
        }
        else
        {
            if (!MusicPlayer.Instance._isPlayingMusic) MusicPlayer.Instance.Play();
            MusicPlayer.Instance._beginFadeOutWind = true;
            if (_cameraScript != null) _cameraScript.SwitchToPlayerCam();
            StartLevel(true);
        }
    }

    public void AlmostStartLevel()
    {
        MusicPlayer.Instance._beginFadeOutWind = true;
    }

    public void PauseKeyPress()
    {
        if (_pauseButton.gameObject.activeSelf) _pauseButton.OnClick();

    }
    public void Pause()
    {
        if (_achievementsMenuInGame.gameObject.activeSelf)
        {
            _pauseButton.gameObject.SetActive(true);
            _achievementsMenuInGame.gameObject.SetActive(false);
        }
    
        if (_isPaused)
        {
            _isPaused = false;
            Time.timeScale = 1f;
            _pauseMenu.gameObject.SetActive(false);
            if (_isPlayingOnMobile) _mobileInputButtons.gameObject.SetActive(true);
            SelectDefaultButton(null);
            return;
        }
        _isPaused = true;
        Time.timeScale = 0f;
        _pauseMenu.gameObject.SetActive(true);
        SelectDefaultButton(_pauseMenu.transform.GetChild(0).gameObject);

        if (_isPlayingOnMobile) _mobileInputButtons.gameObject.SetActive(false);
    }

    public void StartLevel(bool preSpawn)
    {
        if(!MusicPlayer.Instance._isPlayingMusic) MusicPlayer.Instance.Play();
        if (_spawner != null) _spawner.StartSpawning(preSpawn); 

        FindAnyObjectByType<PlayerMovement>().IsHorizonalMovementDisabled = false;
    }


    public void LoadLevel(int levelIndex)
    {
        _levelLoader.LoadLevel(levelIndex);
    }

    public void NextLevel()
    {
        PersistentInfo.Instance.LevelCompletionInfo[_activeSceneIndex].WasBeaten = true;
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        if (!_wasLevelJustWon) LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void Lose()
    {        
        StopAllCoroutines();
        StartCoroutine(ILevelEnd(false));
    }

    public void OutOfBounds()
    {
        OnAchievementUnlocked(3);
        StopAllCoroutines();
        StartCoroutine(ILevelEnd(false));
        BossBehavior boss = FindAnyObjectByType<BossBehavior>();
        if (boss != null) boss.OnPlayerDeath();
    }

    public void Win()
    {
        PersistentInfo.Instance.LevelCompletionInfo[_activeSceneIndex].WasBeaten = true;
        _wasLevelJustWon = true;
        if (_activeSceneIndex == 1) OnAchievementUnlocked(1);
        StopAllCoroutines();
        StartCoroutine(ILevelEnd(true));
    }


    IEnumerator ILevelEnd(bool hasWon)
    {
        _mobileInputButtons.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(false);

        yield return new WaitForEndOfFrame();

        if (_indicator != null) _indicator.gameObject.SetActive(false);
        if (_mSpawner != null) _mSpawner.StopSpawning();

        yield return new WaitForEndOfFrame();

        bool newHighscore = false;
        if (_scoreKeeper != null) newHighscore = AssessHighscore();
            
        yield return new WaitForEndOfFrame();

        PersistentInfo.Instance.SaveCompletionData();

        yield return new WaitForEndOfFrame();

        PlayfabManager.Instance.SendCompletionStatistics();
        MusicPlayer.Instance.Pause();

        yield return new WaitForSeconds(_endDelay);
        VibrationHandler.Instance.HeavyVibrate();
        if (hasWon)
        {
            _winMenu.gameObject.SetActive(true);
            SelectDefaultButton(_winMenu.transform.GetChild(1).gameObject);
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._victory);
            yield return new WaitForSeconds(1.2f);
            Instantiate(_confetti, new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f), Quaternion.identity);
            yield return new WaitForSeconds(1.8f);
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._winDialog);
        }
        else if (newHighscore)
        {
            _highscoreSumbitter.gameObject.SetActive(true);
            _highscoreSumbitter.SetName(PersistentInfo.Instance.DisplayName);
            _highscoreSumbitter.SetScore(PersistentInfo.Instance.LastScore);
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._victory);
            SelectDefaultButton(_highscoreSumbitter.transform.GetChild(0).GetChild(0).gameObject);
        }
        else
        {
            _loseMenu.gameObject.SetActive(true);
            SelectDefaultButton(_loseMenu.transform.GetChild(1).gameObject);
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._defeat);
        }
    }

    private bool AssessHighscore()
    {
        int score = _scoreKeeper.GetScore();
        _scoreKeeper.gameObject.SetActive(false);
        PlayfabManager.Instance.SendHighscore(score);
        PersistentInfo.Instance.LastScore = score;

        if (score > PersistentInfo.Instance.GlobalHighScore)
        {
            PersistentInfo.Instance.GlobalHighScore = score;
            PersistentInfo.Instance.PersonalHighScore = score;
            PersistentInfo.Instance.SaveHighScoreData();
        }
        else if (score > PersistentInfo.Instance.PersonalHighScore)
        {
            PersistentInfo.Instance.PersonalHighScore = score;
            PersistentInfo.Instance.SaveHighScoreData();
        }
        else return false;
        return true;
        //_scoreKeeper.Set(false); disable scorekeeper counting
        
    }

    public void SubmitHighScore()
    {
        string name = _highscoreSumbitter.GetName();
        if (name != PersistentInfo.Instance.DisplayName)
        {
            PersistentInfo.Instance.DisplayName = name;
            PlayfabManager.Instance.UpdateDisplayName(name);
            PersistentInfo.Instance.SaveDisplayName();
        }
        _highscoreSumbitter.gameObject.SetActive(false);
        _loseMenu.gameObject.SetActive(true);
        SelectDefaultButton(_loseMenu.transform.GetChild(1).gameObject);
    }

    public void CloseLeaderboard()
    {
        if (_noNetworkMenu.gameObject.activeSelf)
        {
            _noNetworkMenu.gameObject.SetActive(false);
        }
        else
        {
            string name = _leaderboardMenu.GetName();
            if (name != PersistentInfo.Instance.DisplayName)
            {
                PersistentInfo.Instance.DisplayName = name;
                PlayfabManager.Instance.UpdateDisplayName(name);
                PersistentInfo.Instance.SaveDisplayName();
            }
            _leaderboardMenu.gameObject.SetActive(false);
        }

        if (_activeSceneIndex == 0)
        {
            _levelSelectMenu.gameObject.SetActive(true);
            SelectDefaultButton(_levelSelectMenu.transform.GetChild(0).GetChild(0).gameObject);
        }
        else
        {
            _loseMenu.gameObject.SetActive(true);
            SelectDefaultButton(_loseMenu.transform.GetChild(1).gameObject);
        }
        LeaderboardManager.Instance.HighScores = null;
    }

    public void StartLoadingLeaderboard()
    {
        if (PlayfabManager.Instance._nameChangeBeingProcessed)
        {
            PlayfabManager.Instance._openLeaderBoardAfterNameChange = true;
            return;
        }

        PlayfabManager.Instance.GetGlobalStatistics(true, 100);

        if (_activeSceneIndex == 0)
        {
            _levelSelectMenu.gameObject.SetActive(false);
            _loadingScreen.gameObject.SetActive(true);
        }
        else
        {
            _loseMenu.gameObject.SetActive(false);
            _loadingScreen.gameObject.SetActive(true);
        }  
    }

    public void OpenLeaderBoard()
    {
        _loadingScreen.gameObject.SetActive(false);
        _leaderboardMenu.gameObject.SetActive(true);
        SelectDefaultButton(_leaderboardMenu.transform.GetChild(0).GetChild(4).gameObject);
    }

    public void CancelOpenLeaderBoard()
    {
        
        _loadingScreen.gameObject.SetActive(false);
        _noNetworkMenu.gameObject.SetActive(true);
        SelectDefaultButton(_noNetworkMenu.transform.GetChild(0).gameObject);
    }

    internal void ToggleCredits()
    {
        _creditsMenu.gameObject.SetActive(!_creditsMenu.gameObject.activeSelf);
        _startMenu.gameObject.SetActive(!_startMenu.gameObject.activeSelf);
        if (_creditsMenu.gameObject.activeSelf) SelectDefaultButton(_creditsMenu.transform.GetChild(0).gameObject);
        else SelectDefaultButton(_startMenu.transform.GetChild(0).gameObject);
    }
    
    internal void ToggleAchievements()
    {
        if (_deleteDataMenu.gameObject.activeSelf)
        {
            _deleteDataMenu.gameObject.SetActive(false);
            _achievementsMenu.gameObject.SetActive(true);
            SelectDefaultButton(_achievementsMenu.transform.GetChild(0).gameObject);
            return;
        }
        _achievementsMenu.gameObject.SetActive(!_achievementsMenu.gameObject.activeSelf);
        _startMenu.gameObject.SetActive(!_startMenu.gameObject.activeSelf);
        if (_achievementsMenu.gameObject.activeSelf) SelectDefaultButton(_achievementsMenu.transform.GetChild(0).gameObject);
        else SelectDefaultButton(_startMenu.transform.GetChild(0).gameObject);
    }

    internal void DeleteData()
    {
        if (!_deleteDataMenu.gameObject.activeSelf)
        {
            _deleteDataMenu.gameObject.SetActive(true);
            _achievementsMenu.gameObject.SetActive(false);
            SelectDefaultButton(_deleteDataMenu.transform.GetChild(0).gameObject);
            return;
        }
        _achievementsMenu.gameObject.SetActive(true);
        PersistentInfo.Instance.ResetData();
        _achievementsMenu.gameObject.SetActive(false);
        LoadLevel(0);
    }

    internal void TutorialOrLevelsMenu()
    {
        if (PersistentInfo.Instance.LevelCompletionInfo[1].WasShown)
        {
            if (!_levelSelectMenu.gameObject.activeSelf)
            {
                _levelSelectMenu.gameObject.SetActive(true);
                _startMenu.gameObject.SetActive(false);
                SelectDefaultButton(_levelSelectMenu.transform.GetChild(0).GetChild(0).gameObject);
                return;
            }
            _levelSelectMenu.gameObject.SetActive(false);
            _startMenu.gameObject.SetActive(true);
            SelectDefaultButton(_startMenu.transform.GetChild(0).gameObject);
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
        SelectDefaultButton(_tutorialMenu.transform.GetChild(2).gameObject);
        _startMenu.gameObject.SetActive(false);
    }

    private void ActivateProperTutorial()
    {
        if (_isPlayingOnMobile) _tutorialMenu.transform.GetChild(1).gameObject.SetActive(true);
        else if (Gamepad.current != null) _tutorialMenu.transform.GetChild(4).gameObject.SetActive(true);
        else _tutorialMenu.transform.GetChild(0).gameObject.SetActive(true); 
    }

    public void ExitLevel()
    {
        if (_activeSceneIndex != 0)
        LoadLevel(0);
    }

    public void ShowTutorial()
    {
        _tutorialMenu.gameObject.SetActive(!_tutorialMenu.gameObject.activeSelf);
        _levelSelectMenu.gameObject.SetActive(!_levelSelectMenu.gameObject.activeSelf);
        if (_tutorialMenu.gameObject.activeSelf) SelectDefaultButton(_tutorialMenu.transform.GetChild(3).gameObject);
        else SelectDefaultButton(_levelSelectMenu.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);
    }

    public void OnAchievementUnlocked(int achievementIndex)
    {
        if (!PersistentInfo.Instance.AchievementCompletionInfo.ContainsKey(achievementIndex))
        {
            Debug.LogError($"Achievement with index {achievementIndex} does not exist.");
            return;
        }
        if (PersistentInfo.Instance.AchievementCompletionInfo[achievementIndex].WasUnlocked) return;
        PersistentCanvas.Instance.ShowPopup();

        PersistentInfo.Instance.AchievementCompletionInfo[achievementIndex].WasUnlocked = true;
        PersistentInfo.Instance._isThereNewAchievement = true;
    }

    public void SetLocale(string locale)
    {
        
        PersistentInfo.Instance._wasLanguageSet = true;
        _argFlag.SetActive(false);
        _usaFlag.SetActive(false);
        if (locale == "es") _loadingTextEsp.gameObject.SetActive(true);
        else if (locale == "en") _loadingTextEng.gameObject.SetActive(true);

        StartCoroutine(ISetLocale(locale));
    }

    public IEnumerator ISetLocale(string locale)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(x => x.Identifier.Code == locale);

        PersistentInfo.Instance.LanguageCode = locale;
        LoadLevel(0);
    }

    public void AchievementMenuInGame()
    {
        if (_winMenu != null &&_winMenu.gameObject.activeSelf) _lastMenu = _winMenu;
        else if (_loseMenu != null && _loseMenu.gameObject.activeSelf) _lastMenu = _loseMenu;
        else if (_pauseMenu.gameObject.activeSelf) _lastMenu = _pauseMenu;
        else if (_creditsMenu != null && _creditsMenu.gameObject.activeSelf) _lastMenu = _creditsMenu;
       
        MusicPlayer.Instance.LowerVolume();
        _achievementsMenuInGame.gameObject.SetActive(true);
        _lastMenu.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(false);
        SelectDefaultButton(_achievementsMenuInGame.transform.GetChild(0).gameObject);
    }

    public void BackFromAchievementMenuInGame()
    {
        _achievementsMenuInGame.gameObject.SetActive(false);
        _lastMenu.gameObject.SetActive(true);
        SelectDefaultButton(_lastMenu.transform.GetChild(1).gameObject);
        MusicPlayer.Instance.RaiseVolume();
        if (_lastMenu == _pauseMenu)
        {
            _pauseButton.gameObject.SetActive(true);
            _pauseButton.SetPressed();
            SelectDefaultButton(_pauseMenu.transform.GetChild(0).gameObject);
        }
        if (_lastMenu == _creditsMenu)
        {
            SelectDefaultButton(_creditsMenu.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject);
        }

        if (_lastMenu == _winMenu)
        {
            _winMenu.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Idle");
        }
    }

   public void SelectDefaultButton(GameObject gameObject)
    {
        if (_isPlayingOnMobile) EventSystem.current.SetSelectedGameObject(null);
        else EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void UnlockEverything()
    {
        PersistentInfo.Instance.UnlockAllLevelsAndAchievements();
        ReloadLevel();
    }

    
}
