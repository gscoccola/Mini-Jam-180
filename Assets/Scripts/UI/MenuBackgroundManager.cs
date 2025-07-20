using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MenuBackgroundManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sky;
    [SerializeField] SpriteRenderer _clouds;
    [SerializeField] SpriteRenderer _mountains;
    [SerializeField] SpriteRenderer _buildings;
    [SerializeField] SpriteRenderer _bushes;

    [SerializeField] TextMeshProUGUI _creditsText;

    [SerializeField] List<BackgroundSetSO> backgroundSets;

    private void Start()
    {
        var maxLevelIndex = PersistentInfo.Instance.LevelCompletionInfo.Where(x => x.Value.WasShown).Max(x => x.Key);
        if (maxLevelIndex >= backgroundSets.Count) maxLevelIndex = backgroundSets.Count - 1;
        _mountains.sprite = backgroundSets[maxLevelIndex].MountainsSprite;
        _buildings.sprite = backgroundSets[maxLevelIndex].BuidlingsSprite;
        _sky.sprite = backgroundSets[maxLevelIndex].SkySprite;
        _clouds.sprite = backgroundSets[maxLevelIndex].CloudsSprite;
        _creditsText.color = backgroundSets[maxLevelIndex].ContrastingColor;
        if (_bushes != null) _bushes.sprite = backgroundSets[maxLevelIndex].BushesSprite;
    }
}
