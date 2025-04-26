public class LevelMileStones
{
    public bool WasShown { get; set; }
    public bool WasBeaten { get; set; }

    public LevelMileStones(bool wasShown = false, bool wasBeaten = false)
    {
        WasShown = wasShown;
        WasBeaten = wasBeaten;
    }
}
