using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] ButtonType _type;
    GameManager _gameManager;
    float _menuClickDelay = 0.6f;
    bool _isBeingPressed;
    Animator _animator;
    [SerializeField] AudioClip _clip;

    private void Awake()
    {
        _gameManager = FindAnyObjectByType<GameManager>();
        _animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(ILoadScene());
        AudioSource.PlayClipAtPoint(_clip, Vector3.zero, 0.8f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    IEnumerator ILoadScene()
    {
        _animator.SetBool("isPressed", true);
        yield return new WaitForSeconds(_menuClickDelay);
        LoadScene();
    }


    private void LoadScene()
    {
        if (_type== ButtonType.Start)
        {
            _gameManager.LoadLevel(1);
        }
        else if (_type == ButtonType.Quit)
        {
            _gameManager.LoadLevel(0);
        }
        else if (_type == ButtonType.Restart)
        {
            _gameManager.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
        else if (_type == ButtonType.Next)
        {
            int i = SceneManager.GetActiveScene().buildIndex;
            i++;
            i = i % 4;
            _gameManager.LoadLevel(i);
        }
    }

}

public enum ButtonType
{
    Start,
    Quit,
    Next,
    Restart,
}
