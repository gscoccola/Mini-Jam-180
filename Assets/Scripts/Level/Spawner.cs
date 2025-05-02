using System;
using System.Collections;
using Alchemy.Inspector;
using UnityEngine;

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

    [Header("Endless mode?")]
    [FoldoutGroup("Parameters")][SerializeField] private bool _isEndlessLevel;

    [Header("Reference")]
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBalloon;

    private float _localTime;
    private float _lastX;
    private Transform _player;
    private int _laneNumber;
    private bool _hasStarted;
    private float _worldMax;
    private GameObject _enemies;
    public float _mapHeight;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        _enemies = GameObject.Find("Enemies");
    }

    private void OnEnable()
    {
        _lastX = transform.position.x;
    }

    public void StartSpawning(bool preSpawn)
    {
        CalculateBounds(); /*careful moving the execution order of this function to before height is set by floor.cs or after GameManager start*/
        CalculateLanes();
        SpawnEnemyWave(transform.position.x);
        _hasStarted = true;
        if (preSpawn) InitialBurst();
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
            SpawnEnemyWave(transform.position.x);
        }
    }

    private void CalculateLanes()
    {
        if (_mapHeight <= _minSpawnHeight + _laneHeight)
        {
            Debug.LogError("Lane height must be greater than zero.");
            _laneNumber = 0;
            return;
        }

        _laneNumber = Mathf.FloorToInt((_mapHeight - _minSpawnHeight - _laneHeight) / _laneHeight);
    }

    public void InitialBurst()
    {
        SpawnEnemyWave(_player.position.x - 2 * _spawnXOffset / 3f);
        SpawnEnemyWave(_player.position.x - _spawnXOffset / 3f);
    }

    private void SpawnEnemyWave(float x)
    {
        for (int i = 1; i <= _laneNumber; i++)
        {
            Vector3 spawnPosition = new Vector3(x, _minSpawnHeight + _laneHeight * i, 0f);
            SpawnRandomEnemy(spawnPosition);
        }
    }

    private void SpawnRandomEnemy(Vector3 position)
    {
        float randomFloat = UnityEngine.Random.Range(0f, 1f);
        GameObject prefabToSpawn = null;
        

        if (randomFloat < _birdSpawnChance)
        {
            prefabToSpawn = _enemyPrefabBird;
        }
        else if (randomFloat < _birdSpawnChance + _droneSpawnChance)
        {
            prefabToSpawn = _enemyPrefabDrone;
        }
        else if (randomFloat < _birdSpawnChance + _droneSpawnChance + _balloonSpawnChance)
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
            position + Vector3.up * ((UnityEngine.Random.Range(0f, 1f) - 0.5f) * _laneHeight)
            + Vector3.right * (-UnityEngine.Random.Range(0f, 1f) * _minXSpacing),
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

    public void SetWorldMax(float worldMax)
    {
        _worldMax = worldMax;
    }
}
