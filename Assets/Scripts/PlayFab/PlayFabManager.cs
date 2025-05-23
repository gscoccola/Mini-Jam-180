using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using PlayFab.AuthenticationModels;
using UnityEngine;
using System;

// This Singleton class handles all the requests to the playfab server

public class PlayfabManager : Singleton<PlayfabManager>
{

    public void Login(int playerID)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = playerID.ToString(),
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLogin, OnError);
    }

    /*private static Task<PlayFabAuthenticationContext> LoginAsPlayer(string customId)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        PlayFabResult<LoginResult> loginResult = PlayFabClientAPI.LoginWithCustomID(request, null, null);

        return loginResult.Result.AuthenticationContext;
    }*/

    /*private static async Task<PlayFabAuthenticationContext> LoginAsPlayer(string customId = "GettingStartedGuide")
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest { CustomId = customId, CreateAccount = true };

        PlayFabResult<LoginResult> loginResult = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

        return loginResult.Result.AuthenticationContext; 
    }*/

    public void SendCompletionStatistics()
    {
        SendMaximumLevelReached();
        SendAchievementCount();
    }

    public void SendHighscore(int score)
    {
        SendStatistic("Highscore", score);
    }

    private void SendMaximumLevelReached()
    {
        SendStatistic("Maximum Level Reached", PersistentInfo.Instance.MaxLevelBeatenIndex());
    }

    private void SendAchievementCount()
    {
        SendStatistic("Achievements Found", PersistentInfo.Instance.AchievementCount());
    }

    public void SendMobile()
    {
        if (Application.isMobilePlatform) SendStatistic("Is Device Mobile", 1);
        else SendStatistic("Is Device Mobile", 0);
    }

    private void SendStatistic(string name, int value)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = name, Value = value }
            }
        },
        OnStatisticUpdated, OnError);
    }



    public void GetGlobalStatistics(bool openLeaderboardWhenDone, int maxResultsCount)
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest()
            {
                MaxResultsCount = maxResultsCount,
                StartPosition = 0,
                StatisticName = "Highscore",
            },
            response =>
            {
                if (response.Leaderboard != null)
                {
                    LeaderboardManager.Instance.PopulateLeaderBoard(response.Leaderboard);
                    if (openLeaderboardWhenDone) FindAnyObjectByType<GameManager>().OpenLeaderBoard();
                    Debug.Log("Leaderboard loaded");
                }

                else
                {
                    if (openLeaderboardWhenDone) FindAnyObjectByType<GameManager>().CancelOpenLeaderBoard();
                    Debug.Log("No leaderboard found");
                }
                    
            },
            error =>
            {
                if (openLeaderboardWhenDone) FindAnyObjectByType<GameManager>().CancelOpenLeaderBoard();
                Debug.Log(error.GenerateErrorReport());
            }
            );
    }




    
    /*private void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    private void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Dictionary<string, int> highScores;
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName != "Highscore") return;
            highScores[eachStat.]
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
        }    
    }*/

    public void UpdateDisplayName(string name)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnStatisticUpdated(UpdatePlayerStatisticsResult result)
    {
        //Debug.Log("Statistic Updated");
    }

    private void OnLogin(LoginResult result)
    {
        Debug.Log("Successful Login as " + PersistentInfo.Instance.CustomPlayerId);
        SendMobile();
        GetGlobalStatistics(false, 1);
        SendCompletionStatistics();

        PersistentInfo.Instance.PlayfabId = result.PlayFabId;
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
