using UnityEngine;
using System.Collections.Generic;

public class DifficultyDirector : MonoBehaviour
{
    [SerializeField] DifficultyMilestoneSO[] _difficultyMilestones;
    int currentMilestoneIndex = 0;
    private float _nextMilestoneDistance;

    Spawner _spawner;
    MissileSpawner _mSpawner;
    private Transform _player;
    private float _initialPlayerX;

    private void Awake()
    {
        _spawner = FindAnyObjectByType<Spawner>();
        _mSpawner = FindAnyObjectByType<MissileSpawner>();
        _player = FindAnyObjectByType<PlayerMovement>().transform;
        _initialPlayerX = _player.position.x;
        _nextMilestoneDistance = (_difficultyMilestones.Length != currentMilestoneIndex) ?
            _difficultyMilestones[0].GetDistance() :
            Mathf.Infinity;
    }

    private void Update()
    {
        if (_player.position.x - _initialPlayerX < -_nextMilestoneDistance)
        {
            ChangeDifficultySettings();
        }
    }

    private void ChangeDifficultySettings()
    {
        _mSpawner.SetMissileNumber(_difficultyMilestones[currentMilestoneIndex].GetMissileNumber());
        _spawner.SetSpecialWaveChance(_difficultyMilestones[currentMilestoneIndex].GetSpecialWaveChance());
        currentMilestoneIndex++;
        _nextMilestoneDistance = (_difficultyMilestones.Length != currentMilestoneIndex) ?
            _difficultyMilestones[currentMilestoneIndex].GetDistance() :
            Mathf.Infinity;
    }
}
