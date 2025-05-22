using System.Collections;
using UnityEngine;

public class Padlock : MonoBehaviour
{
    Rigidbody2D _rigidBody;
    Animator _animator;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(IUnlock());
    }

    IEnumerator IUnlock()
    {
        _animator.SetTrigger("Open");

        yield return new WaitForSeconds(2f);


        _rigidBody.gravityScale = 50;
        _rigidBody.linearVelocity = new Vector3(-40f, 50f, 0f);
    }
}
