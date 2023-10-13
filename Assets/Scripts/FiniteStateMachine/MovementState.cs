using UnityEngine;

public class MovementState : State
{
    public MovementState(ThirdPersonCharacterController sm) : base(sm)
    {
    }

    private float _rotationVelocity; // SmoothDampAngle function ref

    public override void OnStart()
    {
        base.OnStart();
    }


    public override void OnUpdate()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = playerController.IsSprinting ? playerController.SprintSpeed : playerController.MoveSpeed;

        Vector2 movementVector = playerController.GameInput.GetMovementVectorNormalized();

        // If there is no input set the targetspeed to 0.
        if (movementVector == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(playerController.CharacterController.velocity.x,
            0.0f, playerController.CharacterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        float inputMagnitude = playerController.GameInput.GetMoveInputMagnitude();

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            playerController.Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * playerController.SpeedChangeRate);

            // round speed to 3 decimal places
            playerController.Speed = Mathf.Round(playerController.Speed * 1000f) / 1000f;
        }
        else
        {
            playerController.Speed = targetSpeed;
        }

        playerController.AnimationBlend = Mathf.Lerp(playerController.AnimationBlend, targetSpeed, Time.deltaTime * playerController.SpeedChangeRate);
        if (playerController.AnimationBlend < 0.01f) playerController.AnimationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(movementVector.x, 0f, movementVector.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude

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

        Vector3 targetDirection = Quaternion.Euler(0.0f, playerController.TargetRotation, 0.0f) * Vector3.forward;

        // move the player
        playerController.CharacterController.Move(targetDirection.normalized * (playerController.Speed * Time.deltaTime) +
                         new Vector3(0.0f, playerController.VerticalVelocity, 0.0f) * Time.deltaTime);

        playerController.InvokeOnSpeedChangeAction(playerController.Speed);

        playerController.InvokeOnMotionBlendChangeAction(inputMagnitude);

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
