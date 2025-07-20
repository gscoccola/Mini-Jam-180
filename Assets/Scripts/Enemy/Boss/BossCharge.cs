using UnityEngine;

public class BossCharge : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        SFXPlayer.Instance.PlayRandomClipFromArray(SFXPlayer.Instance._bossFistConnect);
    }
}
