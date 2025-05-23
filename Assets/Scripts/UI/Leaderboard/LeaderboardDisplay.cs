using System.Linq;
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
        SetProfileInfo();
        SetGlobalInfo();
    }

    private void SetGlobalInfo()
    {
        int i = 0;
        while (i < ScoreEntries.Length && i < LeaderboardManager.Instance.HighScores.Count)
        {
            ScoreEntries[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text
                = LeaderboardManager.Instance.HighScores[i].DisplayName.Substring(0, 3);

            ScoreEntries[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text
                = LeaderboardManager.Instance.HighScores[i].StatValue.ToString();
            i++;
        }
    }

    private void SetProfileInfo()
    {
        HighscoreSubmitter.SetName(PersistentInfo.Instance.DisplayName);
        HighscoreSubmitter.SetScore(PersistentInfo.Instance.PersonalHighScore);
        var playerEntries = LeaderboardManager.Instance.HighScores
            .Where(entry => entry.PlayFabId == PersistentInfo.Instance.PlayfabId).ToList();

        if (playerEntries.Count() > 0)
        {
            IsPlayerRanked = true;
            PlayerRank = playerEntries[0].Position;
            PersonalRank.text = (PlayerRank + 1).ToString();
            ScoreEntries[PlayerRank].transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = Color.green;
            ScoreEntries[PlayerRank].transform.GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.green;
        }
        else
        {
            PersonalRank.text = "???";
        }
    }

    public void UpdateNameInRanking()
    {
        if (IsPlayerRanked)
        {
            ScoreEntries[PlayerRank].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = HighscoreSubmitter.GetName();
        }   
    }

    public string GetName()
    {
        return HighscoreSubmitter.GetName();
    }
}
