using System;
using UnityEngine;

public class UnlockAchievementEventArgs : EventArgs
{
    public int AchievementId { get; }

    public UnlockAchievementEventArgs(int achievementId)
    {
        AchievementId = achievementId;
    }

}
