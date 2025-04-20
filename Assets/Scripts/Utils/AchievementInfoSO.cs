using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "Achievement Info", fileName = "New Achievement")]

public class AchievementInfoSO: ScriptableObject
{
    [SerializeField] int Id;
    [SerializeField] string Name;
    [TextArea(1, 4)]
    [SerializeField] string Description ;

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        return Name;
    }

    public string GetDescription()
    {
        return Description;
    }

}