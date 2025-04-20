using System.Linq;
using UnityEngine;

public class LevelButtonManager : MonoBehaviour
{
    [SerializeField] GameButton[] _gameButtons;


    private void Start()
    {
        var beatenLevels = PersistentInfo.Instance.LevelCompletionInfo
            .Where((levelinfo) => levelinfo.WasBeaten);

        EnableButton(_gameButtons[0]);
        foreach (LevelInfo levelInfo in beatenLevels)
        {
            EnableButton(_gameButtons[levelInfo.Id]);
        }
    }

    void EnableButton(GameButton gameButton)
    {
        gameButton.gameObject.SetActive(true);
    }

}
