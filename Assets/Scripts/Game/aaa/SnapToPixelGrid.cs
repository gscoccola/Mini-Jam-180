using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// Snap the object to the pixel grid determined by the pixel perfect camera.
/// </summary>
public class SnapToPixelGrid : MonoBehaviour
{

    public PixelPerfectCamera ppc;

    private void LateUpdate()
    {
        ppc.transform.position = ppc.RoundToPixel(transform.position);
    }
}