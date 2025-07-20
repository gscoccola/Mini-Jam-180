using System.Collections;
using DG.Tweening;
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
    [SerializeField] bool _isEndless;
    Vector3 _lastCamPos;
    [SerializeField] float _followVec;

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

        if (_isEndless)
        {
            transform.position += (1f - displacementRatio) * (_camera.transform.position.Flatten() - _lastCamPos.Flatten())
                + Mathf.Sign(_camera.transform.position.x - transform.position.x) * Vector3.right * _followVec * Time.deltaTime;

            _lastCamPos = _camera.transform.position.Flatten();
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
        if (_isEndless)
        {
            transform.position = (_camera.transform.position.x - 30f) * Vector3.right + _initialY * Vector3.up;
            _lastCamPos = _camera.transform.position.Flatten();
            return;
        }
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

    public void ChangeColor(Color color)
    {
        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().DOColor(color, 1.5f);
        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().DOColor(color, 1.5f);
        transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().DOColor(color, 1.5f);

        foreach (GameObject cigar in cigars)
        {
            cigar.GetComponent<SpriteRenderer>().DOColor(color, 1.5f);
        }
    }
}
