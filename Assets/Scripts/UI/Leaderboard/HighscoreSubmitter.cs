using TMPro;
using UnityEngine;

public class HighscoreSubmitter : MonoBehaviour
{
    [SerializeField] SelectableCharacter[] Characters;
    [SerializeField] TextMeshProUGUI CurrentScore;

    public string GetName()
    {
        string name = "";
        foreach (var character in Characters)
        {
            name += character.Character.ToString();
        }
        return name + PersistentInfo.Instance.CustomPlayerId.ToString();
    }

    public void SetName(string name)
    {
        Characters[0].SetChar(name[0]);
        Characters[1].SetChar(name[1]);
        Characters[2].SetChar(name[2]);
    }

    public void SetScore(int score)
    {
        CurrentScore.text = score.ToString();
    }
}
