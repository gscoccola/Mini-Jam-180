using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

// This Singleton class stores the global leaderboard information locally,to be displayed in the leaderboard canvas

public class LeaderboardManager : Singleton<LeaderboardManager>
{
    private List<PlayerLeaderboardEntry> HighScores;

    public void PopulateLeaderBoard(List<PlayerLeaderboardEntry> highScores)
    {
        HighScores = highScores;
    }

    public void DisplayLeaderBoard()
    {
        foreach (var entry in HighScores)
        {
            //Debug.Log($"Name: {entry.DisplayName} , Score: {entry.StatValue}");
        }
    }
}
