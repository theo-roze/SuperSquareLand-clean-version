using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform[] _detectionLeftPoints;
    [SerializeField] private Transform[] _detectionRightPoints;
    [SerializeField] private float _detectionLength = 0.1f;
    [SerializeField] private LayerMask _WallLayerMask;

    public bool DetectWallsNearBy()
    {
        foreach (Transform detectionleftPoint in _detectionLeftPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionleftPoint.position,
                Vector2.left,
                _detectionLength,
                _WallLayerMask
                );
            if (hitResult.collider != null)
            {
                return true;
            }
        }
        foreach (Transform detectionRightPoint in _detectionRightPoints)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(
                detectionRightPoint.position,
                Vector2.right,
                _detectionLength,
                _WallLayerMask
                );
            if (hitResult.collider != null)
            {
                return true;
            }
        }
        return false;
    }
}