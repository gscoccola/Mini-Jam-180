using UnityEngine;

public class DifficultyDirector : MonoBehaviour
{
    Spawner _spawner;
    MissileSpawner _mSpawner;
    private Transform _player;
    private float _initialPlayerX;
    bool _isMissileSpawning = false;

    private void Awake()
    {
        _spawner = FindAnyObjectByType<Spawner>();
        _mSpawner = FindAnyObjectByType<MissileSpawner>();
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _initialPlayerX = _player.position.x;
    }

    private void Update()
    {
        if (!_isMissileSpawning && _player.position.x < _initialPlayerX - 10)
        {
            _mSpawner.StartSpawning();
            _isMissileSpawning = true;
        }
    }
}
