using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [FoldoutGroup("Parameters")][SerializeField] float _horizontalSpeed;
    [FoldoutGroup("Parameters")][SerializeField] float _verticalSpeed;

    [FoldoutGroup("Parameters")][SerializeField] bool _oscilates;
    [FoldoutGroup("Parameters")][SerializeField] float _oscilationAmplitude;
    [FoldoutGroup("Parameters")][SerializeField] float _oscilationPeriod;


    Vector3 basePosition;
    float _angularFrequency;
    float phase = 0;


    EnemyBounceable _enemyBounceable;
    Vector3 _speed;

    private void Awake()
    {
        _enemyBounceable = GetComponent<EnemyBounceable>();
    }

    private void Start()
    {
        _speed = new Vector3(_horizontalSpeed, _verticalSpeed, 0);
        
        if (_oscilates)
        {
            _angularFrequency = (2 * Mathf.PI) / _oscilationPeriod;
            basePosition = transform.position;
            phase = Random.Range(0, (2 * Mathf.PI));
        }
            
    }

    private void FixedUpdate()
    {
        
        if (_oscilates)
        {
            basePosition += _speed * Time.deltaTime;
            phase += Time.deltaTime * _angularFrequency;
            transform.position = basePosition + Vector3.up * _oscilationAmplitude * Mathf.Sin(phase);
        }
        else transform.position += _speed * Time.deltaTime;


    }
}
