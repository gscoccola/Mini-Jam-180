using UnityEngine;

public class AchievementMilestones
{
    public bool WasUnlocked { get; set; }
    public bool WasShown { get; set; }

    public AchievementMilestones(bool wasUnlocked = false, bool wasShown = false)
    {
        WasUnlocked = wasUnlocked;
        WasShown = wasShown;
    }
}
