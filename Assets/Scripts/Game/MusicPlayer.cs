using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] float _fadeRate = 0.2f;
    [SerializeField] float _maxVolume = 0.265f;
    [SerializeField] float _maxVolume2 = 0.2f;
    [SerializeField] float _maxVolume3 = 0.4f;
    public GamePhase _gamePhase;
    AudioSource _audioSource;
    AudioSource _audioSource2;
    AudioSource _audioSource3;
    AudioSource _audioSource4;
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

    [SerializeField] AudioResource _bossPhase1;
    [SerializeField] AudioResource _bossPhase2;
    [SerializeField] AudioResource _bossPhase3;
    [SerializeField] AudioResource _bossCombo;

    [SerializeField] AudioMixerGroup _musicMixer;
    [SerializeField] bool _isMusicEnabled = true;

    public bool _isSecondTrackEnabled;
    public bool _isThirdTrackEnabled;
    public bool _isFourthTrackEnabled;
    public bool _beginFadeOutWind;
    public bool _isPlayingWind;
    public bool _isPlayingMusic;
    public bool _firstTrackFadeIn;


    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        _audioSource3 = transform.GetChild(2).GetComponent<AudioSource>();
        _audioSource4 = transform.GetChild(3).GetComponent<AudioSource>();
        _audioSourceWind = transform.GetChild(1).GetComponent<AudioSource>();

    }


    public void OnLevelLoad(int levelIndex)
    {
        _isSecondTrackEnabled = false;
        GamePhase previousPhase = _gamePhase;
        if (new List<int> { 1, 2, 3}.Contains(levelIndex)) _gamePhase = GamePhase.First;
        else if (new List<int> { 4, 5}.Contains(levelIndex)) _gamePhase = GamePhase.Second;
        else if (new List<int> { 6}.Contains(levelIndex)) _gamePhase = GamePhase.Third;
        else if (new List<int> {7}.Contains(levelIndex)) _gamePhase = GamePhase.Boss;
        else if(new List<int> {8 }.Contains(levelIndex)) _gamePhase = GamePhase.Epilogue;
        else if(new List<int> {9 }.Contains(levelIndex)) _gamePhase = GamePhase.Endless;

        if (_gamePhase == GamePhase.Boss)
        {
            _isThirdTrackEnabled = false;
            _audioSource3.volume = 0f;
            _isFourthTrackEnabled = false;
            _audioSource4.volume = 0f;
        }

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
        else if (_gamePhase == GamePhase.Boss)
        {
            _audioSource.volume = 0f;
            _firstTrackFadeIn = true;
            _audioSource.resource = _bossPhase1;
            _audioSource2.resource = _bossCombo;
            _audioSource3.resource = _bossPhase2;
            _audioSource4.resource = _bossPhase3;
        }
        _isPlayingMusic = false;

        if (!_isMusicEnabled) DisableMusic();
    }

    public void DisableMusic()
    {
        _audioSource.volume = 0f;
        _audioSource2.volume = 0f;
        _audioSource3.volume = 0f;
        _audioSource4.volume = 0f;
        _maxVolume = 0f;
        _maxVolume2 = 0f;
        _maxVolume3 = 0f;
    }

    public void LowerVolume()
    {
        _audioSource.volume = _audioSource.volume / 4;
        _audioSource2.volume = _audioSource2.volume / 4;
        if (_gamePhase == GamePhase.Boss)
        {
            _audioSource3.volume = _audioSource3.volume / 4;
            _audioSource4.volume = _audioSource4.volume / 4;
        }
    }

    public void RaiseVolume()
    {
        _audioSource.volume = _audioSource.volume * 4;
        _audioSource2.volume = _audioSource2.volume * 4;
        if (_gamePhase == GamePhase.Boss)
        {
            _audioSource3.volume = _audioSource3.volume * 4;
            _audioSource4.volume = _audioSource4.volume * 4;
        }
    }

    private void Update()
    {
        if (_beginFadeOutWind)
        {
            _isPlayingWind = false;
            _audioSourceWind.volume = Mathf.MoveTowards(_audioSourceWind.volume, 0, _fadeRate * Time.deltaTime);
        }

        if (_firstTrackFadeIn)
        {
            _audioSource.volume = Mathf.MoveTowards(_audioSource.volume, _maxVolume, _fadeRate * Time.deltaTime);
            if (_audioSource.volume >= 0.264f)
            {
                _firstTrackFadeIn = false;
            }
        }

        if (_isThirdTrackEnabled)
        {
            _audioSource3.volume = Mathf.MoveTowards(_audioSource3.volume, _maxVolume3, _fadeRate * Time.deltaTime);
        }

        if (_isFourthTrackEnabled)
        {
            _audioSource4.volume = Mathf.MoveTowards(_audioSource4.volume, _maxVolume3, _fadeRate * Time.deltaTime);
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
        if (_gamePhase == GamePhase.Boss)
        {
            _audioSource3.Stop();
            _audioSource4.Stop();
        }
        
    }

    public void Play()
    {
        
        _isPlayingMusic = true;
        if (!_isMusicEnabled) return;
        _audioSource.Play();
        _audioSource2.Play();
        if (_gamePhase == GamePhase.Boss)
        {
            _audioSource3.Play();
            _audioSource4.Play();
        }   
    }

    public void PlayWind()
    {
        _beginFadeOutWind = false;
        _audioSourceWind.volume = 0.344f;
        _isPlayingWind = true;
    }

    public void EnableLowPass()
    {
        _musicMixer.audioMixer.SetFloat("CutOffFreq", 6000f);
    }

    public void DisableLowPass()
    {
        _musicMixer.audioMixer.SetFloat("CutOffFreq", 22000f);
    }

}

public enum GamePhase
{
    First,
    Second,
    Third,
    Epilogue,
    Endless,
    Boss,
}
