using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;

public class FireworkSpawner : MonoBehaviour
{
    [SerializeField] float displacementRatio;
    [SerializeField] GameObject _fireWorkPrefab;
    [SerializeField] float _offset;
    Camera _camera;
    List<GameObject> _activeFireworkds = new();
    Vector3 _lastCameraPosition;
    const float period = 8.64864864865f;
    List<Color> _colors;
    List<Vector3> _positions1;
    List<Vector3> _positions2;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
    }

    private void Start()
    {
        _lastCameraPosition = _camera.transform.position;
        _colors = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow };
        _positions1 = new List<Vector3>() { new Vector3(3f, 2f, 0f), new Vector3(0f, -2f, 0f), new Vector3(-3f, 2f, 0f) };
        _positions2 = new List<Vector3>() { new Vector3(3f, -2f, 0f), new Vector3(0f, 2f, 0f), new Vector3(-3f, -2f, 0f) };
    }

    public void Begin()
    {
        StartCoroutine(ISpawnWave());
    }

    public IEnumerator ISpawnWave()
    {
        while (true)
        {
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._fireworkWhistle);

            yield return new WaitForSeconds(2f);

            List<Vector3> positions = Random.Range(0, 2) == 0 ? _positions1 : _positions2;
            _colors.Shuffle();
            positions.Shuffle();

            StartCoroutine(ISpawnFirework(positions[2], _colors[2]));


            yield return new WaitForSeconds(0.2f);


            StartCoroutine(ISpawnFirework(positions[1], _colors[1]));


            yield return new WaitForSeconds(0.2f);

            StartCoroutine(ISpawnFirework(positions[0], _colors[0]));

            yield return new WaitForSeconds(period - 2.4f);
        }
    }

    private IEnumerator ISpawnFirework(Vector3 position, Color color)
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._fireworkExplosion);
        Vector3 startPosition = _camera.transform.position + position;
        GameObject fireWork = Instantiate(_fireWorkPrefab,
            new Vector3(startPosition.x + Random.Range(-_offset, _offset), startPosition.y + Random.Range(-_offset, _offset), 0f),
            Quaternion.identity);
        VibrationHandler.Instance.LightVibrate();
        var main = fireWork.GetComponent<ParticleSystem>().main;
        main.startColor = color;        
        _activeFireworkds.Add(fireWork);
        yield return new WaitForSeconds(5f);
        _activeFireworkds.Remove(fireWork);
        Destroy(fireWork.gameObject, 1f);
    }

    private void Update()
    {
        foreach (GameObject fireWork in _activeFireworkds)
        {
            fireWork.transform.position += (1f-displacementRatio) * (_camera.transform.position - _lastCameraPosition);
            fireWork.transform.position = new Vector3(fireWork.transform.position.x, fireWork.transform.position.y, 0f);
        }

        _lastCameraPosition = _camera.transform.position;
    }
}
