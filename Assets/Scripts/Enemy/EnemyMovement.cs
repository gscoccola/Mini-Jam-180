using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Speeds")]
    [FoldoutGroup("Parameters")][SerializeField] float _birdSpeed;
    [FoldoutGroup("Parameters")][SerializeField] float _droneSpeed;

    Dictionary<EnemyType, float> _enemySpeeds = new Dictionary<EnemyType, float>();
    EnemyBounceable _enemyBounceable;
    float _speed;

    private void Awake()
    {
        _enemyBounceable = GetComponent<EnemyBounceable>();
    }

    private void Start()
    {
        _enemySpeeds = new Dictionary<EnemyType, float>
        {
            [EnemyType.Bird] = _birdSpeed,
            [EnemyType.Drone] = _droneSpeed,
            [EnemyType.Tender] = 0f,
        };

        _speed = _enemySpeeds[_enemyBounceable._enemytype];
    }

    private void Update()
    {
        transform.position += Vector3.right * _speed * Time.deltaTime;
    }
}
