using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class FireworkSpawner : MonoBehaviour
{
    [SerializeField] GameObject _fireWorkPrefab;
    Camera _camera;
    List<GameObject> _activeFireworkds = new();

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void SpawnFirework()
    {
        //_camera.ScreenToWorldPoint
        GameObject fireWork = Instantiate(_fireWorkPrefab, new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f), Quaternion.identity);
    }
}
