using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [FoldoutGroup("Parameters")][SerializeField] float _horizontalSpeed;
    [FoldoutGroup("Parameters")][SerializeField] float _verticalSpeed;

    
    EnemyBounceable _enemyBounceable;
    Vector3 _speed;

    private void Awake()
    {
        _enemyBounceable = GetComponent<EnemyBounceable>();
    }

    private void Start()
    {
        _speed = new Vector3(_horizontalSpeed, _verticalSpeed, 0);
    }

    private void Update()
    {
        transform.position += _speed * Time.deltaTime;
    }
}
