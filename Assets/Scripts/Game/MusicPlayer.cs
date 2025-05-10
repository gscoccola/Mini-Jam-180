using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] float _fadeRate = 0.2f;
    [SerializeField] float _maxVolume2 = 0.2f;
    public GamePhase _gamePhase;
    AudioSource _audioSource;
    AudioSource _audioSource2;
    AudioSource _audioSourceWind;
    [SerializeField] AudioResource _firstPhaseTrack;
    [SerializeField] AudioResource _firstPhaseComboTrack;
    [SerializeField] AudioResource _secondPhaseTrack;
    [SerializeField] AudioResource _secondPhaseComboTrack;
    [SerializeField] AudioResource _thirdPhaseTrack;
    [SerializeField] AudioResource _thirdPhaseComboTrack;
    [SerializeField] AudioResource _EpilogueTrack;
    [SerializeField] AudioResource _endlessTrack;
    [SerializeField] AudioResource _endlessComboTrack;
    public bool _isSecondTrackEnabled;
    public bool _beginFadeOutWind;
    public bool _isPlayingWind;
    public bool _isPlayingMusic;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        _audioSourceWind = transform.GetChild(1).GetComponent<AudioSource>();

    }


    public void OnLevelLoad(int levelIndex)
    {
        _isSecondTrackEnabled = false;
        GamePhase previousPhase = _gamePhase;
        if (new List<int> { 1, 2, 3}.Contains(levelIndex)) _gamePhase = GamePhase.First;
        else if (new List<int> { 4, 5}.Contains(levelIndex)) _gamePhase = GamePhase.Second;
        else if (new List<int> { 6}.Contains(levelIndex)) _gamePhase = GamePhase.Third;
        else if(new List<int> {7 }.Contains(levelIndex)) _gamePhase = GamePhase.Epilogue;
        else if(new List<int> {8 }.Contains(levelIndex)) _gamePhase = GamePhase.Endless;

        if (previousPhase == _gamePhase) return;

        if (_gamePhase == GamePhase.First)
        {
            _audioSource.volume = 0.265f;
            _audioSource.resource = _firstPhaseTrack;
            _audioSource2.resource = _firstPhaseComboTrack;
        }
        else if (_gamePhase == GamePhase.Second)
        {
            _audioSource.volume = 0.265f;
            _audioSource.resource = _secondPhaseTrack;
            _audioSource2.resource = _secondPhaseComboTrack;
        }
        else if (_gamePhase == GamePhase.Third)
        {
            _audioSource.volume = 0.265f;
            _audioSource.resource = _thirdPhaseTrack;
            _audioSource2.resource = _thirdPhaseComboTrack;
        }

        else if(_gamePhase == GamePhase.Epilogue)
        {
            _audioSource.resource = _EpilogueTrack;
            _audioSource.volume = 0.6f;
        }

        else if (_gamePhase == GamePhase.Endless)
        {
            _audioSource.volume = 0.265f;
            _audioSource.resource = _endlessTrack;
            _audioSource2.resource = _endlessComboTrack;
        }
        _isPlayingMusic = false;
    }

    public void LowerVolume()
    {
        _audioSource.volume = _audioSource.volume / 4;
        _audioSource2.volume = _audioSource2.volume / 4;
    }

    public void RaiseVolume()
    {
        _audioSource.volume = _audioSource.volume * 4;
        _audioSource2.volume = _audioSource2.volume * 4;
    }

    private void Update()
    {
        if (_beginFadeOutWind)
        {
            _isPlayingWind = false;
            _audioSourceWind.volume = Mathf.MoveTowards(_audioSourceWind.volume, 0, _fadeRate * Time.deltaTime);
        }

        if (_isSecondTrackEnabled)
        {
            _audioSource2.volume = Mathf.MoveTowards(_audioSource2.volume, _maxVolume2, _fadeRate * Time.deltaTime);
            return;
        }
        _audioSource2.volume = 0f;

    }

    public void Pause()
    {
        _isPlayingMusic = false;
        _audioSource.Stop();
        _audioSource2.Stop();
    }

    public void Play()
    {
        _isPlayingMusic = true;
        _audioSource.Play();
        _audioSource2.Play();
    }

    public void PlayWind()
    {
        _beginFadeOutWind = false;
        _audioSourceWind.volume = 0.344f;
        _isPlayingWind = true;
    }

}

public enum GamePhase
{
    First,
    Second,
    Third,
    Epilogue,
    Endless,
}
