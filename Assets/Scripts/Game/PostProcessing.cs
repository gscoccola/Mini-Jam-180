using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessing : Singleton<PostProcessing>
{
    public void Toggle()
    {
        GetComponent<Volume>().enabled = !GetComponent<Volume>().enabled;
    }
}
