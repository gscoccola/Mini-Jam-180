using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2.2f);
    }
}
