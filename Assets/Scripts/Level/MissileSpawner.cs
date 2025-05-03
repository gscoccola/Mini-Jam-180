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

    List<GameObject> _activeLasers = new List<GameObject>();
    private int _laneNumber;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _enemiesFolder = GameObject.Find("Enemies");
        _laneNumber = Mathf.FloorToInt(_waveWidth / _minSpacing);
    }

    public void StartSpawning()
    {
        StartCoroutine(ISpawnMissileWave());
    }

    private IEnumerator ISpawnMissileWave()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i=0; i < _laneNumber; i++)
        {
            float xPosition = _player.transform.position.x - _waveWidth/2 + i * _minSpacing;
            _activeLasers.Add(SpawnLaser(xPosition));
        }
        
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject laser in _activeLasers)
        {
            Instantiate(_missilePrefab, new Vector3(laser.transform.position.x, _player.transform.position.y + 10f, 0f),
            Quaternion.identity, _enemiesFolder.transform);
        }

        yield return new WaitForSeconds(1.5f);

        foreach (GameObject laser in _activeLasers)
        {
            Destroy(laser);
        }
        _activeLasers.Clear();

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ISpawnMissileWave());
    }

    private GameObject SpawnLaser(float xPosition)
    {
        return Instantiate(_laserPrefab, new Vector3(xPosition, 0f, 0f),
            Quaternion.identity, _enemiesFolder.transform);
    }
}
