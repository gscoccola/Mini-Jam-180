using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    private void Awake()
    {
        Spawner spawner = FindAnyObjectByType<Spawner>();
        if (spawner is not null) spawner._mapHeight = GetComponent<TilemapCollider2D>().bounds.max.y;
    }
}
