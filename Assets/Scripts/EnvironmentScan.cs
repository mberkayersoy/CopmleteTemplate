using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScan : MonoBehaviour
{
    public RaycastHitInfo highestHit;

    [Header("DEBUG VARIABLES")]
    [SerializeField] private bool showDebug;
    [SerializeField] private Color highestHitColor;
    [SerializeField] private Color aboveHitColor;
    [SerializeField] private float gizmosSphereRadius = 0.05f;

    [Space(5)]

    [Header("DEFAULT VARIABLES")]
    [SerializeField] private float rayLength = 5f;
    [SerializeField] private float rayHeightOffset = 0.1f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int numberOfRaycasts = 30; // Raycast count
    [SerializeField] private float maxDistanceToPlayer = 2f;
    [Range(0, 179)] [SerializeField] private float minSurfaceNormalAngle = 140f;
    [SerializeField] private Vector3 currentTargetPosition;
    public Vector3 CurrentTargetPosition { get => currentTargetPosition; set => currentTargetPosition = value; }

    [Space(5)]

    [Header("HEIGHT VARIABLES")]
    [SerializeField] private float maxStepUpHeight = 0.5f;
    [SerializeField] private float minStepUpHeight = 3.01f;
    [SerializeField] private float minVaultHeight = 0.51f;
    [SerializeField] private float maxVaultHeight = 1.2f;
    public RelevantAction relevantAction;

    private ThirdPersonCharacterController playerScript;
    private CharacterController controller;
    private float playerHeight;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerScript = GetComponent<ThirdPersonCharacterController>();
        relevantAction = RelevantAction.None;
    }
    private void Update()
    {
        CheckFront();
    }

    private void CheckFront()
    {
        highestHit = new RaycastHitInfo();
        Vector3 characterPosition = transform.position;
        Vector3 characterForward = transform.TransformDirection(Vector3.forward);
        playerHeight = transform.position.y + controller.height;

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector3 raycastOrigin = characterPosition + new Vector3(0, controller.stepOffset, 0) + new Vector3(0, i * rayHeightOffset, 0);

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, characterForward, out hit, rayLength, obstacleLayer))
            {
                // check hit angle
                float angleToCharacter = Vector3.Angle(hit.normal, characterForward);

                //Debug.Log("hit.poin.y : " + hit.point.y);
                //Debug.Log("playerHeight: " + playerHeight);
                // check conditions
                if (angleToCharacter >= minSurfaceNormalAngle && hit.point.y > highestHit.hitInfo.point.y &&
                    Vector3.Distance(hit.point, characterPosition + new Vector3(0, i * rayHeightOffset, 0)) <= maxDistanceToPlayer)
                {
                    RaycastHitInfo newInfo = new RaycastHitInfo(hit, Vector3.Distance(hit.point, characterPosition),
                        hit.point.y, angleToCharacter);

                    highestHit = newInfo;
                    Debug.DrawLine(raycastOrigin, raycastOrigin + characterForward * rayLength, Color.green);
                }
               else
                {
                    Debug.DrawLine(raycastOrigin, raycastOrigin + characterForward * rayLength, Color.red);
                }
            }
            else
            {
                // if there is no hit.
                Debug.DrawLine(raycastOrigin, raycastOrigin + characterForward * rayLength, Color.yellow);
            }
        }
        if (highestHit.hitInfo.collider != null)
        {
            CheckDepth(highestHit.hitInfo.point, highestHit.hitInfo.normal);
        }
        else
        {
            relevantAction = RelevantAction.None;
        }

    }

    public Vector3 GetTargetHangPosition()
    {
        Vector3 capsuleRadiusOffset = GetSurfaceNormalDirection(highestHit.hitInfo.normal) switch
        {
            SurfaceNormalDirection.Backward => new Vector3(0, 0, -controller.radius - controller.skinWidth),
            SurfaceNormalDirection.Right => new Vector3(controller.radius + controller.skinWidth, 0, 0),
            SurfaceNormalDirection.Left => new Vector3(-controller.radius - controller.skinWidth, 0, 0),
            _ => new Vector3(0, 0, controller.radius + controller.skinWidth),
        };
     
        currentTargetPosition = new Vector3(highestHit.hitInfo.point.x + capsuleRadiusOffset.x, highestHit.hitInfo.point.y - controller.height, highestHit.hitInfo.point.z + capsuleRadiusOffset.z); 
        return currentTargetPosition;
    }

    private RelevantAction CheckDepth(Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 origin = hitPoint + Vector3.up;
        float angleThreshHold = 25f;
        float defaultAngle = 85f;

        int raycastCounter = 0;

        for (int i = 0; i < 2; i++)
        {

            float angle = defaultAngle - i * angleThreshHold;

            Vector3 direction = GetSurfaceNormalDirection(-hitNormal) switch
            {
                SurfaceNormalDirection.Backward => Quaternion.Euler(-angle, 0, 0) * -hitNormal,
                SurfaceNormalDirection.Right => Quaternion.Euler(0, 0, -angle) * -hitNormal,
                SurfaceNormalDirection.Left => Quaternion.Euler(0, 0, angle) * -hitNormal,
                _ => Quaternion.Euler(angle, 0, 0) * -hitNormal,
            };

            /*
            - If both raycasts don't hit something, there's a high wall. 
            - If the first raycast hits above the player's height, there is only an edge. So player can hold the edge.
            - If two raycasts hits above the player's height, there is an area where the player can climb on top after holding on.
            - If the first raycast hits below the player's height, the player can move behind the obstacle.
            - If two raycasts hits below the player's height, the player can step up above the obstacle.
             */

            RaycastHit hit;
            if (Physics.Raycast(origin, direction, out hit, 1.5f, obstacleLayer))
            {
                Debug.DrawLine(origin, hit.point, Color.red);
                raycastCounter++;
            }
            else
            {
                Debug.DrawRay(origin, direction * 3f, Color.green);
            }
        }

        if (raycastCounter == 0)
        {
            //Debug.Log("WALL");
            return relevantAction = RelevantAction.None;
        }
        else if (hitPoint.y >= playerHeight && raycastCounter == 1 )
        {
            //Debug.Log("ONLY EDGE");
            return relevantAction = RelevantAction.Edge;
        }
        else if (hitPoint.y >= playerHeight && raycastCounter == 2)
        {
            //Debug.Log("EDGE AND AREA");
            return relevantAction = RelevantAction.Edge;
        }
        else if (hitPoint.y < playerHeight && raycastCounter == 1)
        {
            //Debug.Log("VAULT");
            currentTargetPosition = highestHit.hitInfo.point;
            return relevantAction = RelevantAction.Vault;
        }
        else if (hitPoint.y < playerHeight && raycastCounter == 2)
        {
            //Debug.Log("STEP UP");
            return relevantAction = RelevantAction.StepUp;
        }
        else
        {
            return relevantAction = RelevantAction.None;
        }

    }

    private SurfaceNormalDirection GetSurfaceNormalDirection(Vector3 hitNormal)
    {
        if (Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.z))
        {
            if (hitNormal.x > 0)
            {
                return SurfaceNormalDirection.Right;
            }
            else
            {
                return SurfaceNormalDirection.Left;
            }
        }
        else
        {
            if (hitNormal.z > 0)
            {
                return SurfaceNormalDirection.Forward;
            }
            else
            {
                return SurfaceNormalDirection.Backward;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = highestHitColor;
            Gizmos.DrawSphere(highestHit.hitInfo.point, gizmosSphereRadius);
        }
    }
    private enum SurfaceNormalDirection
    {
        Forward,
        Backward,
        Right,
        Left
    }
}

public enum RelevantAction
{
    Edge,
    Vault,
    StepUp,
    None
}



[System.Serializable]
public struct RaycastHitInfo
{
    public RaycastHit hitInfo;
    public float distanceToPlayer;
    public float hitHeight;
    public float angle;

    public RaycastHitInfo(RaycastHit hitInfo, float distanceToPlayer, float hitHeight, float angle)
    {
        this.hitInfo = hitInfo;
        this.distanceToPlayer = distanceToPlayer;
        this.hitHeight = hitHeight;
        this.angle = angle;
    }
}


