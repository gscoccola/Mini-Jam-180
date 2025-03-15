using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeat : MonoBehaviour
{
    #region Parameters

    [Header("Heat")]
    [FoldoutGroup("Parameters")][SerializeField] float _maxHeat;
    [FoldoutGroup("Parameters")][SerializeField] float _heatGainRate;
    [FoldoutGroup("Parameters")][SerializeField] float _heatDissipateRate;


    #endregion

    #region Global Variables

    Rigidbody2D _rigidbody;
    float _currentHeat;
    GameManager _gameManager;
    [FoldoutGroup("References")][SerializeField] Slider _heatSlider;

    #endregion


    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _heatSlider.maxValue = _maxHeat;
    }

    private void Update()
    {
        _currentHeat += (_heatGainRate - (_heatDissipateRate * _rigidbody.linearVelocity.magnitude)) * Time.deltaTime;
        _heatSlider.value = _currentHeat;
        if (_currentHeat >= _maxHeat) _gameManager.GameOver();
    }
    
}
