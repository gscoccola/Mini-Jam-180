using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

// This Singleton class stores the global leaderboard information locally,to be displayed in the leaderboard canvas

public class LeaderboardManager : Singleton<LeaderboardManager>
{
    public List<PlayerLeaderboardEntry> HighScores;

    public void PopulateLeaderBoard(List<PlayerLeaderboardEntry> highScores)
    {
        HighScores = highScores;
        if (HighScores.Count > 0) { PersistentInfo.Instance.GlobalHighScore = HighScores[0].StatValue; }
        else PersistentInfo.Instance.GlobalHighScore = 0;
    }
}
