
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// This Singleton class stores all the persistent session information

public class PersistentInfo : Singleton<PersistentInfo>
{
    /* Fields to be saved to file */
    public Dictionary<int, LevelMileStones> LevelCompletionInfo;

    public Dictionary<int, AchievementMilestones> AchievementCompletionInfo;

    public int PersonalHighScore;

    public int PlayerId;

    public string DisplayName;


    /* other fields */

    public int GlobalHighScore;

    public int LastScore;

    public bool _isThereNewAchievement;

    public string LanguageCode = "es";

    public bool _loadTrailer = true;

    [SerializeField] private AchievementInfoSO[] _achievements;

    
    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        AchievementDictSetup();
        LevelDictSetup();
        LoadAllData();
        UnlockAllLevelsAndAchievements(true, true);
    }

    public void UnlockAllLevelsAndAchievements(bool levels = true, bool achievements = true)
     {
        AchievementCompletionInfo[1] = new AchievementMilestones(true, true);
        AchievementCompletionInfo[2] = new AchievementMilestones(true, true);
        AchievementCompletionInfo[3] = new AchievementMilestones(true, true);
        AchievementCompletionInfo[4] = new AchievementMilestones(true, true);
        for (int i = 0; i < LevelCompletionInfo.Count - 1; i++)
         {
            LevelCompletionInfo[i].WasShown = true;
            LevelCompletionInfo[i].WasBeaten = true;
         }
        LevelCompletionInfo[LevelCompletionInfo.Count - 1].WasShown = true;
        //_isThereNewAchievement = true;
        SaveCompletionData();
        Debug.Log("REMEMBER TO DELETE TEST");
    }

    private void PlayerIdSetup()
    {
        PlayerId = 0;
        PlayerPrefs.SetInt("PlayerId", PlayerId);
    }

    private void AchievementDictSetup()
    {
        AchievementCompletionInfo = new();
        foreach (AchievementInfoSO achievement in _achievements)
        {
            AchievementCompletionInfo[achievement.GetId()] = new AchievementMilestones(true, true);
        }
    }


    public void LevelDictSetup()
    {
        LevelCompletionInfo = new();
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        LevelCompletionInfo.Add(0, new LevelMileStones(true));
        for (int i = 1; i < sceneCount; i++)
        {
            LevelCompletionInfo[i] = new LevelMileStones(true);
        }
    }

    public void SaveCompletionData()
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

    public void SaveHighScoreData()
    {
        PlayerPrefs.SetInt("HighScore", PersonalHighScore);
    }

    public void SaveDisplayName()
    {
        PlayerPrefs.SetString("DisplayName", DisplayName);
    }


    private void LoadAllData()
    {
        if (PlayerPrefs.HasKey("PlayerId"))
        {
            PlayerId = PlayerPrefs.GetInt("PlayerId") + 1;
            PlayerPrefs.SetInt("PlayerId", PlayerId);
        }
        else
        {
            PlayerIdSetup();
            Debug.Log("No PlayerId data found, starting ID from 0");
        }

        PersonalHighScore = 0;
        DisplayName = "AAA";
    }

    public void ResetData()
    {
        _isThereNewAchievement = false;
        //PlayerPrefs.DeleteAll();
        //PlayerPrefs.DeleteKey("AchievementData");
        //PlayerPrefs.DeleteKey("LevelData");
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("DisplayName");
        //LevelDictSetup();
        //AchievementDictSetup();
        LoadAllData();
    }

    public int MaxLevelBeatenIndex()
    {
        if (LevelCompletionInfo.Where(level => level.Value.WasBeaten).Count() == 0) return 0;
        return LevelCompletionInfo.Where(level => level.Value.WasBeaten).Max(level => level.Key);
    }

    public int AchievementCount()
    {
        return AchievementCompletionInfo.Where(achievement => achievement.Value.WasUnlocked).Count();
    }

    public void CheckUnseenAchievements()
    {
        foreach (KeyValuePair<int, AchievementMilestones> achievementInfo in AchievementCompletionInfo)
        {
            if (achievementInfo.Value.WasUnlocked != achievementInfo.Value.WasShown)
            {
                _isThereNewAchievement = true;
                return;
            }
        }
        _isThereNewAchievement = false;
    }

    private bool BoolFromChar(char c) => c == '1' ? true : false;
    private char CharFromBool(bool boolean) => boolean ? '1' : '0';

} 

public enum Language
{
    English,
    Spanish,
}

