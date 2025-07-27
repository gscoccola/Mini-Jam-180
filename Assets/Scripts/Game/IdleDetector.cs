using UnityEngine;

public class IdleDetector : Singleton<IdleDetector>
{
    float idleTime = 0f;
    [SerializeField] float idleThreshold = 5f; 

    void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            idleTime = 0f;
        }
        else
        {
            idleTime += Time.deltaTime;
        }

        if (idleTime >= idleThreshold)
        {
            FindAnyObjectByType<GameManager>().TimeOutReset();
            idleTime = 0f; // Reset idle time after exceeding threshold
        }
    }
}
