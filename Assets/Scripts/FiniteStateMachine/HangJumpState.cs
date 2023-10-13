using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangJumpState : State
{
    public HangJumpState(ThirdPersonCharacterController sm) : base(sm)
    {
    }
    private float _rotationVelocity; // SmoothDampAngle function ref
    private float jumpTimeoutDelta; // timeout deltatime
    private Vector2 jumpDirection;
    private Vector3 backJumpDirection;
    private bool isBackJumping;
    public override void OnStart()
    {
        playerController.StopHangingRemainingTime = playerController.StopScanTimer; // reset timer;
        jumpTimeoutDelta = playerController.HangJumpTimeout;
        playerController.IsHangJumping = true;
        playerController.Animator.SetBool("HangJump", true);
        jumpDirection = playerController.GameInput.GetMovementVectorNormalized();
        playerController.InvokeOnHangJumpAction(jumpDirection);

        if (jumpDirection == new Vector2(0,-1))
        {
            playerController.RotateOnMove = true;
            isBackJumping = true;
            backJumpDirection = -playerController.transform.forward;
        }
        else
        {
            playerController.RotateOnMove = false;
            isBackJumping = false;
        }
    }

    public override void OnUpdate()
    {   CameraRotation();
        if (playerController.IsHanging)
        {
            playerController.ChangeState(playerController.hangState);
        }

        if (playerController.GroundCheck())
        {
            playerController.ChangeState(playerController.movementState);
        }

        //if (playerController.IsHangJumping && jumpTimeoutDelta >= 0.0f)
        //{
        //    playerController.VerticalVelocity = Mathf.Sqrt(playerController.HangJumpHeight * -1.5f * playerController.Gravity);
        //}

        // jump timeout
        if (jumpTimeoutDelta >= 0.0f)
        {
            jumpTimeoutDelta -= Time.deltaTime;
            playerController.VerticalVelocity = Mathf.Sqrt(playerController.HangJumpHeight * -1.5f * playerController.Gravity);
        }
        else
        {
            
            // move the player
            if (!isBackJumping)
            {

                playerController.CharacterController.Move(playerController.transform.TransformDirection( new Vector3(jumpDirection.x * 2, jumpDirection.y, 0)) * (playerController.HangJumpHeight * Time.deltaTime) +
                        new Vector3(0.0f, playerController.VerticalVelocity, 0.0f) * Time.deltaTime);

                if (playerController.EnvironmentScan.highestLedge.hitInfo.distance == 0)
                {
                    playerController.RotateOnMove = true;
                }
                // playerController.CharacterController.Move(playerController.transform.TransformDirection(new Vector3(0, 0, playerController.GameInput.GetMovementVectorNormalized().y * 0.2f * Time.deltaTime)));
            }
            else
            {
                playerController.CharacterController.Move(backJumpDirection * 2 * (playerController.HangJumpHeight * Time.deltaTime) +
                    new Vector3(0.0f, playerController.VerticalVelocity, 0.0f) * Time.deltaTime);
            }
        }

        float inputMagnitude = playerController.GameInput.GetMoveInputMagnitude();
        playerController.InvokeOnMotionBlendChangeAction(inputMagnitude * Time.deltaTime);
    }

    private void CameraRotation()
    {
        Vector2 movementVector = playerController.GameInput.GetMovementVectorNormalized();

        // normalise input direction
        Vector3 inputDirection = new Vector3(movementVector.x, 0f, movementVector.y).normalized;

        if (playerController.CharacterController.enabled == true)
        {
            // if there is a move input rotate player when the player is moving
            if (playerController.GameInput.GetMovementVectorNormalized() != Vector2.zero)
            {
                playerController.TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(playerController.transform.eulerAngles.y,
                    playerController.TargetRotation, ref _rotationVelocity,
                    playerController.RotationSmoothTime);

                if (playerController.RotateOnMove)
                {
                    //// rotate to face input direction relative to camera position
                    //playerController.transform.rotation = Quaternion.Slerp(playerController.transform.rotation, 
                    //    Quaternion.Euler(0.0f, rotation, 0.0f), 5f * Time.deltaTime) ;
                
                    if (isBackJumping)
                    {
                        playerController.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    }
                    else
                    {
                        // rotate to face input direction relative to camera position
                        playerController.transform.rotation = Quaternion.Slerp(playerController.transform.rotation,
                            Quaternion.Euler(0.0f, rotation, 0.0f), 5 * Time.deltaTime);
                    }
                    
                }
            }
        }
    }
    public override void OnExit()
    {
        playerController.IsHangJumping = false;
        playerController.Animator.SetBool("HangJump", false);
        playerController.StopHangingRemainingTime = playerController.StopScanTimer;
        playerController.RotateOnMove = true;
        //jumpTimeoutDelta = playerController.HangJumpTimeout;
        isBackJumping = false;
    }
}
