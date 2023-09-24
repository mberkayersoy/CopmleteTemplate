using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIKHandler : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private ThirdPersonCharacterController characterController;

    [Header("FEET GROUNDER")]
    [SerializeField] public bool enableFeetIK = true;
    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.14f;
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f;
    [SerializeField] private LayerMask envrionmentLayer;
    [SerializeField] private float pelvisOffset = 0f;
    [Range(0, 1)] [SerializeField] private float pelvisUpAndDownSpeed = 0.25f;
    [Range(0, 1)] [SerializeField] private float feetToIKPositionSpeed = 0.5f;

    public string leftFootAnimVariableName = "LeftFootCurve";
    public string rightFootAnimVariableName = "RightFootCurve";

    [SerializeField] private bool useProIKFeature = false;
    [SerializeField] private bool showSolverDebug = true;


    private Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKPosition;
    private Quaternion leftFootIKRotation, rightFootIKRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (characterController.GroundCheck())
        {
            enableFeetIK = true;
        }
        else
        {
            enableFeetIK = false;
        }
        if (enableFeetIK == false) return;
        //Debug.Log("lastPelvisPositionY: " + lastPelvisPositionY);
        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        // Find and raycast to the ground to find positions.
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation); // Handle the solver for left foot.
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation); // Handle the solver for right foot.
    }


    private void OnAnimatorIK(int layerIndex)
    {
        if (enableFeetIK == false) return;

        CalculatePelvisPosition();
        // right foot ik position and rotation -- utilities the pro features in here.
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, animator.GetFloat(rightFootAnimVariableName));
        }

        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);

        // left foot ik position and rotation -- utilities the pro features in here.
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        if (useProIKFeature)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, animator.GetFloat(leftFootAnimVariableName));
        }

        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="foot"></param>
    /// <param name="positionIKHolder"></param>
    /// <param name="rotationIKHolder"></param>
    /// <param name="lastFootPositionY"></param>
    private void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = animator.GetIKPosition(foot);

        if (positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yVariable = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, feetToIKPositionSpeed);
            targetIKPosition.y += yVariable;

            lastFootPositionY = yVariable;

            targetIKPosition = transform.TransformPoint(targetIKPosition);
            animator.SetIKRotation(foot, rotationIKHolder);
        }

        animator.SetIKPosition(foot, targetIKPosition);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CalculatePelvisPosition()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastLeftFootPositionY == 0)
        {
            lastLeftFootPositionY = animator.bodyPosition.y;
            return;
        }

        float leftOffsetPosition = leftFootIKPosition.y - transform.position.y;
        float rightOffsetPosition = rightFootIKPosition.y - transform.position.y;

        float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;

        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisUpAndDownSpeed);

        //if (enableFeetIK == true)
        //{
        //    SetPelvisPosition(newPelvisPosition);
        //}
        animator.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = animator.bodyPosition.y;
    }

    /// <summary>
    /// Locating the feet position via a Raycast and then solving. 
    /// </summary>
    /// <param name="fromSkyPosition"></param>
    /// <param name="feetIKPositions"></param>
    /// <param name="feetIKRotations"></param>
    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        // Raycast handling section
        RaycastHit feetOutHit;

        if (showSolverDebug)
        {
            Debug.DrawLine(fromSkyPosition, fromSkyPosition + Vector3.down * (raycastDownDistance + heightFromGroundRaycast), Color.yellow);
        }

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit,
            raycastDownDistance + heightFromGroundRaycast, envrionmentLayer))
        {
            // finding feet ik positions from the sky position.
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = feetOutHit.point.y + pelvisOffset;
            feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            return;
        }

        feetIKPositions = Vector3.zero; // it didn't work.
    }

    private void AdjustFeetTarget(ref Vector3 feetPositions, HumanBodyBones foot)
    {
        feetPositions = animator.GetBoneTransform(foot).position;
        feetPositions.y = transform.position.y + heightFromGroundRaycast;
    }
}
