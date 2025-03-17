using System;
using System.Collections;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [Header("Spawn")]
    [FoldoutGroup("Parameters")][SerializeField] float _maxSpawnsPerSecond;
    [FoldoutGroup("Parameters")][SerializeField] float _minSpawnHeight;
    [FoldoutGroup("Parameters")][SerializeField] float _spawnXOffset;

    [Header("Chances")]
    [FoldoutGroup("Parameters")][SerializeField] float _birdSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] float _droneSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] float _balloonSpawnChance;

    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBalloon;

    Transform _player;
    System.Random _random;
    float _spawnPeriod;
    float _mapHeight;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _random = new System.Random();
    }

    private void Start()
    {
        _mapHeight = Mathf.Max(_player.position.y, 50f);
        _spawnPeriod = 1f / _maxSpawnsPerSecond;
        StartCoroutine(ISpawnCoroutine());
    }

    IEnumerator ISpawnCoroutine()
    {
        float XPosition = _player.position.x - _spawnXOffset;
        if (_random.NextDouble() < _birdSpawnChance) SpawnEnemy(XPosition, _enemyPrefabBird);
        if (_random.NextDouble() < _droneSpawnChance) SpawnEnemy(XPosition, _enemyPrefabDrone);
        if (_random.NextDouble() < _balloonSpawnChance) SpawnEnemy(XPosition, _enemyPrefabBalloon);

        yield return new WaitForSeconds(_spawnPeriod);
        StartCoroutine(ISpawnCoroutine());
    }

    private void SpawnEnemy(float xPosition, GameObject prefab)
    {
        float height = (float)_random.NextDouble() * (_mapHeight - _minSpawnHeight -2f) + _minSpawnHeight;
        Instantiate(prefab, Vector3.right * xPosition + Vector3.up * height, Quaternion.identity);
    }

}
