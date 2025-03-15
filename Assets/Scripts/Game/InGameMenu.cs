using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    GameManager _gameManager;
    [SerializeField] Button PlayAgainButton;
    [SerializeField] Button MainMenuButton;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        PlayAgainButton.onClick.AddListener(() => _gameManager.ReloadLevel());
        MainMenuButton.onClick.AddListener(() => _gameManager.LoadLevel(0));
    }
}
