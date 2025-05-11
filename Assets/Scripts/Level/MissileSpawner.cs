using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

public class MissileSpawner : MonoBehaviour
{

    private Transform _player;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _missilePrefab;
    private GameObject _enemiesFolder;

    [SerializeField] private float _waveWidth;
    [SerializeField] private float _minSpacing;
    [SerializeField] private int _missilesPerWave;

    [SerializeField] private float _waveCooldown = 3f;
    [SerializeField] private float _warningTime = 1f;

    List<GameObject> _activeLasers = new List<GameObject>();

    private int _laneNumber;
    int[] _laneArray;
    bool _isSpawning;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _enemiesFolder = GameObject.Find("Enemies");
        GenerateLanes();
    }

    private void GenerateLanes()
    {
        _laneNumber = Mathf.FloorToInt(_waveWidth / _minSpacing);
        _laneArray = RandomIntArrayGeneration.Generate(_laneNumber);
    }

    public void StartSpawning()
    {
        StartCoroutine(ISpawnMissileWave());
        _isSpawning = true;
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        foreach (GameObject laser in _activeLasers)
        {
            Destroy(laser);
        }
        _activeLasers.Clear();
    }

    private IEnumerator ISpawnMissileWave()
    {
        

        _laneArray = RandomIntArrayGeneration.Shuffle(_laneArray);
        
        float playerX = _player.transform.position.x;
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._missileWarning);
        for (int i=0; i < _missilesPerWave; i++)
        {
            float xPosition = playerX - _waveWidth/2 + _laneArray[i] * _minSpacing;
            _activeLasers.Add(SpawnLaser(xPosition));
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(_warningTime);
        SFXPlayer.Instance.PlayClipFromArray(SFXPlayer.Instance._missileFall);
        foreach (GameObject laser in _activeLasers)
        {
            Instantiate(_missilePrefab, new Vector3(laser.transform.position.x, _player.transform.position.y + 10f, 0f),
            Quaternion.identity, _enemiesFolder.transform);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1.5f);

        foreach (GameObject laser in _activeLasers)
        {
            Destroy(laser);
        }
        _activeLasers.Clear();

        yield return new WaitForSeconds(_waveCooldown);
        StartCoroutine(ISpawnMissileWave());
    }

    private GameObject SpawnLaser(float xPosition)
    {
        return Instantiate(_laserPrefab, new Vector3(xPosition, 0f, 0f),
            Quaternion.identity, _enemiesFolder.transform);
    }

    public void SetWaveStats(int number, float waveCooldown, float minSpacing)
    {
        _missilesPerWave = number;
        _waveCooldown = waveCooldown;
        _minSpacing = minSpacing;
        GenerateLanes();
        if (number!=0 && !_isSpawning) StartSpawning();
    }
}
