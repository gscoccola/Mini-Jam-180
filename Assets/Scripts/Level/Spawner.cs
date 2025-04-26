using System;
using System.Collections;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    [Header("Spacing")]
    [FoldoutGroup("Parameters")][SerializeField] private float _laneHeight;
    [FoldoutGroup("Parameters")][SerializeField] private float _minXSpacing;

    [Header("Spawn")]
    [FoldoutGroup("Parameters")][SerializeField] private float _spawnPeriod;
    [FoldoutGroup("Parameters")][SerializeField] private float _minSpawnHeight;
    [FoldoutGroup("Parameters")][SerializeField] private float _spawnXOffset;

    [Header("Chances")]
    [FoldoutGroup("Parameters")][SerializeField] private float _birdSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] private float _droneSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] private float _balloonSpawnChance;

    [Header("Reference")]
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBalloon;

    private float _localTime;
    private float _lastX;
    private Transform _player;
    private System.Random _random;
    private int _laneNumber;
    private bool _hasStarted;
    private float _worldMax;
    private GameObject _enemies;
    public float _mapHeight;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _random = new System.Random();
        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        _enemies = GameObject.Find("Enemies");
    }

    private void Start()
    {
        CalculateBounds();
        CalculateLanes();
        _lastX = transform.position.x;
    }

    public void StartSpawning()
    {
        SpawnEnemies();
        _hasStarted = true;
    }

    private void Update()
    {
        if (!_hasStarted) return;

        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        float deltaX = transform.position.x - _lastX;
        _localTime += Mathf.Max(Time.deltaTime * (1 - deltaX / (Time.deltaTime * 3)), 0f);
        _lastX = transform.position.x;

        if (_localTime > _spawnPeriod)
        {
            _localTime = 0f;
            SpawnEnemies();
        }
    }

    private void CalculateLanes()
    {
        if (_laneHeight <= 0)
        {
            Debug.LogError("Lane height must be greater than zero.");
            _laneNumber = 0;
            return;
        }

        _laneNumber = Mathf.FloorToInt((_mapHeight - _minSpawnHeight - _laneHeight) / _laneHeight);
    }

    private void SpawnEnemies()
    {
        for (int i = 1; i <= _laneNumber; i++)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, _minSpawnHeight + _laneHeight * i, 0f);
            SpawnRandomEnemy(spawnPosition);
        }
    }

    private void SpawnRandomEnemy(Vector3 position)
    {
        double spawnRandomDouble = _random.NextDouble();
        GameObject prefabToSpawn = null;

        if (spawnRandomDouble < _birdSpawnChance)
        {
            prefabToSpawn = _enemyPrefabBird;
        }
        else if (spawnRandomDouble < _birdSpawnChance + _droneSpawnChance)
        {
            prefabToSpawn = _enemyPrefabDrone;
        }
        else if (spawnRandomDouble < _birdSpawnChance + _droneSpawnChance + _balloonSpawnChance)
        {
            prefabToSpawn = _enemyPrefabBalloon;
        }

        if (prefabToSpawn != null)
        {
            SpawnEnemy(position, prefabToSpawn);
        }
    }

    private void SpawnEnemy(Vector3 position, GameObject prefab)
    {
        Instantiate(prefab,
            position + Vector3.up * (((float)_random.NextDouble() - 0.5f) * _laneHeight)
            + Vector3.right * (-(float)_random.NextDouble() * _minXSpacing),
            Quaternion.identity, _enemies.transform).GetComponent<EnemyMovement>()._maxXValue = _worldMax;
    }

    private void CalculateBounds()
    {
        var collider = FindAnyObjectByType<PolygonCollider2D>();
        if (collider == null)
        {
            Debug.LogError("PolygonCollider2D not found in the scene.");
            enabled = false;
            return;
        }

        _worldMax = collider.bounds.max.x + 0.6f;
    }
}
