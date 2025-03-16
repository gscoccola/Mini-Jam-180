using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [Header("EnemyDeath")]
    public AudioClip[] _birdDeath;
    public AudioClip[] _droneDeath;

    [Header("Player Fall")]
    public AudioClip[] _playerWin;
    public AudioClip[] _playerDeath;


    public void PlayRandomClip(AudioClip[] clipArray)
    {
        AudioSource.PlayClipAtPoint(clipArray[Random.Range(0, clipArray.Length)], transform.position);
    }


    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume = 1, float pitch = 1, bool dontDestroyOnLoad = false)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource tempASource = tempGO.AddComponent<AudioSource>(); // add an audio source
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(tempASource);
        }
        tempASource.clip = clip;
        tempASource.pitch = pitch;
        tempASource.volume = volume;
        // set other aSource properties here, if desired
        tempASource.rolloffMode = AudioRolloffMode.Linear;
        tempASource.maxDistance = 200f;
        tempASource.Play(); // start the sound
        MonoBehaviour.Destroy(tempGO, tempASource.clip.length); // destroy object after clip duration (this will not account for whether it is set to loop)
        return tempASource; // return the AudioSource reference
    }

}
