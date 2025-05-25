using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] float AttackDelay;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] GameObject Projectile;
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(IAttack());
    }

    IEnumerator IAttack()
    {
        _animator.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.2f);
        Instantiate(Projectile, transform.position + new Vector3(-xOffset, -yOffset, 0f), Quaternion.identity, transform);
        yield return new WaitForSeconds(AttackDelay);
        StartCoroutine(IAttack());
    }

}
