using UnityEngine;
using Unity.Cinemachine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] float minScreenSpeed;
    [SerializeField] float maxDistanceFromEdge;

    private float _cameraEdgeX = 0f;
    PolygonCollider2D _polygonCollider2D;
    CinemachineConfiner2D _cameraConfiner;
    PlayerMovement _playerMovement;



    private void Awake()
    {
        _polygonCollider2D = FindAnyObjectByType<PolygonCollider2D>();
        _cameraConfiner = FindAnyObjectByType<CinemachineConfiner2D>();
        _playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        _cameraEdgeX = CameraEdgeX(_cameraEdgeX);
        UpdateBoundary(_cameraEdgeX);
    }

    private void UpdateBoundary(float xMax)
    {
        float yMax = 4000f;
        float yMin = -0f;
        float xMin = xMax - 100f;

        _polygonCollider2D.points = new Vector2[4]
        {
            new Vector2(xMin, yMin),
            new Vector2(xMax, yMin),
            new Vector2(xMax, yMax),
            new Vector2(xMin, yMax),
        };
        _cameraConfiner.InvalidateBoundingShapeCache();
        _playerMovement.CalculateBounds();
    }

    private float CameraEdgeX(float previousX)
    {
        return Mathf.Min(previousX - minScreenSpeed * Time.deltaTime, _playerMovement.transform.position.x + maxDistanceFromEdge);
    }
}
