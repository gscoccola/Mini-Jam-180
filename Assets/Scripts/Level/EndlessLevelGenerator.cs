using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class EndlessLevelGenerator : MonoBehaviour
{
    [SerializeField] int _chunksToTransitionBackground = 2;
    [SerializeField] GameObject[] _levelChunkPrefabs;
    [SerializeField] GameObject _startPrefab;
    [SerializeField] DynamicBackground dynamicBackground;
    [SerializeField] float _confinerMinY = -2.3f;
    [SerializeField] float _mapHeight = 45f;

    Transform _playerTransform;
    float _chunkWidth = 50f;
    int _chunkCounter = 0;

    GameObject _previousChunk;
    GameObject _currentChunk;
    GameObject _nextChunk;

    PlayerMovement _playerMovement;
    PolygonCollider2D _polygonCollider2D;
    CinemachineConfiner2D _cameraConfiner;
    Spawner _spawner;
    public bool _spawnBuildings = true;

    private void Awake()
    {
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        _spawner = FindAnyObjectByType<Spawner>();
        _spawner._mapHeight = _mapHeight;
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _polygonCollider2D = FindAnyObjectByType<PolygonCollider2D>();
        _cameraConfiner = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void Start()
    {
        InitializeChunks();
    }

    private void Update()
    {
        if (_currentChunk.transform.position.x - _playerTransform.position.x > _chunkWidth)
        {
            InstantiateNewChunk();
            if (_chunkCounter == 1) dynamicBackground.TransitionBackgrounds();

            _chunkCounter++;
            _chunkCounter %= _chunksToTransitionBackground;
        }
    }

    private void InitializeChunks()
    {
        _currentChunk = Instantiate(_startPrefab, Vector3.zero, Quaternion.identity, this.transform);

        _nextChunk = Instantiate(_levelChunkPrefabs[Random.Range(0, _levelChunkPrefabs.Length)],
            Vector3.left * _chunkWidth, Quaternion.identity, this.transform);
    }

    private void InstantiateNewChunk()
    {
        
        if (_previousChunk!= null) Destroy(_previousChunk);
        _previousChunk = _currentChunk;
        _currentChunk = _nextChunk;
        _nextChunk = _spawnBuildings 
            ? Instantiate(_levelChunkPrefabs[Random.Range(0, _levelChunkPrefabs.Length)], 
            new Vector3(_currentChunk.transform.position.x - _chunkWidth, 0f, 0f), Quaternion.identity, this.transform)
            : Instantiate(_levelChunkPrefabs[0],
            new Vector3(_currentChunk.transform.position.x - _chunkWidth, 0f, 0f), Quaternion.identity, this.transform);
        SetNewBounds();
    }

    private void SetNewBounds()
    {
        float yMax = 4000f;
        float xMin = _currentChunk.transform.position.x - 50f * _chunkWidth;
        float xMax = _currentChunk.transform.position.x + _chunkWidth;
        _polygonCollider2D.points = new Vector2[4]
        {
            new Vector2(xMin, _confinerMinY),
            new Vector2(xMax, _confinerMinY),
            new Vector2(xMax, yMax),
            new Vector2(xMin, yMax),
        };

        _playerMovement.CalculateBounds();
        _cameraConfiner.InvalidateBoundingShapeCache();
        _spawner.SetWorldMax(xMax);
    }
}
