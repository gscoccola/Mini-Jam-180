using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Floor : MonoBehaviour
{
    GameManager _gameManager;
    SFXPlayer _SFXPlayer;
    PlayerMovement _playerMovement;
    [SerializeField] FloorType _floorType;
    [SerializeField] GameObject _bloodSplatter;
    [SerializeField] GameObject _waterSplash;
    TilemapCollider2D _tilemapCollider2D;
    Tilemap _tilemap;
    bool _isTriggered;

    private void Awake()
    {
        _SFXPlayer = FindAnyObjectByType<SFXPlayer>();
        _gameManager = FindAnyObjectByType<GameManager>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        _tilemap = GetComponent<Tilemap>();
    }
    private void Start()
    {
        DefineBoundaries();
    }

    private void DefineBoundaries()
    {
        if (_floorType == FloorType.Ground) { return; }
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
                _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerDeath);
                Instantiate(_bloodSplatter, _playerMovement.transform.position, Quaternion.identity);
                return;
            }
            _gameManager.Win();
            _SFXPlayer.PlayClipFromArray(_SFXPlayer._playerSplash);
            _playerMovement.Win();
            Instantiate(_waterSplash, _playerMovement.transform.position, Quaternion.identity);
        }
    }
}

public enum FloorType
{
    Water,
    Ground,
}