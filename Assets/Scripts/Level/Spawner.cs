using System;
using System.Collections;
using System.Linq;
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
    [FoldoutGroup("Parameters")][SerializeField] private float _goldenBalloonSpawnChance;
    [FoldoutGroup("Parameters")][SerializeField] private float _specialWaveChance;

    [Header("Endless mode?")]
    [FoldoutGroup("Parameters")][SerializeField] private bool _isEndlessLevel;

    [Header("Reference")]
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBird;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabDrone;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabBalloon;
    [FoldoutGroup("Reference")][SerializeField] private GameObject _enemyPrefabGoldenBalloon;

    private float _localTime;
    private float _lastX;
    private Transform _player;
    private int _laneNumber;
    private bool _hasStarted;
    private float _worldMax;
    private GameObject _enemies;
    public float _mapHeight;
    public bool _spawnsSpecialWaves;
    public bool _isTopSparser;
    private float _attenuationPerLane;
    private bool _spawnGoldenBallons;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        _enemies = GameObject.Find("Enemies");
    }

    private void Start()
    {
        if (PersistentInfo.Instance.AchievementCompletionInfo == null) return;
        if (PersistentInfo.Instance.AchievementCompletionInfo.Where(x => x.Value.WasShown).Count() ==
            PersistentInfo.Instance.AchievementCompletionInfo.Count()) _spawnGoldenBallons = true;
    }

    private void OnEnable()
    {
        _lastX = transform.position.x;
       
    }

    public void StartSpawning(bool preSpawn)
    {
        CalculateBounds(); /*careful moving the execution order of this function to before height is set by floor.cs or after GameManager start*/
        CalculateLanes();
        SpawnWave(transform.position.x);
        _hasStarted = true;
        if (preSpawn) InitialBurst();
    }

    private void Update()
    {
        if (!_hasStarted) return;

        transform.position = Vector3.right * (_player.position.x - _spawnXOffset);
        float deltaX = transform.position.x - _lastX;
        _localTime += Mathf.Max(Time.deltaTime * (1 - deltaX / (Time.deltaTime * 3f)), 0f);
        _lastX = transform.position.x;

        if (_localTime > _spawnPeriod)
        {
            _localTime = 0f;
            SpawnWave(transform.position.x);
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
        _attenuationPerLane = _isTopSparser ? 1f / (_laneNumber) : 0f;
    }

    public void InitialBurst()
    {
        for (int i = 0; i < (_spawnXOffset - 6f) / 4f ; i++)
        {
            SpawnWave(_player.position.x - _spawnXOffset  + 4f*i);
        }
    }

    private void SpawnWave(float x)
    {
        for (int i = 0; i <= _laneNumber; i++)
        {
            if (_spawnsSpecialWaves && UnityEngine.Random.Range(0f, 1f) < _specialWaveChance)
            {
                SpawnSpecialWave(x, i);
                i++;
                break;
            }
            SpawnRandomEnemy(new Vector3(x, _minSpawnHeight + _laneHeight * i, 0f), i);
        }
    }

    private void SpawnSpecialWave(float x, int i)
    {
        Vector3 spawnPosition = new Vector3(x, _minSpawnHeight + _laneHeight * i, 0f);
        float ySpeed = Mathf.Clamp( 9 * (_player.position.y  - spawnPosition.y) / _spawnXOffset , -9f, 9f);

        SpawnEnemy(spawnPosition, _enemyPrefabDrone, false, true, ySpeed);
        SpawnEnemy(spawnPosition + 2.5f * Vector3.down, _enemyPrefabDrone, false, true, ySpeed);
        SpawnEnemy(spawnPosition + 5f * Vector3.down, _enemyPrefabDrone, false, true, ySpeed);

        SpawnEnemy(spawnPosition + 1.25f * Vector3.down + 2f * Vector3.left, _enemyPrefabDrone, false, true, ySpeed);
        SpawnEnemy(spawnPosition + 3.75f * Vector3.down + 2f * Vector3.left, _enemyPrefabDrone, false, true, ySpeed);
    }

    private void SpawnRandomEnemy(Vector3 position, int i)
    {
        float randomFloat = i==0 ? UnityEngine.Random.Range(0f, 0.65f) : UnityEngine.Random.Range(0f, 1f + i * _attenuationPerLane);
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
            prefabToSpawn = (_spawnGoldenBallons && UnityEngine.Random.Range(0f, 1f) < _goldenBalloonSpawnChance)
                ? _enemyPrefabGoldenBalloon : _enemyPrefabBalloon;
        }
        else return;
        SpawnEnemy(position, prefabToSpawn);
    }

    private void SpawnEnemy(Vector3 position, GameObject prefab, bool randomPosition = true, bool trackingPlayer = false, float ySpeed = 0f)
    {
        if (trackingPlayer)
        {
            GameObject enemy = Instantiate(prefab, position, Quaternion.identity, _enemies.transform);
            enemy.GetComponent<EnemyMovement>()._maxXValue = _worldMax;
            enemy.GetComponent<Rigidbody2D>().linearVelocityY = ySpeed;
        }
        else if (randomPosition)
        Instantiate(prefab,
            position + Vector3.up * ((UnityEngine.Random.Range(0f, 1f) - 0.5f) * _laneHeight)
            + Vector3.right * (-UnityEngine.Random.Range(0f, 1f) * _minXSpacing),
            Quaternion.identity, _enemies.transform).GetComponent<EnemyMovement>()._maxXValue = _worldMax;
        else
            Instantiate(prefab,
            position,
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

    public void SetSpecialWaveChance(float specialWaveChance)
    {
        _specialWaveChance = specialWaveChance;
        if (!_spawnsSpecialWaves) _spawnsSpecialWaves = true;
    }

   
}
