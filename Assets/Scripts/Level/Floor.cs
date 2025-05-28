using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;
    PlayerMovement _playerMovement;
    [SerializeField] FloorType _floorType;
    [SerializeField] bool _isEndlessLevel;
    [SerializeField] float _groundOffset;
    [SerializeField] GameObject _bloodSplatter;
    [SerializeField] GameObject _waterSplash;
    TilemapCollider2D _tilemapCollider2D;
    Tilemap _tilemap;
    bool _isTriggered;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        _tilemap = GetComponent<Tilemap>();
        SetConfiner();
        DefineBoundaries();
    }
    private void Start()
    {
        if (_floorType == FloorType.Ground)  _tilemapCollider2D.offset = new Vector2(0f, - _groundOffset);
    }

    private void SetConfiner()
    {
        if (_isEndlessLevel) return;
        if (_floorType != FloorType.Ground) return;
        Bounds bounds = _tilemapCollider2D.bounds;
        float xMin = bounds.min.x;
        float xMax = bounds.max.x;
        float yMin = bounds.min.y + _groundOffset;

        float yMax = 4000f;
        FindAnyObjectByType<PolygonCollider2D>().points = new Vector2[4]
        {
            new Vector2(xMin, yMin),
            new Vector2(xMax, yMin),
            new Vector2(xMax, yMax),
            new Vector2(xMin, yMax),
        };
        FindAnyObjectByType<CinemachineConfiner2D>().InvalidateBoundingShapeCache();
    }

    private void DefineBoundaries()
    {
        if (_floorType != FloorType.Water && _floorType != FloorType.BossTrigger) return;
        Bounds bounds = _tilemapCollider2D.bounds;
        float worldMin = _tilemap.transform.TransformPoint(bounds.min).x;
        float worldMax = _tilemap.transform.TransformPoint(bounds.max).x;
        FindAnyObjectByType<TargetIndicator>().SetBounds(worldMin, worldMax);
        FindAnyObjectByType<CameraScript>().SetStartPosition(worldMax);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isTriggered && collision.collider.tag == "Player")
        {
            _isTriggered = true;
            if (_floorType == FloorType.Ground)
            {
                _gameManager.Lose();
                _playerMovement.Die();
                SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerDeath);
                Instantiate(_bloodSplatter, _playerMovement.transform.position, Quaternion.identity);
                return;
            }

            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._playerSplash);
            _playerMovement.Win();
            Instantiate(_waterSplash, _playerMovement.transform.position, Quaternion.identity);

            if (_floorType == FloorType.BossTrigger)
            {
                FindAnyObjectByType<BossfIntroManager>().OnLevelEnd();
                return;
            }
            _gameManager.Win();
            
        }
    }
}

public enum FloorType
{
    Water,
    Ground,
    BossTrigger,
}