using System.Collections.Generic;
using System.Linq;
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
            if (levelInfo.Key >= _gameButtons.Length) {
                Debug.LogError("More levels than buttons!");

                return; 
            }
            EnableButton(_gameButtons[levelInfo.Key]);

        }
    }

    void EnableButton(GameButton gameButton)
    {
        gameButton.gameObject.SetActive(true);
    }

}
