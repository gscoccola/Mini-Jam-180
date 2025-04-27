using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    Transform _playerTransform;
    int _score = 0;
    float _initialXPosition;
    TextMeshProUGUI _scoreDisplay;


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
        _score = Mathf.FloorToInt(_initialXPosition - _playerTransform.position.x);
        _scoreDisplay.text = "Score " + _score.ToString();
    }
}
