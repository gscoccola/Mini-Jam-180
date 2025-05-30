using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BossIntroManager : MonoBehaviour
{
    [SerializeField] BossIntroBehavior Boss;
    [SerializeField] CinemachinePositionComposer _playerCamera;
    CinemachineBasicMultiChannelPerlin _playerCameraShake;

    [SerializeField] float SpawnDelay;
    [SerializeField] float ShakeAmplitude;
    [SerializeField] float ShakeDuration;

    bool _amplitudeIncreasing;
    bool _amplitudeDecreasing;

    private void Awake()
    {
        _playerCameraShake = _playerCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void OnLevelEnd()
    {
        StartCoroutine(IBossSpawnSequence());
    }

    private void Update()
    {
        if (_amplitudeDecreasing) _playerCameraShake.AmplitudeGain = Mathf.Lerp(_playerCameraShake.AmplitudeGain, 0f, ShakeDuration / 2f);
        else if (_amplitudeIncreasing) _playerCameraShake.AmplitudeGain = Mathf.Lerp(_playerCameraShake.AmplitudeGain, ShakeAmplitude, ShakeDuration / 2f);
    }

    private IEnumerator IBossSpawnSequence()
    {
        _playerCamera.Damping = Vector3.one * 99999f;


        yield return new WaitForSeconds(SpawnDelay);

        _amplitudeIncreasing = true;
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossEarthquake);

        yield return new WaitForSeconds(2.8f);

        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossScream);

        yield return new WaitForSeconds(1.2f);

        Boss.StartMovement();

        yield return new WaitForSeconds(ShakeDuration - 1f);

        _amplitudeDecreasing = true;
    }

}
