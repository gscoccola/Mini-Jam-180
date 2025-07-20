
using UnityEngine;


[CreateAssetMenu(menuName = "Background Set", fileName = "New Background")]

public class BackgroundSetSO : ScriptableObject
{
    public Sprite SkySprite;
    public Sprite CloudsSprite;
    public Sprite MountainsSprite;
    public Sprite BuidlingsSprite;
    public Sprite BushesSprite;
    public Color ContrastingColor;
    public Color BackgroundBossColor;
}
