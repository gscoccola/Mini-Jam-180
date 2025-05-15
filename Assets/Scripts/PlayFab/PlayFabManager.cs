using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;

public class PlayfabManager : Singleton<PlayfabManager>
{
    private void Start()
    {
        Login();
    }

    private void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLogin, OnError);
    }

    public void SendLeaderboard(int score)
        {
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate> {
            new StatisticUpdate { StatisticName = "Highscore", Value = score }
            }
            },
            OnLeaderBoardUpdate, OnError);
        }



    private void GetStatistics()
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest()
            {
                MaxResultsCount = 10,
                StartPosition = 0,
                StatisticName = "Highscore",
            },
            response =>
            {
                if (response.Leaderboard != null)
                {
                    Dictionary<string, int> highscores = new Dictionary<string, int>();
                    LeaderboardManager.Instance.PopulateLeaderBoard(response.Leaderboard);
                    LeaderboardManager.Instance.DisplayLeaderBoard();
                    Debug.Log("Leaderboard loaded");
                }
                
                else Debug.Log("No leaderboard found");
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            }
            );
    }




    /*
    private void GetStatistics()
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

    private void UpdateDisplayName(string name)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        }, result => {
            Debug.Log("The player's display name is now: " + result.DisplayName);
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }

    private void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful highscore Sent");
    }

    private void OnLogin(LoginResult result)
    {
        Debug.Log("Successful Login");
        GetStatistics();
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
