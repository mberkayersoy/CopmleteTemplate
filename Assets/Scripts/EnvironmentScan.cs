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
    [SerializeField] private float rayOffset = 0.05f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int numberOfRaycasts = 10; // Raycast count
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
    public Vector3 currentSurfaceNormal;

    private ThirdPersonCharacterController playerScript;
    private CharacterController controller;
    private float playerHeight;

    private List<RaycastHit> rayHitList;
    private List<RaycastHit> sideRayHitList;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerScript = GetComponent<ThirdPersonCharacterController>();
        relevantAction = RelevantAction.None;
    }

    private void Update()
    {
        CheckFront();
        CheckLedgeBounds();
        CheckWall();
    }

    public bool CheckWall()
    {
        if (highestLedge.hitInfo.distance == 0) return false;
        Vector3 characterPosition = transform.position;
        Vector3 characterForward = transform.forward;


        Vector3 raycastOrigin = characterPosition  + new Vector3(0, 9 * rayOffset, 0);
        if (Physics.Raycast(raycastOrigin, characterForward, rayLength * 1.2f, obstacleLayer))
        {
            Debug.DrawRay(raycastOrigin, characterForward * rayLength * 1.2f, Color.green);
            return true;
        }
        else
        {
            Debug.DrawRay(raycastOrigin, characterForward * rayLength * 1.2f, Color.red);
            return false;
        }
        
    }
    private void CheckFront()
    {
        rayHitList = new List<RaycastHit>();
        highestLedge = new RaycastHitInfo();
        Vector3 characterPosition = transform.position;
        Vector3 characterForward = transform.forward;
        playerHeight = transform.position.y + controller.height;

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector3 raycastOrigin = characterPosition + new Vector3(0, controller.height, 0) + new Vector3(0, i * rayOffset, 0);

            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, characterForward, out hit, rayLength, obstacleLayer))
            {
                // check hit angle
                rayHitList.Add(hit);
                Debug.DrawLine(raycastOrigin, hit.point, Color.green);
            }
            else
            {
                // if there is no hit.
                Debug.DrawLine(raycastOrigin, raycastOrigin + characterForward * rayLength, Color.red);
                rayHitList.Add(hit);
            }
        }
        CheckLedge();
    }


    public Vector2 CheckLedgeBounds()
    {
        float ledgeCheckOffSet = 0.4f;
        float ledgeCheckYOffSet = 0.1f;
        Vector3 raycastPosition = highestLedge.hitInfo.point - transform.right * ledgeCheckOffSet;

        for (int i = 0; i < 2; i++)
        {
            Vector3 raycastOrigin = raycastPosition + transform.forward * 0.02f +
                  new Vector3(0, ledgeCheckYOffSet, 0) + transform.right * i * ledgeCheckOffSet * 2;
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, -transform.up, out hit, rayLength, obstacleLayer))
            {
                Debug.DrawRay(raycastOrigin, -transform.up * rayLength, Color.blue);
            }
            else
            {
                Debug.DrawRay(raycastOrigin, -transform.up * rayLength, Color.red);
                if (i == 0)
                {
                    return new Vector2(-1, 0);
                }
                else
                {
                    return new Vector2(1, 0);
                }
            }
        }
        return new Vector2(1,1);

    }
    public void CheckLedge()
    {
        for (int i = rayHitList.Count - 1; i < rayHitList.Count; i--)
        {
            if (i == 0) break;
            // Ledge
            if (rayHitList[i].distance == 0f && rayHitList[i - 1].distance != 0f)
            {
                RaycastHitInfo newLedge = new RaycastHitInfo(rayHitList[i - 1], rayHitList[i - 1].distance,
                       rayHitList[i - 1].point.y, Vector3.Angle(rayHitList[i - 1].normal, transform.forward)); //.TransformDirection(Vector3.forward)));
                highestLedge = newLedge;
                relevantAction = RelevantAction.Edge;
                //GetSurfaceNormalDirection(rayHitList[i - 1].normal);
                //GetTargetHangPosition();
            }
            //Ledge
            else if (rayHitList[i].distance != 0 && rayHitList[i - 1].distance != 0 && 
                rayHitList[i].distance - rayHitList[i - 1].distance > 0) // 0.1f is ledge off set.
            {
                RaycastHitInfo newLedge = new RaycastHitInfo(rayHitList[i - 1], rayHitList[i - 1].distance,
                    rayHitList[i - 1].point.y, Vector3.Angle(rayHitList[i - 1].normal, transform.forward));//.TransformDirection(Vector3.forward)));
                highestLedge = newLedge;
                relevantAction = RelevantAction.Edge;
                //GetSurfaceNormalDirection(rayHitList[i - 1].normal);

            }
            else
            {
                relevantAction = RelevantAction.None;
            }
        }
    }

    public Vector3 GetTargetHangPosition()
    {
        Vector3 capsuleOffSet = -transform.forward * (controller.radius + controller.skinWidth);
         currentTargetPosition = highestLedge.hitInfo.point + capsuleOffSet - new Vector3(0, controller.height, 0);
        
        return currentTargetPosition;
    }
    public Quaternion GetRotationToMatch()
    {
        // Calculate the rotation needed to align the player's forward direction with the surface -normal.
        Quaternion rotationToMatch = Quaternion.FromToRotation(transform.rotation * Vector3.forward, -highestLedge.hitInfo.normal);

        return rotationToMatch;
    }

    /*private RelevantAction CheckDepth(Vector3 hitPoint, Vector3 hitNormal)
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

            
            - If both raycasts don't hit something, there's a high wall. 
            - If the first raycast hits above the player's height, there is only an edge. So player can hold the edge.
            - If two raycasts hits above the player's height, there is an area where the player can climb on top after holding on.
            - If the first raycast hits below the player's height, the player can move behind the obstacle.
            - If two raycasts hits below the player's height, the player can step up above the obstacle.
             

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
    */

    /*private SurfaceNormalDirection GetSurfaceNormalDirection(Vector3 hitNormal)
    {
        //// Calculate the rotation needed to align the player's forward direction with the surface normal.
        //Quaternion rotation = Quaternion.FromToRotation(transform.up, hitNormal);

        //// Apply the rotation to the player's transform.
        //transform.rotation = rotation * transform.rotation;

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
    }*/

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


