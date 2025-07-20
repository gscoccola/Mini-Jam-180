using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PoolCover : MonoBehaviour
{
    [SerializeField] private GameObject _keyboardInput;
    [SerializeField] private GameObject _mobileInput;
    [SerializeField] private GameObject _gamepadInput;
    [SerializeField] private GameObject _coverChunk;
    [SerializeField] private float _torque;

    private void Awake()
    {
        if (!PersistentInfo.Instance.LevelCompletionInfo[SceneManager.GetActiveScene().buildIndex].WasShown)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (Application.isMobilePlatform) _mobileInput.SetActive(true);
        else if (Gamepad.current != null) _gamepadInput.SetActive(true);
        else _keyboardInput.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player" &&
            collision.collider.GetComponent<PlayerMovement>()._isDiving)
        {
            collision.collider.GetComponent<Rigidbody2D>().linearVelocityY = 7f;
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossFistConnect);
            OnPlayerDive();
        }
        else SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossThrow);
    }

    private void OnPlayerDive()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        Instantiate(_coverChunk, transform.position + 1.875f * Vector3.left, Quaternion.identity, transform).GetComponent<Rigidbody2D>().AddTorque(_torque);
        Instantiate(_coverChunk, transform.position + 1.875f * Vector3.right, Quaternion.identity, transform).GetComponent<Rigidbody2D>().AddTorque(-_torque);
    }

}
