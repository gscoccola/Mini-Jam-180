using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer _sky;
    [SerializeField] SpriteRenderer _clouds;
    [SerializeField] SpriteRenderer _mountains;
    [SerializeField] SpriteRenderer _buildings;
    [SerializeField] SpriteRenderer _bushes;


    [SerializeField] List<BackgroundSetSO> backgroundSets;

    private void Start()
    {
        var maxLevelIndex = PersistentInfo.Instance.LevelCompletionInfo.Where(x => x.Value.WasShown).Max(x => x.Key);
        if (backgroundSets[maxLevelIndex] is null) return;
        _mountains.sprite = backgroundSets[maxLevelIndex].MountainsSprite;
        _buildings.sprite = backgroundSets[maxLevelIndex].BuidlingsSprite;
        _sky.sprite = backgroundSets[maxLevelIndex].SkySprite;
        _clouds.sprite = backgroundSets[maxLevelIndex].CloudsSprite;
        if (_bushes != null) _bushes.sprite = backgroundSets[maxLevelIndex].BushesSprite;
    }
}
