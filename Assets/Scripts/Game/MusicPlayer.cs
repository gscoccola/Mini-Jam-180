using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] float _fadeRate = 0.2f;
    [SerializeField] float _maxVolume2 = 0.2f;
    [SerializeField] float _maxVolume3 = 0.35f;
    public GamePhase _gamePhase;
    AudioSource _audioSource;
    AudioSource _audioSource2;
    AudioSource _audioSource3;
    AudioSource _audioSourceWind;
    [SerializeField] AudioResource _firstPhaseTrack;
    [SerializeField] AudioResource _firstPhaseComboTrack;
    [SerializeField] AudioResource _secondPhaseTrack;
    [SerializeField] AudioResource _secondPhaseComboTrack;
    [SerializeField] AudioResource _EpilogueTrack;
    public bool _isSecondTrackEnabled;
    public bool _beginFadeOutWind;
    public bool _isPlayingWind;
    public bool _isPlayingMusic;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        _audioSourceWind = transform.GetChild(2).GetComponent<AudioSource>();
        _audioSource3 = transform.GetChild(1).GetComponent<AudioSource>();

    }


    public void OnLevelLoad(int levelIndex)
    {
        if (new List<int> { 1, 2, 3, 4}.Contains(levelIndex)) _gamePhase = GamePhase.First;
        else if (new List<int> { 5}.Contains(levelIndex)) _gamePhase = GamePhase.Second;
        else if(new List<int> {SceneManager.sceneCountInBuildSettings - 1 }.Contains(levelIndex)) _gamePhase = GamePhase.Epilogue;

        _isSecondTrackEnabled = false;

        if (_gamePhase == GamePhase.First)
        {
            if (_audioSource is not null)
            {
                _audioSource.resource = _firstPhaseTrack;
                _audioSource2.resource = _firstPhaseComboTrack;
            }
            
        }
        if (_gamePhase == GamePhase.Second)
        {
            _audioSource.resource = _secondPhaseTrack;
            _audioSource2.resource = _secondPhaseComboTrack;
        }
        if (_gamePhase == GamePhase.Epilogue)
        {
            _audioSource.resource = _EpilogueTrack;
            _audioSource.volume = 0.6f;
        }
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
            if (_gamePhase == GamePhase.First)  _audioSource3.volume = Mathf.MoveTowards(_audioSource3.volume, _maxVolume3, _fadeRate * Time.deltaTime);
            return;
        }
        _audioSource2.volume = 0f;
        if (_gamePhase == GamePhase.First) _audioSource3.volume = 0f;

    }

    public void Pause()
    {
        _isPlayingMusic = false;
        _audioSource.Stop();
        _audioSource2.Stop();
        if (_gamePhase == GamePhase.First) _audioSource3.Stop();
    }

    public void Play()
    {
        _isPlayingMusic = true;
        _audioSource.Play();
        _audioSource2.Play();
        if (_gamePhase == GamePhase.First) _audioSource3.Play();
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
    Epilogue,
}
