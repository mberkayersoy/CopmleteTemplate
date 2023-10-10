using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangState : State
{
    public float fixRotationSmoothSpeed = 2.0f;
    private bool isArrived;
    private float hangingMoveSpeed = 0.5f;
    private float hangJumpTimeOut = 0.5f;
    public HangState(ThirdPersonCharacterController sm) : base(sm)
    {
    }
    public override void OnStart()
    {
        playerController.VerticalVelocity = -1;
        playerController.IsHanging = true;
        playerController.TargetPosition = playerController.EnvironmentScan.GetTargetHangPosition();
        //playerController.InvokeOnJumpAction(playerController.IsJumping, playerController.EnvironmentScan.relevantAction);
        playerController.Animator.SetBool("IsHanging", true);
        playerController.GameInput.OnJumpAction += GameInput_OnJumpAction;
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
        playerController.transform.rotation = FixRotation();
        if (!isArrived)
        {
            float distanceToTarget = Vector3.Distance(playerController.transform.position, playerController.TargetPosition);
            float tolerance = 0.01f; 

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
        if (!isArrived && hangJumpTimeOut <= 0) return;

        Vector2 movementVector = playerController.GameInput.GetMovementVectorNormalized();
        if (playerController.GameInput.GetMovementVectorNormalized() != Vector2.zero)
        {
            switch (playerController.EnvironmentScan.currentSurfaceNormalDirection)
            {
                case EnvironmentScan.SurfaceNormalDirection.Forward:
                    playerController.CharacterController.Move(new Vector3(-movementVector.x, 0, 0) * hangingMoveSpeed * Time.deltaTime);
                    break;
                case EnvironmentScan.SurfaceNormalDirection.Backward:
                    playerController.CharacterController.Move(new Vector3(movementVector.x, 0, 0) * hangingMoveSpeed * Time.deltaTime);
                    break;
                case EnvironmentScan.SurfaceNormalDirection.Right:
                    playerController.CharacterController.Move(new Vector3(0, 0, movementVector.x) * hangingMoveSpeed * Time.deltaTime);
                    break;
                case EnvironmentScan.SurfaceNormalDirection.Left:
                    playerController.CharacterController.Move(new Vector3(0, 0, -movementVector.x) * hangingMoveSpeed * Time.deltaTime);
                    break;
                default:
                    break;
            }
        }
    }
    private Quaternion FixRotation()
    {
        Quaternion playerRotation = playerController.transform.rotation;

        Debug.Log("FixRotation; " + playerController.EnvironmentScan.currentSurfaceNormalDirection);
        switch (playerController.EnvironmentScan.currentSurfaceNormalDirection)
        {
            default:
            case EnvironmentScan.SurfaceNormalDirection.Forward:
                playerRotation = Quaternion.Slerp(playerRotation, Quaternion.Euler(0,-180,0), fixRotationSmoothSpeed * Time.deltaTime);
                return playerRotation;
            case EnvironmentScan.SurfaceNormalDirection.Backward:
                playerRotation = Quaternion.Slerp(playerRotation, Quaternion.Euler(0, 0, 0), fixRotationSmoothSpeed * Time.deltaTime);
                return playerRotation;
            case EnvironmentScan.SurfaceNormalDirection.Right:
                playerRotation = Quaternion.Slerp(playerRotation, Quaternion.Euler(0, -90, 0), fixRotationSmoothSpeed * Time.deltaTime);
                return playerRotation;
            case EnvironmentScan.SurfaceNormalDirection.Left:
                playerRotation = Quaternion.Slerp(playerRotation, Quaternion.Euler(0, 90, 0), fixRotationSmoothSpeed * Time.deltaTime);
                return playerRotation;
        }
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
        //playerController.InvokeOnFreeFallAction(true);
    }
}
