using UnityEngine;

public class AchievementMilestones
{
    public string Title { get; }
    public bool WasUnlocked { get; set; }
    public bool WasShown { get; set; }

    public AchievementMilestones(string title)
    {
        Title = title;
    }
}
