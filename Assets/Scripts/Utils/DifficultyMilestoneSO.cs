using UnityEngine;


[CreateAssetMenu(menuName = "Difficulty Milestone", fileName = "New Difficulty Milestone")]

public class DifficultyMilestoneSO : ScriptableObject
{
    [SerializeField] float Distance;
    [SerializeField] float SpecialWaveChance;
    [SerializeField] int MissileNumber;
    [SerializeField] float MissileCooldown = 3f;
    [SerializeField] bool SpawnBuildings = true;
    [SerializeField] float MinSpacing = 4f;

    public float GetDistance()
    {
        return Distance;
    }
    public float GetSpecialWaveChance()
    {
        return SpecialWaveChance;
    }
    public int GetMissileNumber()
    {
        return MissileNumber;
    }
    public bool GetSpawnBuildings()
    {
        return SpawnBuildings;
    }

    public float GetMissileCooldown()
    {
        return MissileCooldown;
    }

    public float GetMinSpacing()
    {
        return MinSpacing;
    }

}
