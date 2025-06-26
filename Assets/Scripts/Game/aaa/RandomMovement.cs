using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomMovement : MonoBehaviour
{
    float angle = 0f;
    private void Update()
    {
        angle += Time.deltaTime * 2f; // Rotate at 30 degrees per second
        transform.position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 1f; // Move in a circle with radius 5
    }
}
