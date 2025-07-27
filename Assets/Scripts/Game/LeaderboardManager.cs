using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class LeaderboardManager : Singleton<LeaderboardManager>
{
    public List<PlayerLeaderboardEntry> HighScores;

    private void Start()
    {
        if (File.Exists(Application.dataPath + "/Leaderboard.csv"))
            HighScores = LoadFromCSV();
        else PopulateNewLeaderboard();
        SaveToCSV();
    }

    public void SendHighscore(int score)
    {
        HighScores.Add(new PlayerLeaderboardEntry
        {
            DisplayName = PersistentInfo.Instance.DisplayName,
            PlayerID = PersistentInfo.Instance.PlayerId,
            Score = score
        });
        SortLeaderboard();
        SaveToCSV();
    }
     
    public void UpdateDisplayName(string name)
    {
        if (HighScores.Where(score => score.PlayerID == PersistentInfo.Instance.PlayerId).Count() == 0) return;
        HighScores.Where(score => score.PlayerID == PersistentInfo.Instance.PlayerId).ToList()[0].DisplayName = name;
        SaveToCSV();
    }

    private void SortLeaderboard()
    {
        HighScores = HighScores.OrderByDescending(entry => entry.Score).ToList();
    }

    private void SaveToCSV()
    {
       string path = Application.dataPath + "/Leaderboard.csv";
         using (System.IO.StreamWriter file = new System.IO .StreamWriter(path))
         {
             foreach (var entry in HighScores)
             {
                 file.WriteLine($"{entry.PlayerID},{entry.DisplayName},{entry.Score}");
             }
        }
    }

    private List<PlayerLeaderboardEntry> LoadFromCSV()
    {
        List<PlayerLeaderboardEntry> highScores = new();
        string path = Application.dataPath + "/Leaderboard.csv";
        using (System.IO.StreamReader file = new System.IO.StreamReader(path))
        {
            List<string> lines = file.ReadToEnd().Split('\n').ToList();
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] parts = line.Split(',');
                if (parts.Length < 3) continue;
                PlayerLeaderboardEntry entry = new PlayerLeaderboardEntry
                {
                    PlayerID = int.Parse(parts[0]),
                    DisplayName = parts[1],
                    Score = int.Parse(parts[2])
                };
                highScores.Add(entry);
            }
            PersistentInfo.Instance.GlobalHighScore = highScores.Count > 0 ? highScores[0].Score : 0;
            return highScores;
        }
    }

    private void PopulateNewLeaderboard()
    {
        HighScores = new();
        for (int i = 0; i < 10; i++)
        {
            HighScores.Add(new PlayerLeaderboardEntry
            {
                DisplayName = "AAA",
                PlayerID = (-i),
                Score = 0
            });
        }
        SortLeaderboard();
    }
}



public class PlayerLeaderboardEntry
{
    public string DisplayName;

    public int PlayerID;

    public int Score;
}
