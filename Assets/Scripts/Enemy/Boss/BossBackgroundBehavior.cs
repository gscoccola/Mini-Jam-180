using System.Collections;
using UnityEngine;

public class BossBackgroundBehavior : MonoBehaviour
{
    Vector3 _initialPos;
    float _initialX;
    float _initialY;
    Camera _camera;
    bool _active;
    public bool _isExiting;
    [SerializeField] float displacementRatio;
    [SerializeField] float _cigarSpeed;
    [SerializeField] float _cigarDelay;
    [SerializeField] GameObject[] cigars;

    private void Awake()
    {
        _camera = FindAnyObjectByType<Camera>();
        _initialPos = transform.position;
        _initialY = FindAnyObjectByType<PlayerMovement>().transform.position.y;
    }

    private void Update()
    {
        foreach (GameObject cigar in cigars)
        {
            cigar.transform.position += _cigarSpeed * Vector3.up * Time.deltaTime;
        }

        if (!_active) return;
        if (_isExiting)
        {
            transform.position += Vector3.up * Time.deltaTime * 8f;
            return;
        }
        transform.position = _initialPos + (1f - displacementRatio) *
            (_camera.transform.position.Flatten() - _initialX * Vector3.right - _initialY * Vector3.up);

        
    }

    public void Activate()
    {
        _active = true;
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().enabled = true;
        _initialX = _camera.transform.position.x;
    }

    public IEnumerator IAttack()
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossCigarAttack);
        transform.GetChild(0).GetComponent<Animator>().SetTrigger("Spit");
        foreach (GameObject cigar in cigars)
        {
            cigar.transform.position = transform.position;
            yield return new WaitForSeconds(_cigarDelay);
        }
    }
}
