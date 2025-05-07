using UnityEngine;

public class PropRandomPhase : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Bobbing", 0, Random.Range(0f, 1f));
    }
}
