using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentChecker : MonoBehaviour
{
    [SerializeField] private Vector3 rayOffset = new Vector3(0, 0.2f, 0);
    [SerializeField] private float rayLength = 0.9f;
    [SerializeField] private float heightRayLength = 6f;
    [SerializeField] private LayerMask obstacleLayer;

    public ObstacleInfo CheckObstacle()
    {
        ObstacleInfo hitData = new ObstacleInfo();

        Vector3 rayOrigin = transform.position + rayOffset;

        hitData.hitFound = Physics.Raycast(rayOrigin, transform.forward, out hitData.hitInfo, rayLength, obstacleLayer);

        Debug.DrawRay(rayOrigin, transform.forward * rayLength, (hitData.hitFound) ? Color.red : Color.green);

        if (hitData.hitFound)
        {
            Vector3 heightOrigin = hitData.hitInfo.point + Vector3.up * heightRayLength;
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightInfo, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.blue : Color.green);
        }



        return hitData;
    }

}

public struct ObstacleInfo
{
    public bool hitFound;
    public bool heightHitFound;
    public RaycastHit hitInfo;
    public RaycastHit heightInfo;
}
