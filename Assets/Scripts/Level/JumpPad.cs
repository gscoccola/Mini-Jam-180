using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    bool _isOnCooldown;
    [SerializeField] float _boostAmount;
    [SerializeField] bool _isEpilogue;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" && !_isOnCooldown)
        {
            collider.GetComponent<PlayerMovement>().Jump();
            StartCoroutine(IJumpCooldown());
            Rigidbody2D rigidbody2D = collider.transform.GetComponent<Rigidbody2D>();
            rigidbody2D.linearVelocityX = Mathf.Sign(rigidbody2D.linearVelocityX) * _boostAmount;
            if (_isEpilogue)
            {
                collider.GetComponent<PlayerMovement>().IsHorizonalInputDisabled = true;
            }
        }
    }

    IEnumerator IJumpCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(1f);
        _isOnCooldown = false;
    }
}
