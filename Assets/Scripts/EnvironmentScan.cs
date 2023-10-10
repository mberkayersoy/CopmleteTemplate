using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScan : MonoBehaviour
{
    public RaycastHitInfo highestLedge;
    public RelevantAction relevantAction;

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
    public SurfaceNormalDirection currentSurfaceNormalDirection;

    private ThirdPersonCharacterController playerScript;
    private CharacterController controller;
    private float playerHeight;

    private List<RaycastHit> rayHitList;

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

    private void HangScan()
    {
        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector3 raycastOrigin = currentTargetPosition + new Vector3(0, controller.stepOffset, 0) + new Vector3(0, i * rayHeightOffset, 0);
        }
    }
    private void CheckFront()
    {
        int highestHitIndex = 0;
        rayHitList = new List<RaycastHit>();
        highestLedge = new RaycastHitInfo();
        Vector3 characterPosition = transform.position;
        Vector3 characterForward = transform.TransformDirection(Vector3.forward);
        playerHeight = transform.position.y + controller.height;

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector3 raycastOrigin = characterPosition + new Vector3(0, controller.height, 0) + new Vector3(0, i * rayHeightOffset, 0);

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, characterForward, out hit, rayLength, obstacleLayer))
            {
                // check hit angle
                float angleToCharacter = Vector3.Angle(hit.normal, characterForward);

                // check conditions
                if (angleToCharacter >= minSurfaceNormalAngle && hit.point.y > highestLedge.hitInfo.point.y) //&&
                     //hit.distance <= highestLedge.hitInfo.distance)// Vector3.Distance(hit.point, characterPosition + new Vector3(0, i * rayHeightOffset, 0)) <= maxDistanceToPlayer)
                {
                    RaycastHitInfo newLedge = new RaycastHitInfo(hit, Vector3.Distance(hit.point, characterPosition),
                        hit.point.y, angleToCharacter);
                    highestHitIndex = i;
                    highestLedge = newLedge;

                    //rayHits.Add(i, hit);
                    rayHitList.Add(hit);

                    Debug.DrawLine(raycastOrigin, hit.point, Color.green);
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
                Debug.Log("No hit : " + hit);
                //rayHits.Add(i, hit);
                rayHitList.Add(hit);
            }
        }
        CheckLedge();
        //CheckDepth(highestHit.hitInfo.point, highestHit.hitInfo.normal);
        //if (highestHitIndex != numberOfRaycasts - 1)
        //{
        //    CheckDepth(highestHit.hitInfo.point, highestHit.hitInfo.normal);
        //}
        //else
        //{
        //    relevantAction = RelevantAction.None;
        //}
    }

    public void CheckLedge()
    {
        for (int i = rayHitList.Count - 1; i < rayHitList.Count; i++)
        {
            // Ledge
            if (rayHitList[i].distance == 0f && rayHitList[i - 1].distance != 0f)
            {
                Debug.Log("IF");
                relevantAction = RelevantAction.Edge;
                RaycastHitInfo newLedge = new RaycastHitInfo(rayHitList[i - 1], rayHitList[i - 1].distance,
                       rayHitList[i - 1].point.y, Vector3.Angle(rayHitList[i - 1].normal, transform.TransformDirection(Vector3.forward)));
                highestLedge = newLedge;
                GetSurfaceNormalDirection(rayHitList[i - 1].normal);
            }
            //Ledge
            else if (rayHitList[i].distance != 0 && rayHitList[i - 1].distance != 0 && rayHitList[i].distance - rayHitList[i - 1].distance >= 0.1f) // 0.1f is ledge off set.
            {
                Debug.Log("ELSE IF");
                RaycastHitInfo newLedge = new RaycastHitInfo(rayHitList[i - 1], rayHitList[i - 1].distance,
                    rayHitList[i - 1].point.y, Vector3.Angle(rayHitList[i - 1].normal, transform.TransformDirection(Vector3.forward)));
                highestLedge = newLedge;
                relevantAction = RelevantAction.Edge;
                GetSurfaceNormalDirection(rayHitList[i - 1].normal);
            }
            else
            {
                relevantAction = RelevantAction.None;
            }

            
        }
    }

    public Vector3 GetTargetHangPosition()
    {
        Vector3 capsuleRadiusOffset = GetSurfaceNormalDirection(highestLedge.hitInfo.normal) switch
        {
            SurfaceNormalDirection.Backward => new Vector3(0, 0, -controller.radius - controller.skinWidth),
            SurfaceNormalDirection.Right => new Vector3(controller.radius + controller.skinWidth, 0, 0),
            SurfaceNormalDirection.Left => new Vector3(-controller.radius - controller.skinWidth, 0, 0),
            _ => new Vector3(0, 0, controller.radius + controller.skinWidth),
        };
     
        currentTargetPosition = new Vector3(highestLedge.hitInfo.point.x + capsuleRadiusOffset.x, highestLedge.hitInfo.point.y - controller.height, highestLedge.hitInfo.point.z + capsuleRadiusOffset.z); 
        return currentTargetPosition;
    }

    private RelevantAction CheckDepth(Vector3 hitPoint, Vector3 hitNormal)
    {
        Vector3 origin = hitPoint + Vector3.up / 2;
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
            Debug.Log("WALL");
            return relevantAction = RelevantAction.None;
        }
        else if (hitPoint.y >= playerHeight && raycastCounter == 1 )
        {
            Debug.Log("ONLY EDGE");
            return relevantAction = RelevantAction.Edge;
        }
        else if (hitPoint.y >= playerHeight && raycastCounter == 2)
        {
            Debug.Log("EDGE AND AREA");
            return relevantAction = RelevantAction.Edge;
        }
        else if (hitPoint.y < playerHeight && raycastCounter == 1)
        {
            //Debug.Log("VAULT");
            currentTargetPosition = highestLedge.hitInfo.point;
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
                currentSurfaceNormalDirection = SurfaceNormalDirection.Right;
                return SurfaceNormalDirection.Right;
            }
            else
            {
                currentSurfaceNormalDirection = SurfaceNormalDirection.Left;
                return SurfaceNormalDirection.Left;
            }
        }
        else
        {
            if (hitNormal.z > 0)
            {
                currentSurfaceNormalDirection = SurfaceNormalDirection.Forward;
                return SurfaceNormalDirection.Forward;
            }
            else
            {
                currentSurfaceNormalDirection = SurfaceNormalDirection.Backward;
                return SurfaceNormalDirection.Backward;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebug)
        {
            Gizmos.color = highestHitColor;
            Gizmos.DrawSphere(highestLedge.hitInfo.point, gizmosSphereRadius);
        }
    }
    public enum SurfaceNormalDirection
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
    None,
    ClimbUp
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


