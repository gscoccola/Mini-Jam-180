using TMPro;
using UnityEngine;

public class ScoresDisplay : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI _lastScore;
    [SerializeField] TextMeshProUGUI _personalHighScore;
    [SerializeField] TextMeshProUGUI _globalHighscore;

    private void OnEnable()
    {
        _lastScore.text = PersistentInfo.Instance.LastScore.ToString();
        _personalHighScore.text = PersistentInfo.Instance.PersonalHighScore.ToString();
        _globalHighscore.text = PersistentInfo.Instance.GlobalHighScore.ToString();
    }
}
