using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Jump : MonoBehaviour
{
    private ThirdPersonCharacterController playerController;

    // EVENTS
    public event EventHandler<OnFreeFallEventArgs> OnFreeFallAction;

    // EVENT ARGS
    public class OnFreeFallEventArgs : EventArgs { public bool freeFall; }


    // JUMP VARIABLES
    public bool IsJumping { get; set; }
    public float VerticalVelocity { get => verticalVelocity; private set => verticalVelocity = value; }

    //Time required to pass before entering the fall state. Useful for walking down stairs
    [SerializeField] private float FallTimeout = 0.15f;
    //The height the player can jump
    [SerializeField] private float JumpHeight = 1.2f;
    // The character uses its own gravity value. The engine default is -9.81f
    [SerializeField] private float Gravity = -15.0f;
    //Time required to pass before being able to jump again. Set to 0f to instantly jump again
    [SerializeField] private float JumpTimeout = 0.50f;

    private float terminalVelocity = 53.0f;
    [SerializeField] private float verticalVelocity;
    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;


    private void Awake()
    {
        playerController = GetComponent<ThirdPersonCharacterController>();
    }

    private void Start()
    {
        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
        playerController.OnJumpAction += PlayerController_OnJumpAction;
    }

    private void PlayerController_OnJumpAction(object sender, ThirdPersonCharacterController.OnJumpActionEventArgs e)
    {
        switch (e.relevantAction)
        {
            case RelevantAction.Edge:
                break;
            case RelevantAction.Vault:
                break;
            case RelevantAction.StepUp:
                break;
            case RelevantAction.None:
                IsJumping = true;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        JumpAndGravity();
    }

    private void JumpAndGravity()
    {
        if (playerController.GroundCheck())
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;
            //OnJumpAction?.Invoke(this, new OnJumpActionEventArgs {
            //IsJumping = false;
            //});
            OnFreeFallAction?.Invoke(this, new OnFreeFallEventArgs
            {
                freeFall = false
            });
            // stop our velocity dropping infinitely when grounded
            if (VerticalVelocity < 0.0f)
            {
                VerticalVelocity = -2f;
            }
            // Jump
            if (IsJumping && jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                //OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
                //{
                //    isJumping = true
                //});
            }
            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                OnFreeFallAction?.Invoke(this, new OnFreeFallEventArgs
                {
                    freeFall = true
                });
            }

            // if we are not grounded, do not jump
            IsJumping = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (VerticalVelocity < terminalVelocity)
        {
            VerticalVelocity += Gravity * Time.deltaTime;
        }
    }

}
