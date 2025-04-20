public class LevelInfo
{
    public int Id { get; private set; }
    public bool HasBeenShown;
    public bool WasBeaten;

    public LevelInfo(int id, bool hasBeenShown, bool hasBeenBeaten)
    {
        Id = id;
        HasBeenShown = hasBeenShown;
        WasBeaten = hasBeenBeaten;
    }
}
