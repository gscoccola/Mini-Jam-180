using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject _platformPrefab;
    Transform _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Update()
    {
        transform.position = _player.position;
    }

}
