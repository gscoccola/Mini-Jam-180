using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] float _fadeRate = 0.2f;
    [SerializeField] float _maxVolume2 = 0.2f;
    [SerializeField] float _maxVolume3 = 0.35f;
    AudioSource _audioSource;
    AudioSource _audioSource2;
    AudioSource _audioSource3;
    public bool _isSecondTrackEnabled;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        _audioSource3 = transform.GetChild(1).GetComponent<AudioSource>();
    }

    private void Update()
    {
       if (_isSecondTrackEnabled)
        {
            _audioSource2.volume = Mathf.Lerp(_audioSource2.volume, _maxVolume2, _fadeRate * Time.deltaTime);
            _audioSource3.volume = Mathf.Lerp(_audioSource3.volume, _maxVolume3, _fadeRate * Time.deltaTime);
            return;
        }
        _audioSource2.volume = 0f;
        _audioSource3.volume = 0f;

    }

    public void Pause()
    {
        _audioSource.Pause();
        _audioSource2.Pause();
        _audioSource3.Pause();
    }

    public void Play()
    {
        _audioSource.Play();
        _audioSource2.Play();
        _audioSource3.Play();
    }
}
