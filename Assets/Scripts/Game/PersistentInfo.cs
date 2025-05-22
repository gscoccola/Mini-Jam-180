
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

// This Singleton class stores all the information that is persistent throughout the session

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
        LoadAllData();
        
        /*TestChestDELETEAFTERTEST();*/
    }

    private void Start()
    {
        PlayfabManager.Instance.Login(PlayerId);
    }

    /* private void TestChestDELETEAFTERTEST()
     {

         foreach (AchievementInfoSO achievement in _achievements)
         {
             AchievementCompletionInfo[achievement.GetId()] = new AchievementMilestones(true, false);
         }
         Debug.Log("REMEMBER TO DELETE TEST");
         SaveData();
     }*/

    private void PlayerIdSetup()
    {
        PlayerId = UnityEngine.Random.Range(0, 2147483647);
        PlayerPrefs.SetInt("PlayerId", PlayerId);
    }

    private void AchievementDictSetup()
    {
        AchievementCompletionInfo = new();
        foreach (AchievementInfoSO achievement in _achievements)
        {
            AchievementCompletionInfo[achievement.GetId()] = new AchievementMilestones();
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

        if (PlayerPrefs.HasKey("PlayerId"))
        {
            PlayerId = PlayerPrefs.GetInt("PlayerId");
        }
        else
        {
            PlayerIdSetup();
            Debug.Log("No PlayerId data found, creating new ID");
            Debug.Log(PlayerId);
        }

        if (PlayerPrefs.HasKey("HighScore"))
        {
            PersonalHighScore = PlayerPrefs.GetInt("HighScore");
        }
        else PersonalHighScore = 0;

        if (PlayerPrefs.HasKey("DisplayName"))
        {
            DisplayName = PlayerPrefs.GetString("DisplayName");
        }
        else DisplayName = "AAA";
    }

    public void ResetData()
    {
        _newAchievement = false;
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("AchievementData");
        PlayerPrefs.DeleteKey("LevelData");
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.DeleteKey("DisplayName");
        LevelDictSetup();
        AchievementDictSetup();
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

