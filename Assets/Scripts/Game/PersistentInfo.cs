using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.SocialPlatforms.Impl;

public class PersistentInfo : Singleton<PersistentInfo>
{
    public Dictionary<int, LevelMileStones> LevelCompletionInfo;

    public Dictionary<int, AchievementMilestones> AchievementCompletionInfo;
    public Dictionary<int, string> AchievementTitles;
    public bool _newAchievement;

    public string LanguageCode;

    [SerializeField] private AchievementInfoSO[] _achievements;

    public bool _wasLanguageSet;
    
    private void OnEnable()
    {
        Initialize();

    }

    public void Initialize()
    {
        AchievementDictSetup();
        LevelDictSetup();
        LoadData();
        TestChestDELETEAFTERTEST();
    }

    private void TestChestDELETEAFTERTEST()
    {
        
        foreach (AchievementInfoSO achievement in _achievements)
        {
            AchievementCompletionInfo[achievement.GetId()] = new AchievementMilestones(true, false);
        }
        Debug.Log("REMEMBER TO DELETE TEST");
        SaveData();
    }

    private void AchievementDictSetup()
    {
        AchievementCompletionInfo = new();
        foreach (AchievementInfoSO achievement in _achievements)
        {
            AchievementCompletionInfo[achievement.GetId()] = new AchievementMilestones();
        }
    }

    public void AchievementNameDictSetup()
    {
        AchievementTitles = new();
        foreach (AchievementInfoSO achievement in _achievements)
        {
            AchievementTitles[achievement.GetId()] = achievement.GetName();
        }
    }

    public void LevelDictSetup()
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
                if (s[1] != s[2]) _newAchievement = true;
            }
        }
        else    Debug.Log("No achievement data found");
    }

    public void ResetData()
    {
        _newAchievement = false;
        PlayerPrefs.DeleteAll();
        LevelDictSetup();
        AchievementDictSetup();
    }

    public void CheckUnseenAchievements()
    {
        foreach (KeyValuePair<int, AchievementMilestones> achievementInfo in AchievementCompletionInfo)
        {
            if (achievementInfo.Value.WasUnlocked != achievementInfo.Value.WasShown)
            {
                _newAchievement = true;
                return;
            }
        }
        _newAchievement = false;
    }

    private bool BoolFromChar(char c) => c == '1' ? true : false;
    private char CharFromBool(bool boolean) => boolean ? '1' : '0';

} 

public enum Language
{
    English,
    Spanish,
}

