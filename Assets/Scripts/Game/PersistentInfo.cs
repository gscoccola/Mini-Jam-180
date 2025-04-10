using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentInfo : Singleton<PersistentInfo>
{
    public Dictionary<int, LevelInfo> LevelsInfo = new Dictionary<int, LevelInfo>();

    private void Start()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            LevelsInfo.Add(i, new LevelInfo(false, false));
        }
    }
} 

public class LevelInfo
{
    public bool HasBeenShown;
    public bool HasBeenBeaten;

    public LevelInfo(bool hasBeenShown, bool hasBeenBeaten)
    {
        HasBeenShown = hasBeenShown;
        HasBeenBeaten = hasBeenBeaten;
    }
}
