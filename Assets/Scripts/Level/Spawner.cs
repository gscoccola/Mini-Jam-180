using System;
using System.Collections;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
    [FoldoutGroup("Parameters")][SerializeField] float _averageSpawnFrequency;
    [FoldoutGroup("Parameters")][SerializeField] float _minSpawnHeight;

    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBalloon;

    Transform _player;
    System.Random _random;
    float _spawnPeriod;
    float _mapHeight;
    float _mapWidth;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _random = new System.Random();
    }

    private void Start()
    {
        _mapWidth = _player.position.x;
        _mapHeight = _player.position.y;
        //InitialSpawn();
        StartCoroutine(IInitialSpawn());
        _spawnPeriod = 1f / _averageSpawnFrequency;
        StartCoroutine(ISpawnCoroutine());
    }

    IEnumerator ISpawnCoroutine()
    {
        SpawnEnemy(-_mapWidth);
        yield return new WaitForSeconds(_spawnPeriod);
        StartCoroutine(ISpawnCoroutine());
    }

    void InitialSpawn()
    {
        for (int i=-(int)_mapWidth; i < _mapWidth; i++)
        {
            SpawnEnemy(i);
        }
    }

    IEnumerator IInitialSpawn()
    {
        for (int i = -(int)_mapWidth; i < (int)_mapWidth; i++)
        {
            yield return new WaitForSeconds(0.01f);
            SpawnEnemy(- i);
        }
    }

    private void SpawnEnemy(float xPosition)
    {
        float height = (float)_random.NextDouble() * (_mapHeight - _minSpawnHeight -2f) + _minSpawnHeight;
        int type = _random.Next(3);
        if (type == 0) Instantiate(_enemyPrefabBird, Vector3.right * xPosition + Vector3.up * height, Quaternion.identity);
        else if (type == 1) Instantiate(_enemyPrefabDrone, Vector3.right * xPosition + Vector3.up * height, Quaternion.identity);
        else if (type == 2) Instantiate(_enemyPrefabBalloon, Vector3.right * xPosition + Vector3.up * height, Quaternion.identity);

    }

}
