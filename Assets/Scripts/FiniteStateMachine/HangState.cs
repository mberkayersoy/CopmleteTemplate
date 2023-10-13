using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangState : State
{
    public float fixRotationSmoothSpeed = 4f;
    private bool isArrived;
    private float hangingMoveSpeed = 0.5f;
    private float hangJumpTimeOut = 0.5f;
    public HangState(ThirdPersonCharacterController sm) : base(sm)
    {
    }
    public override void OnStart()
    {
        if (playerController.EnvironmentScan.highestLedge.distanceToPlayer == 0)
        {
            playerController.ChangeState(playerController.movementState);
        }
        playerController.VerticalVelocity = -1;
        playerController.IsHanging = true;
        playerController.TargetPosition = playerController.EnvironmentScan.GetTargetHangPosition();
        //playerController.InvokeOnJumpAction(playerController.IsJumping, playerController.EnvironmentScan.relevantAction);
        playerController.Animator.SetBool("IsHanging", true);
        playerController.GameInput.OnJumpAction += GameInput_OnJumpAction;
        isArrived = true;
        hangJumpTimeOut = 0.5f;
    }   

    private void GameInput_OnJumpAction(object sender, System.EventArgs e)
    {
        if (hangJumpTimeOut <= 0)
        {
            playerController.ChangeState(playerController.hangJumpState);
        }
    }

    public override void OnUpdate()
    {
        playerController.InvokeOnHasWallChangeAction(playerController.EnvironmentScan.CheckWall());
        FixRotation();
        if (!isArrived)
        {
            float distanceToTarget = Vector3.Distance(playerController.transform.position, playerController.TargetPosition);
            float tolerance = 0.05f; 

            if (distanceToTarget > tolerance)
            {
                Vector3 moveDirection = (playerController.TargetPosition - playerController.transform.position).normalized;

                playerController.CharacterController.Move(moveDirection * 2 *  Time.deltaTime);
            }
            else
            {
                isArrived = true;
                Debug.Log("IsArrived: " + isArrived);
            }
        }
        hangJumpTimeOut -= Time.deltaTime;

        playerController.AnimationBlend = Mathf.Lerp(playerController.AnimationBlend, playerController.HangJumpHeight, Time.deltaTime * 0.5f);
        if (playerController.AnimationBlend < 0.01f) playerController.AnimationBlend = 0f;

        playerController.InvokeOnMotionBlendChangeAction(playerController.GameInput.GetMoveInputMagnitude());
        
        Move();
    }
    private void Move()
    {
        if (hangJumpTimeOut >= 0) return;

        Vector2 movementVector = playerController.GameInput.GetMovementVectorNormalized();
        Vector2 ledgeBounds = playerController.EnvironmentScan.CheckLedgeBounds();
        if (playerController.GameInput.GetMovementVectorNormalized() != Vector2.zero)
        {
            if (ledgeBounds == Vector2.right && movementVector.x > 0)
            {
                movementVector.x = 0;
                playerController.CharacterController.Move(playerController.transform.right * movementVector.x * hangingMoveSpeed * Time.deltaTime);
            }
            else if (ledgeBounds == -Vector2.right && movementVector.x < 0)
            {
                movementVector.x = 0;
                playerController.CharacterController.Move(playerController.transform.right * movementVector.x * hangingMoveSpeed * Time.deltaTime);
            }
            else
            {
                playerController.CharacterController.Move(playerController.transform.right * movementVector.x * hangingMoveSpeed * Time.deltaTime);
            }

            playerController.InvokeOnHangMovementAction(movementVector);
        }
        else
        {
            playerController.InvokeOnHangMovementAction(movementVector);
        }
    }
    private void FixRotation()
    {
        Quaternion startRotation = playerController.transform.rotation;
        Quaternion targetRotation = playerController.EnvironmentScan.GetRotationToMatch();
        targetRotation.x = 0;
        targetRotation.z = 0;
        playerController.transform.rotation = Quaternion.Slerp(startRotation,
                                              targetRotation * startRotation,
                                              fixRotationSmoothSpeed * Time.deltaTime);
    }

    public override void OnExit()
    {
        playerController.GameInput.OnJumpAction -= GameInput_OnJumpAction;
        playerController.IsHanging = false;
        isArrived = false;
        //playerController.GameInput.OnJumpAction -= GameInput_OnJumpAction;
        playerController.Animator.SetBool("IsHanging", false);
        playerController.Animator.SetBool("FreeFall", true);
        hangJumpTimeOut = 0.5f;
        playerController.StopHangingRemainingTime = playerController.StopScanTimer;
        //playerController.InvokeOnFreeFallAction(true);
    }
}
