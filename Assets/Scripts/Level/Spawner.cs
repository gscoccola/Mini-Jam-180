using System;
using System.Collections;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [Header("Spacing")]
    [FoldoutGroup("Parameters")][SerializeField] float _laneHeight;
    [FoldoutGroup("Parameters")][SerializeField] float _minXSpacing;

    [Header("Spawn")]
    [FoldoutGroup("Parameters")][SerializeField] float _spawnPeriod;
    [FoldoutGroup("Parameters")][SerializeField] float _minSpawnHeight;
    [FoldoutGroup("Parameters")][SerializeField] float _spawnXOffset;

    [Header("Chances")]
    [FoldoutGroup("Parameters")][SerializeField] float _birdSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] float _droneSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] float _balloonSpawnChance;

    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] GameObject _enemyPrefabBalloon;

    float _localTime;
    float _lastX;
    Transform _player;
    System.Random _random;
    float _mapHeight;
    int _laneNumber;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _random = new System.Random();
        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
    }

    private void Start()
    {
        _mapHeight = Mathf.Max(_player.position.y, 50f);
        CalculateLanes();
        _lastX = transform.position.x;
    }

    private void Update()
    {
        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        _localTime += Mathf.Max( Time.deltaTime  * (
             1 - (transform.position.x - _lastX) / (Time.deltaTime * 3 )
            ), 0f) ;
        _lastX = transform.position.x;

        if (_localTime > _spawnPeriod)
        {
            _localTime = 0f;
            SpawnEnemies();
        }
    }

    private void CalculateLanes()
    {
        _laneNumber = Mathf.FloorToInt((_mapHeight- _minSpawnHeight)/_laneHeight);
    }


    void SpawnEnemies()
    {
        for (int i = 1; i<= _laneNumber; i++)
        {
            double spawnRandomDouble = _random.NextDouble();
            Vector3 spawnPosition = new Vector3(transform.position.x,
                _minSpawnHeight + _laneHeight * i, 0f);

            if (spawnRandomDouble < _birdSpawnChance) SpawnEnemy(spawnPosition, _enemyPrefabBird);
            else if (spawnRandomDouble < _droneSpawnChance + _birdSpawnChance) SpawnEnemy(spawnPosition, _enemyPrefabDrone);
            else if (spawnRandomDouble < _balloonSpawnChance + _droneSpawnChance + _birdSpawnChance) SpawnEnemy(spawnPosition, _enemyPrefabBalloon);
        }

        double spawnRandomDouble0 = _random.NextDouble();
        Vector3 spawnPosition0 = new Vector3(transform.position.x,
            _minSpawnHeight + _laneHeight, 0f);

        if (spawnRandomDouble0 < 2*_birdSpawnChance) SpawnEnemy(spawnPosition0, _enemyPrefabBird);
        else if (spawnRandomDouble0 < _droneSpawnChance + 2 * _birdSpawnChance) SpawnEnemy(spawnPosition0, _enemyPrefabDrone);
        else if (spawnRandomDouble0 < 2 * _balloonSpawnChance + 2 * _droneSpawnChance + 2 * _birdSpawnChance) SpawnEnemy(spawnPosition0, _enemyPrefabBalloon);
    }

    private void SpawnEnemy(Vector3 position, GameObject prefab)
    {
        Instantiate(prefab,
            position + Vector3.up * (((float)_random.NextDouble() - 0.5f) * _laneHeight)
            + Vector3.right * (((float)_random.NextDouble() - 0.5f) * _minXSpacing),
            Quaternion.identity);
    }

}
