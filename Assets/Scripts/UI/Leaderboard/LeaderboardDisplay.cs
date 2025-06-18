using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class LeaderboardDisplay : MonoBehaviour
{
    [SerializeField] HighscoreSubmitter HighscoreSubmitter;
    [SerializeField] TextMeshProUGUI PersonalRank;
    [SerializeField] GameObject[] ScoreEntries;

    private bool IsPlayerRanked;
    private int PlayerRank;

    private void OnEnable()
    {
        MergeLocalAndGlobalData();
        PopulateProfileInfo();
    }

    private void MergeLocalAndGlobalData()
    {
        PlayerRank = LeaderboardManager.Instance.HighScores.Count;
        string playerId = PersistentInfo.Instance.PlayfabId;
        int playerScore = PersistentInfo.Instance.PersonalHighScore;
        string playerName = PersistentInfo.Instance.DisplayName;

        int i = 0;
        int j = 0;
        bool wasPlayerShown = false;

        while (i < ScoreEntries.Length && j < LeaderboardManager.Instance.HighScores.Count)
        {
            if (LeaderboardManager.Instance.HighScores[j].PlayFabId == playerId)
            {
                j++;
                continue;
            }

            if (!wasPlayerShown && playerScore >= LeaderboardManager.Instance.HighScores[j].StatValue)
            {
                IsPlayerRanked = true;
                wasPlayerShown = true;
                PopulateLeaderboardEntry(i, playerName, playerScore);
                PlayerRank = i;
                i++;
                continue;
            }

            PopulateLeaderboardEntry(i, 
                LeaderboardManager.Instance.HighScores[j].DisplayName,
                LeaderboardManager.Instance.HighScores[j].StatValue);
            i++;
            j++;
        }

        if (wasPlayerShown) return;

        IsPlayerRanked = false;

        while (j < LeaderboardManager.Instance.HighScores.Count)
        {
            if (LeaderboardManager.Instance.HighScores[j].PlayFabId == playerId)
            {
                j++;
                continue;
            }

            if (playerScore >= LeaderboardManager.Instance.HighScores[j].StatValue)
            {
                PlayerRank = i;
                return;
            }

            i++;
            j++;
        }

    }

    private void PopulateLeaderboardEntry(int i, string displayName, int score)
    {
        ScoreEntries[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = displayName != null
                ? displayName.Substring(0, 3)
                : "???";

        ScoreEntries[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text
            = score.ToString();
    }

    private void PopulateProfileInfo()
    {
        HighscoreSubmitter.SetName(PersistentInfo.Instance.DisplayName);
        HighscoreSubmitter.SetScore(PersistentInfo.Instance.PersonalHighScore);
        PersonalRank.text = (PlayerRank + 1).ToString();

        if (IsPlayerRanked)
        {
            ScoreEntries[PlayerRank].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.green;
            ScoreEntries[PlayerRank].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.green;
        }
    }

    public void UpdateNameInRanking()
    {
        if (IsPlayerRanked)
        {
            ScoreEntries[PlayerRank].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = HighscoreSubmitter.GetName().Substring(0, 3);
        }   
    }

    public string GetName()
    {
        return HighscoreSubmitter.GetName();
    }
}


public class HighscoreEntry
{
    public string PlayFabId;
    public string DisplayName;
    public int Score;

    public HighscoreEntry(string playFabId, string displayName, int score)
    {
        PlayFabId = playFabId;
        DisplayName = displayName;
        Score = score;
    }
}