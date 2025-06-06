using UnityEngine;

public class Fist : MonoBehaviour
{
    public bool _SFXAvailable;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_SFXAvailable)
        {
            _SFXAvailable = false;
            SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossFistConnect);
        }
            
    }
}
