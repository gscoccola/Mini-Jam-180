using System.Collections.Generic;
using UnityEngine;

public class DynamicBackground : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sky1;
    [SerializeField] SpriteRenderer _clouds1;
    [SerializeField] SpriteRenderer _mountains1;
    [SerializeField] SpriteRenderer _buildings1;
    [SerializeField] SpriteRenderer _bushes1;

    [SerializeField] SpriteRenderer _sky2;
    [SerializeField] SpriteRenderer _clouds2;
    [SerializeField] SpriteRenderer _mountains2;
    [SerializeField] SpriteRenderer _buildings2;
    [SerializeField] SpriteRenderer _bushes2;

    [SerializeField] List<BackgroundSetSO> backgroundSets;

    SpriteRenderer[] _activeBackground;
    SpriteRenderer[] _standByBackground;
    bool _isBackgroundFading = false;
    float _alpha = 1f;
    int currentSetIndex = 0;

    private void Awake()
    {
        _activeBackground = new SpriteRenderer[]{ _mountains1, _buildings1, _sky1, _clouds1, _bushes1};
        _standByBackground = new SpriteRenderer[] { _mountains2, _buildings2, _sky2, _clouds2, _bushes2 };
    }

    public void TransitionBackgrounds()
    {
        currentSetIndex++;
        currentSetIndex = currentSetIndex % backgroundSets.Count;
        _isBackgroundFading = true;
        _alpha = 1f;
        SwapSprites(currentSetIndex);
    }

    private void SwapSprites(int index)
    {
        _standByBackground[0].sprite = backgroundSets[index].MountainsSprite;
        _standByBackground[1].sprite = backgroundSets[index].BuidlingsSprite;
        _standByBackground[2].sprite = backgroundSets[index].SkySprite;
        _standByBackground[3].sprite = backgroundSets[index].CloudsSprite;
        _standByBackground[4].sprite = backgroundSets[index].BushesSprite;
    }

    private void SwapOrderInLayer()
    {
        foreach (var spriteRenderer in _activeBackground)
        {
            spriteRenderer.sortingOrder += 1;
        }
        foreach (var spriteRenderer in _standByBackground)
        {
            spriteRenderer.sortingOrder -= 1;
        }
    }

    private void Update()
    {
        if (!_isBackgroundFading) return;

        _alpha = Mathf.MoveTowards(_alpha, 0f, Time.deltaTime/1.5f);
        foreach (var spriteRenderer in _activeBackground)
        {     
            spriteRenderer.color = new Color(1, 1, 1, _alpha);
        }

        if (_alpha > Mathf.Epsilon) return;
        
        _isBackgroundFading = false;
        _alpha = 1f;
        foreach (var spriteRenderer in _activeBackground)
        {
            spriteRenderer.color = new Color(1, 1, 1, _alpha);
        }

        SpriteRenderer[] tempBackground  = _standByBackground;
        _standByBackground = _activeBackground;
        _activeBackground = tempBackground;

        SwapSprites(currentSetIndex);
        SwapOrderInLayer();
    }

}
