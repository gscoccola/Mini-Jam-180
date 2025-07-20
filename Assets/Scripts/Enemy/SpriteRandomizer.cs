using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    [SerializeField] Sprite[] _Sprites;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = _Sprites[Random.Range(0, _Sprites.Length)];
    }

}
