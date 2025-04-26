using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using System;

public class PersistentInfo : Singleton<PersistentInfo>
{
    public Dictionary<int, LevelMileStones> LevelCompletionInfo;

    public Dictionary<int, AchievementMilestones> AchievementCompletionInfo;
    public Dictionary<int, string> AchievementTitles;

    private void OnEnable()
    {
        LevelDictSetup();
        AchievementDictSetup();
        LoadData();
    }

    private void AchievementDictSetup()
    {
    AchievementCompletionInfo = new();
    AchievementTitles = new();
    AchievementButton[] achievementButtons = FindObjectsByType<AchievementButton>(FindObjectsSortMode.None);
        foreach (AchievementButton button in achievementButtons)
        {
            AchievementCompletionInfo[button.GetId()] = new AchievementMilestones();
            AchievementTitles[button.GetId()] = button.GetTitle();
        }
    }

    private void LevelDictSetup()
    {
        LevelCompletionInfo = new();
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        LevelCompletionInfo.Add(0, new LevelMileStones(true));
        for (int i = 1; i < sceneCount; i++)
        {
            LevelCompletionInfo[i] = new LevelMileStones();
        }
    }

    public void SaveData()
    {
        string levelData = "";
        foreach (KeyValuePair<int, LevelMileStones> levelInfo in LevelCompletionInfo)
        {
            levelData += levelInfo.Key.ToString() + CharFromBool(levelInfo.Value.WasShown) + CharFromBool(levelInfo.Value.WasBeaten) + ",";
        }
        PlayerPrefs.SetString("LevelData", levelData);
        
        string achievementData = "";
        foreach (KeyValuePair<int, AchievementMilestones> achievementInfo in AchievementCompletionInfo)
        {
            achievementData += achievementInfo.Key.ToString() + CharFromBool(achievementInfo.Value.WasUnlocked) +
                CharFromBool(achievementInfo.Value.WasShown) + ",";
        }
        PlayerPrefs.SetString("AchievementData", achievementData);
    }

    private void LoadData()
    {
        if ( PlayerPrefs.HasKey("LevelData"))
        {
            string[] levelData = PlayerPrefs.GetString("LevelData").Split(",", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in levelData)
            {
                LevelCompletionInfo[(int)Char.GetNumericValue(s[0])] = new LevelMileStones(BoolFromChar(s[1]), BoolFromChar(s[2]));
            }
        }

        if (PlayerPrefs.HasKey("AchievementData"))
        {
            string[] achievementData = PlayerPrefs.GetString("AchievementData").Split(",", System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in achievementData)
            {
                AchievementCompletionInfo[(int)Char.GetNumericValue(s[0])] = new AchievementMilestones(BoolFromChar(s[1]), BoolFromChar(s[2]));
            }
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        LevelDictSetup();
        AchievementDictSetup();
    }

    private bool BoolFromChar(char c) => c == '1' ? true : false;
    private char CharFromBool(bool boolean) => boolean ? '1' : '0';

} 


