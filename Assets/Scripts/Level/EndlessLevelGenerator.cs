using TMPro;
using UnityEngine;

public class EndlessLevelGenerator : MonoBehaviour
{
    [SerializeField] int _chunksToTransitionBackground = 2;
    [SerializeField] GameObject[] _levelChunkPrefabs;
    [SerializeField] GameObject _startPrefab;
    [SerializeField] DynamicBackground dynamicBackground;

    Transform _playerTransform;
    float _chunkWidth = 50f;
    int _chunkCounter = 0;

    GameObject _previousChunk;
    GameObject _currentChunk;
    GameObject _nextChunk;

    private void Awake()
    {
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        FindAnyObjectByType<Spawner>()._mapHeight = 45f;
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
        _nextChunk = Instantiate(_levelChunkPrefabs[Random.Range(0, _levelChunkPrefabs.Length)], 
            new Vector3(_currentChunk.transform.position.x - _chunkWidth, 0f, 0f), Quaternion.identity, this.transform);
    }
}
