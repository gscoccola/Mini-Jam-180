using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class LevelButtonManager : MonoBehaviour
{
    [SerializeField] GameButton[] _gameButtons;


    private void Start()
    {
        var beatenLevels = PersistentInfo.Instance.LevelCompletionInfo
            .Where((levelinfo) => levelinfo.Value.WasBeaten);

        EnableButton(_gameButtons[0]);
        foreach (KeyValuePair<int, LevelMileStones> levelInfo in beatenLevels)
        {
            EnableButton(_gameButtons[levelInfo.Key]);
        }
    }

    void EnableButton(GameButton gameButton)
    {
        gameButton.gameObject.SetActive(true);
    }

}
