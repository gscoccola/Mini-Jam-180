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

    float _angularFrequency;
    float _phase;
    public float _maxXValue;

    Rigidbody2D _rigidBody2D;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _rigidBody2D.linearVelocityX = _horizontalSpeed;
        _rigidBody2D.linearVelocityY = _verticalSpeed;  /*Don't move to Start, this needs to be set here so it's overriden by Spawner if necessary*/
    }

    private void Start()
    {

        if (_oscilates)
        {
            _angularFrequency = (2 * Mathf.PI) / _oscilationPeriod;
            _phase = Random.Range(0, (2 * Mathf.PI));
        }

    }

    private void Update()
    {
        if (transform.position.x > _maxXValue) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        
        if (_oscilates)
        {
            _phase += Time.deltaTime * _angularFrequency;
            _rigidBody2D.linearVelocityY = _verticalSpeed + _oscilationAmplitude * Mathf.Sin(_phase);
        }
    }
}
