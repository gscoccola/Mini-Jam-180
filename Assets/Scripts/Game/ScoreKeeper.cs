using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    Transform _playerTransform;
    int _score = 0;
    int _maxScore = 0;
    float _initialXPosition;
    TextMeshProUGUI _scoreDisplay;
    private bool _isEnabled = true;

    private void Awake()
    {
        _scoreDisplay = GetComponent<TextMeshProUGUI>();
        _playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Start()
    {
        _initialXPosition = _playerTransform.position.x;
    }

    private void Update()
    {
        if (!_isEnabled) return;
        _score = Mathf.FloorToInt(_initialXPosition - _playerTransform.position.x) * 100;
        if (_score > _maxScore)_maxScore = _score;
        _scoreDisplay.text = _maxScore.ToString();
    }

    public void Set(bool boolean)
    {
        _isEnabled = boolean;
    }
}
