using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    TilemapCollider2D _tilemapCollider2D;

    private void Awake()
    {
        Spawner spawner = FindAnyObjectByType<Spawner>();
        if (spawner is not null) spawner._mapHeight = transform.TransformPoint(GetComponent<TilemapCollider2D>().bounds.max).y;
    }
}
