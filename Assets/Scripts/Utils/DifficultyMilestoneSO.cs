using UnityEngine;


[CreateAssetMenu(menuName = "Difficulty Milestone", fileName = "New Difficulty Milestone")]

public class DifficultyMilestoneSO : ScriptableObject
{
    [SerializeField] float Distance;
    [SerializeField] float SpecialWaveChance;
    [SerializeField] int MissileNumber;

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

}
