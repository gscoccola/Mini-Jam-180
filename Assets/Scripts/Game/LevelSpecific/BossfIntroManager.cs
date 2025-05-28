using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class BossfIntroManager : MonoBehaviour
{
    [SerializeField] BossIntroBehavior Boss;
    [SerializeField] CinemachinePositionComposer _playerCamera;

    [SerializeField] float SpawnDelay;
   


    public void OnLevelEnd()
    {
        StartCoroutine(IBossSpawnSequence());
    }

    private IEnumerator IBossSpawnSequence()
    {
        _playerCamera.Damping = Vector3.one * 99999f;
        yield return new WaitForSeconds(SpawnDelay);
        Boss.StartMovement();
    }

}
