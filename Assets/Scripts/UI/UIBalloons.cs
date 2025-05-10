using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UIBalloons : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Transform[] _balloons;
    List<Transform> _activeBallons;

    private void Start()
    {
        foreach (var balloon in _balloons)
        {
            balloon.GetComponent<Animator>().Play("Balloon", 0, Random.Range(0f, 1f));
        }
        _activeBallons = new List<Transform>() { _balloons[0], _balloons[1], _balloons[2] };
        StartCoroutine(IBalloons());
    }

    private IEnumerator IBalloons()
    {
        yield return new WaitForSecondsRealtime(0.45f);
        _activeBallons.Add(_balloons[3]);
        _activeBallons.Add(_balloons[4]);
    }

    void Update()
    {
        foreach (var balloon in _activeBallons)
        {
            balloon.position += Vector3.up * _speed * Time.unscaledDeltaTime;
        }
    }
}
