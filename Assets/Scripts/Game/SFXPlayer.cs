using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [Header("EnemyDeath")]
    public AudioClip[] _birdDeath;
    public AudioClip[] _droneDeath;
    public AudioClip[] _balloonDeath;

    [Header("Player Fall")]
    public AudioClip[] _playerWin;
    public AudioClip[] _playerDeath;

    [Header("Player")]
    public AudioClip[] _playerJump;
    public AudioClip[] _playerHurt;
    public AudioClip[] _playerBounce;

    [Header("End Music")]
    public AudioClip[] _victory;
    public AudioClip[] _defeat;

    [Header("End Dialog")]
    public AudioClip[] _winDialog;

    float _combo = 0f;

    public void PlayRandomClip(AudioClip[] clipArray)
    {
        
        int indexToPlay;
        if (clipArray== _playerBounce)
        {
            IncreaseCombo();
            indexToPlay = (int)_combo;
        }
        else if (clipArray == _playerHurt)
        {
            ResetCombo();
            indexToPlay = Random.Range(0, clipArray.Length);
        }
        else
        {
            indexToPlay = Random.Range(0, clipArray.Length);
        }
        AudioSource.PlayClipAtPoint(clipArray[indexToPlay], transform.position);
    }

    void IncreaseCombo()
    {
        _combo++;
        _combo = Mathf.Clamp(_combo, 0f, _playerBounce.Length - 1f);
    }

    void ResetCombo()
    {
        _combo = 0f;
        _combo = Mathf.Clamp(_combo, 0f, _playerBounce.Length - 1f);
    }

}
