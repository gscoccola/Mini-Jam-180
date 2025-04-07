using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] float _fadeRate = 0.2f;
    [SerializeField] float _maxVolume2 = 0.2f;
    [SerializeField] float _maxVolume3 = 0.35f;
    public GamePhase _gamePhase;
    AudioSource _audioSource;
    AudioSource _audioSource2;
    AudioSource _audioSource3;
    AudioSource _audioSourceWind;
    [SerializeField] AudioResource _secondPhaseTrack;
    [SerializeField] AudioResource _secondPhaseComboTrack;
    [SerializeField] AudioResource _EpilogueTrack;
    public bool _isSecondTrackEnabled;
    public bool _beginFadeOutWind;

    private void Awake()
    {
        
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        _audioSourceWind = transform.GetChild(2).GetComponent<AudioSource>();
        if (_gamePhase == GamePhase.First) _audioSource3 = transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void Start()
    {
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
       if (_isSecondTrackEnabled)
        {
            _audioSource2.volume = Mathf.Lerp(_audioSource2.volume, _maxVolume2, _fadeRate * Time.deltaTime);
            if (_gamePhase == GamePhase.First)  _audioSource3.volume = Mathf.Lerp(_audioSource3.volume, _maxVolume3, _fadeRate * Time.deltaTime);
            return;
        }
        _audioSource2.volume = 0f;
        if (_gamePhase == GamePhase.First) _audioSource3.volume = 0f;

        if (_beginFadeOutWind)
        {
            _audioSourceWind.volume = Mathf.Lerp(_audioSourceWind.volume, 0, _fadeRate/3 * Time.deltaTime);
        }
    }

    public void Pause()
    {
        _audioSource.Pause();
        _audioSource2.Pause();
        if (_gamePhase == GamePhase.First) _audioSource3.Pause();
    }

    public void Play()
    {
        _audioSource.Play();
        _audioSource2.Play();
        if (_gamePhase == GamePhase.First) _audioSource3.Play();
    }

    public void PlayWind()
    {
        _audioSourceWind.Play();
    }

}

public enum GamePhase
{
    First,
    Second,
    Epilogue,
}
