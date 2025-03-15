using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{

    [FoldoutGroup("Parameters")][SerializeField] float _extraSliderMargin;

    [FoldoutGroup("References")][SerializeField] Slider _heightSlider;
    [FoldoutGroup("References")][SerializeField] Slider _distanceSlider;

    Transform _player;

    private void Awake()
    {
        _player = FindAnyObjectByType<PlayerMovement>().transform;
    }

    private void Start()
    {
        _heightSlider.maxValue = _player.position.y;
        _distanceSlider.maxValue = _player.position.x * (1 + _extraSliderMargin);
    }

    private void Update()
    {
        _heightSlider.value = _player.position.y;
        _distanceSlider.value = _player.position.x;
    }

}
