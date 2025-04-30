using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

[CreateAssetMenu (menuName = "Achievement Info", fileName = "New Achievement")]

public class AchievementInfoSO: ScriptableObject
{
    [SerializeField] int Id;
    [SerializeField] string Name;
    [SerializeField] string NameEsp;
    [TextArea(1, 4)]
    [SerializeField] string Description ;
    [TextArea(1, 4)]
    [SerializeField] string DescriptionEsp ;

    public int GetId()
    {
        return Id;
    }

    public string GetName()
    {
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "es")
        {
            return NameEsp;
        }
        else
        {
            return Name;
        }
    }

    public string GetDescription()
    {
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "es")
        {
            return DescriptionEsp;
        }
        else
        {
            return Description;
        }
    }

}