using UnityEngine;
using System;

public class JumpState : State
{
    public JumpState(ThirdPersonCharacterController sm) : base(sm)
    {
    }

    private float _rotationVelocity; // SmoothDampAngle function ref
    // timeout deltatime
    private float jumpTimeoutDelta;
    public override void OnStart()
    {
        playerController.IsJumping = true;
        jumpTimeoutDelta = playerController.JumpTimeout;

        playerController.CalculateJumpHeight();
        playerController.InvokeOnJumpAction(playerController.IsJumping, playerController.EnvironmentScan.relevantAction); 

    }

    public override void OnUpdate()
    {
        if (playerController.GroundCheck())
        {
            if (!playerController.IsJumping)
            {
                playerController.ChangeState(playerController.movementState);
            }
        }

        if (playerController.IsJumping && jumpTimeoutDelta <= 0.0f)
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            playerController.VerticalVelocity = Mathf.Sqrt(playerController.JumpHeight * -2f * playerController.Gravity);
        }

        // jump timeout
        if (jumpTimeoutDelta >= 0.0f)
        {
            jumpTimeoutDelta -= Time.deltaTime;
        }

        CameraRotation();
        
        Vector3 targetDirection = Quaternion.Euler(0.0f, playerController.TargetRotation, 0.0f) * Vector3.forward;

        // move the player
        playerController.CharacterController.Move(targetDirection.normalized * (playerController.Speed * Time.deltaTime) +
                         new Vector3(0.0f, playerController.VerticalVelocity, 0.0f) * Time.deltaTime);

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

                // rotate to face input direction relative to camera position
                playerController.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }
    public override void OnExit()
    {
        playerController.IsJumping = false;
        //playerController.InvokeOnJumpAction(playerController.IsJumping);
    }
}
