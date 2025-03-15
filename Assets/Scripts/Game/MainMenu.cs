using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    GameManager _gameManager;
    [SerializeField] Button StartButton;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
    }

    void Start()
    {
        StartButton.onClick.AddListener(() => _gameManager.LoadLevel(1));
    }

}
