using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInfo : Singleton<PersistentInfo>
{
    public List<LevelInfo> LevelCompletionInfo = new List<LevelInfo>();
    public Dictionary<int, bool> AchievementCompletionInfo = new();

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
            AchievementCompletionInfo.Add(button.GetId(), false);
        }
    }

    private void LevelButtonSetup()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            LevelCompletionInfo.Add(new LevelInfo(i, false, false));
        }
    }
} 
