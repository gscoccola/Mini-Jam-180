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

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _enemiesFolder = GameObject.Find("Enemies");
        _laneNumber = Mathf.FloorToInt(_waveWidth / _minSpacing);
        _laneArray = RandomIntArrayGeneration.Generate(_laneNumber);
    }

    public void StartSpawning()
    {
        StartCoroutine(ISpawnMissileWave());
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

        /*string lanes = "";
        for (int i = 0; i < _laneArray.Length; i++)
        {
            lanes += " " + _laneArray[i];
        }
        Debug.Log(lanes);*/
        float playerX = _player.transform.position.x;

        for (int i=0; i < _missilesPerWave; i++)
        {
            float xPosition = playerX - _waveWidth/2 + _laneArray[i] * _minSpacing;
            _activeLasers.Add(SpawnLaser(xPosition));
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(_warningTime);
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
}
