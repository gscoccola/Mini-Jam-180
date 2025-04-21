using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInfo : Singleton<PersistentInfo>
{
    public Dictionary<int, LevelMileStones> LevelCompletionInfo = new();
    public Dictionary<int, AchievementMilestones> AchievementCompletionInfo = new();

    private void OnEnable()
    {
        LevelButtonSetup();
        AchievementButtonSetup();
    }

    private void AchievementButtonSetup()
    {
        AchievementButton[] achievementButtons = FindObjectsByType<AchievementButton>(FindObjectsSortMode.None);
        foreach (AchievementButton button in achievementButtons)
        {
            AchievementCompletionInfo.Add(button.GetId(), new AchievementMilestones());
        }
    }

    private void LevelButtonSetup()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            LevelCompletionInfo.Add(i, new LevelMileStones());
        }
    }

    public void UnlockAchievement(int achievementId)
    {
        if (AchievementCompletionInfo.ContainsKey(achievementId))
        {
            AchievementCompletionInfo[achievementId].WasUnlocked = true;
        }
    }

} 


