using System.Collections;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    bool _isOnCooldown;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player" && !_isOnCooldown)
        {
            collider.GetComponent<PlayerMovement>().Jump();
            StartCoroutine(IJumpCooldown());
        }
    }

    IEnumerator IJumpCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(1f);
        _isOnCooldown = false;
    }
}
