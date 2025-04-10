using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelButtonManager : MonoBehaviour
{
    [SerializeField] GameButton[] _gameButtons;


    private void Start()
    {
        var beatenLevels = PersistentInfo.Instance.LevelsInfo
            .Where((levelinfo) => levelinfo.HasBeenBeaten);

        EnableButton(_gameButtons[0]);
        for (int i = 0; i < beatenLevels.Count(); i++) 
        {
            EnableButton(_gameButtons[i + 1]);
        }
    }

    void EnableButton(GameButton gameButton)
    {
        gameButton.gameObject.SetActive(true);
    }

}
