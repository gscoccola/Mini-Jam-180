using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementButtonManager : MonoBehaviour
{
    Dictionary<int, AchievementButton> _achievementButtonsById = new();
    [SerializeField] GameButton _backFromAchievements;

    private void Awake()
    {
        AchievementButton[] achievementButtons = FindObjectsByType<AchievementButton>(FindObjectsSortMode.None);
        foreach (AchievementButton button in achievementButtons)
        {
            _achievementButtonsById.Add(button.GetId(), button);
            button.PopupClose += CloseAllPopups;
        }
        _backFromAchievements.PopupClose += CloseAllPopups;
    }

    private void Start()
    {
        var unlockedAchievements = PersistentInfo.Instance.AchievementCompletionInfo
            .Where((achievementInfo) => achievementInfo.Value)
            .Select((achievementInfo) => achievementInfo.Key);

        foreach (int achievementId in unlockedAchievements)
        {
            _achievementButtonsById[achievementId].Unlock();
        }
    }

    public void CloseAllPopups(object sender, PopupEventHandler eventArgs)
    {
        foreach (var achievementButton in _achievementButtonsById.Values)
        {
            achievementButton.SetPopup(false);
        }
    }

}
