using TMPro;
using UnityEngine;

public class AnimationDebugger : MonoBehaviour
{
    [SerializeField] Animator animator;
    TextMeshProUGUI TextMeshProUGUI;

    private void Awake()
    {
        TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        TextMeshProUGUI.text =  animator.GetBool("_isFalling").ToString();
    }
}
