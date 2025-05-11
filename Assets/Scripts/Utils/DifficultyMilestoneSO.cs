using UnityEngine;


[CreateAssetMenu(menuName = "Difficulty Milestone", fileName = "New Difficulty Milestone")]

public class DifficultyMilestoneSO : ScriptableObject
{
    [SerializeField] float Distance;
    [SerializeField] float SpecialWaveChance;
    [SerializeField] int MissileNumber;
    [SerializeField] bool SpawnBuildings = true;

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

}
