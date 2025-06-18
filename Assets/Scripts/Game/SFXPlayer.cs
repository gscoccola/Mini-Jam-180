using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public class SFXPlayer : Singleton<SFXPlayer>
{
    [Header("Enemy Death")]
    [FoldoutGroup("Enemy")] public AudioClip[] _birdDeath;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _birdDeathVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _droneDeath;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _droneDeathVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _balloonDeath;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _balloonDeathVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _droneShock;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _droneShockVolume;

    [Header("Missiles")]
    [FoldoutGroup("Enemy")] public AudioClip[] _missileWarning;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _missileWarningVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _missileFall;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _missileFallVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _missileHit;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _missileHitVolume;

    [Header("Boss")]
    [FoldoutGroup("Enemy")] public AudioClip[] _bossScream;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossScreamVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossEarthquake;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossEarthquakeVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossCigarAttack;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossCigarAttackVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossCigarHit;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossCigarHitVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossSplash;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossSplashVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossHurtShort;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossHurtShortVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossHurtLong;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossHurtLongVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossStun;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossStunVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossFistConnect;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossFistConnectVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossDived;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossDivedVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossThrow;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossThrowVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossPunch;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossPunchVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossNewPhase;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossNewPhaseVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossExplosion;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossExplosionVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossBigExplosion;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossBigExplosionVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossDead;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossDeadVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossCharge;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossChargeVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossChargeWoosh;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossChargeWooshVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossChargeWarning;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossChargeWarningVolume;
    [FoldoutGroup("Enemy")] public AudioClip[] _bossVictoryLaugh;
    [FoldoutGroup("Enemy")][SerializeField][Range(0f, 1f)] float[] _bossVictoryLaughVolume;


    [Header("Player Fall")]
    [FoldoutGroup("Player")] public AudioClip[] _playerSplash;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerSplashVolume;
    [FoldoutGroup("Player")] public AudioClip[] _playerDeath;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerDeathVolume;

    [Header("End Dialog")]
    [FoldoutGroup("Player")] public AudioClip[] _winDialog;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _winDialogVolume;

    [Header("Player footsteps")]
    [FoldoutGroup("Player")] public AudioClip[] _playerRightFootsteps;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerRightFootstepsVolume;
    [FoldoutGroup("Player")] public AudioClip[] _playerLeftFootsteps;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerLeftFootstepsVolume;

    [Header("Player misc")]
    [FoldoutGroup("Player")] public AudioClip[] _playerJump;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerJumpVolume;
    [FoldoutGroup("Player")] public AudioClip[] _playerHurt;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerHurtVolume;
    [FoldoutGroup("Player")] public AudioClip[] _playerBounce;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerBounceVolume;
    [FoldoutGroup("Player")] public AudioClip[] _playerTaunt;
    [FoldoutGroup("Player")][SerializeField][Range(0f, 1f)] float[] _playerTauntVolume;

    [Header("End Music")]
    [FoldoutGroup("Music")] public AudioClip[] _victory;
    [FoldoutGroup("Music")][SerializeField][Range(0f, 1f)] float[] _victoryVolume;
    [FoldoutGroup("Music")] public AudioClip[] _defeat;
    [FoldoutGroup("Music")][SerializeField][Range(0f, 1f)] float[] _defeatVolume;

    [Header("Lost Combo")]
    [FoldoutGroup("Music")] public AudioClip[] _comboLost;
    [FoldoutGroup("Music")][SerializeField][Range(0f, 1f)] float[] _comboLostVolume;

    [Header("Button")]
    [FoldoutGroup("Game misc")] public AudioClip[] _button;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _buttonVolume;
    [FoldoutGroup("Game misc")] public AudioClip[] _buttonSmall;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _buttonSmallVolume;

    [Header("Achievement Click")]
    [FoldoutGroup("Game misc")] public AudioClip[] _achievement;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _achievementVolume;

    [Header("Fireworks")]
    [FoldoutGroup("Game misc")] public AudioClip[] _fireworkExplosion;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _fireworkExplosionVolume;
    [FoldoutGroup("Game misc")] public AudioClip[] _fireworkWhistle;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _fireworkWhistleVolume;

    [Header("Rewards")]
    [FoldoutGroup("Game misc")] public AudioClip[] _padlock;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _padlockVolume;
    [FoldoutGroup("Game misc")] public AudioClip[] _chestOpen;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _chestOpenVolume;
    [FoldoutGroup("Game misc")] public AudioClip[] _balloonRelease;
    [FoldoutGroup("Game misc")][SerializeField][Range(0f, 1f)] float[] _balloonReleaseVolume;

    Vector3 _cameraPosition;
    PlayerMovement _playerMovement;
    Dictionary<AudioClip[], float[]> _volumeDictionary = new();
    GamePhase _gamePhase;

    public void OnLevelLoad(int levelIndex)
    {
        _cameraPosition = FindAnyObjectByType<Camera>().transform.position;
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _gamePhase = FindAnyObjectByType<MusicPlayer>()._gamePhase;

        _volumeDictionary = new Dictionary<AudioClip[], float[]>()
        {
            [_birdDeath] = _birdDeathVolume,
            [_droneDeath] = _droneDeathVolume,
            [_balloonDeath] = _balloonDeathVolume,
            [_playerSplash] = _playerSplashVolume,
            [_playerDeath] = _playerDeathVolume,
            [_playerJump] = _playerJumpVolume,
            [_playerHurt] = _playerHurtVolume,
            [_playerBounce] = _playerBounceVolume,
            [_playerRightFootsteps] = _playerRightFootstepsVolume,
            [_playerLeftFootsteps] = _playerLeftFootstepsVolume,
            [_victory] = _victoryVolume,
            [_defeat] = _defeatVolume,
            [_comboLost] = _comboLostVolume,
            [_winDialog] = _winDialogVolume,
            [_button] = _buttonVolume,
            [_fireworkExplosion] = _fireworkExplosionVolume,
            [_fireworkWhistle] = _fireworkWhistleVolume,
            [_achievement] = _achievementVolume,
            [_missileWarning] = _missileWarningVolume,
            [_missileFall] = _missileFallVolume,
            [_missileHit] = _missileHitVolume,
            [_padlock] = _padlockVolume,
            [_chestOpen] = _chestOpenVolume,
            [_balloonRelease] = _balloonReleaseVolume,
            [_droneShock] = _droneShockVolume,
            [_buttonSmall] = _buttonSmallVolume,
            [_bossScream] = _bossScreamVolume,
            [_bossEarthquake] = _bossEarthquakeVolume,
            [_bossCigarAttack] = _bossCigarAttackVolume,
            [_bossCigarHit] = _bossCigarHitVolume,
            [_bossSplash] = _bossSplashVolume,
            [_bossHurtShort] = _bossHurtShortVolume,
            [_bossHurtLong] = _bossHurtLongVolume,
            [_bossStun] = _bossStunVolume,
            [_bossFistConnect] = _bossFistConnectVolume,
            [_bossDived] = _bossDivedVolume,
            [_bossThrow] = _bossThrowVolume,
            [_bossPunch] = _bossPunchVolume,
            [_bossNewPhase] = _bossNewPhaseVolume,
            [_bossExplosion] = _bossExplosionVolume,
            [_bossDead] = _bossDeadVolume,
            [_bossBigExplosion] = _bossBigExplosionVolume,
            [_bossCharge] = _bossChargeVolume,
            [_bossChargeWoosh] = _bossChargeWooshVolume,
            [_bossChargeWarning] = _bossChargeWarningVolume,
            [_bossVictoryLaugh] = _bossVictoryLaughVolume,
            [_playerTaunt] = _playerTauntVolume,

        };
    }

    public void PlayRandomClipFromArray(AudioClip[] clipArray, float volume = 0f, bool dontDestroyOnLoad = false)
    {
        
        int indexToPlay = 0;
        if (clipArray.Length > 1)
        {
            if (clipArray == _playerBounce) indexToPlay = _playerMovement.PlayerCombo;
            else if (clipArray == _comboLost) indexToPlay = (int)_gamePhase;                
            else indexToPlay = UnityEngine.Random.Range(0, clipArray.Length);
        }
        if (volume != 0f)
        {
            PlayClipAtPoint(clipArray[indexToPlay], _cameraPosition, volume);
            return;
        }
        PlayClipAtPoint(clipArray[indexToPlay], _cameraPosition, _volumeDictionary[clipArray][indexToPlay], dontDestroyOnLoad);
    }

    public void PlayClipFromArrayAtIndex(AudioClip[] clipArray, int index, bool dontDestroyOnLoad = false)
    {
        PlayClipAtPoint(clipArray[index], _cameraPosition, _volumeDictionary[clipArray][index], dontDestroyOnLoad);
    }

    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, float volume = 1, bool dontDestroyOnLoad = false)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        AudioSource tempASource = tempGO.AddComponent<AudioSource>(); // add an audio source
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(tempASource);
        }
        tempASource.clip = clip;
        tempASource.volume = volume;
        tempASource.rolloffMode = AudioRolloffMode.Linear;
        tempASource.maxDistance = 200f;
        // set other aSource properties here, if desired
        tempASource.Play(); // start the sound
        MonoBehaviour.Destroy(tempGO, tempASource.clip.length); // destroy object after clip duration (this will not account for whether it is set to loop)
        return tempASource; // return the AudioSource reference
    }

}
