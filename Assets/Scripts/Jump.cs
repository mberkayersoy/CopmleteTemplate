using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Jump : MonoBehaviour
{
    private GameInput gameInput;
    private PlayerMovement playerMovement;

    // EVENTS
    public event EventHandler<OnJumpActionEventArgs> OnJumpAction;
    public event EventHandler<OnFreeFallEventArgs> OnFreeFallAction;

    // EVENT ARGS
    public class OnJumpActionEventArgs : EventArgs{ public bool isJumping; }
    public class OnFreeFallEventArgs : EventArgs { public bool freeFall; }

    // JUMP VARIABLES
    public bool IsJumping { get; private set; }
    public float VerticalVelocity { get; private set; }
    //Time required to pass before entering the fall state. Useful for walking down stairs
    [SerializeField] private float FallTimeout = 0.15f;
    //The height the player can jump
    [SerializeField] private float JumpHeight = 1.2f;
    // The character uses its own gravity value. The engine default is -9.81f
    [SerializeField] private float Gravity = -15.0f;
    //Time required to pass before being able to jump again. Set to 0f to instantly jump again
    [SerializeField] private float JumpTimeout = 0.50f;
    private float terminalVelocity = 53.0f;
    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        gameInput = GameInput.Instance;

        gameInput.OnJumpAction += GameInput_OnJumpAction;

        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }

    void Update()
    {
        JumpAndGravity();
    }
    private void GameInput_OnJumpAction(object sender, System.EventArgs e)
    {
        if (playerMovement.GroundCheck())
        {
            IsJumping = true;
        }
    }

    private void JumpAndGravity()
    {
        if (playerMovement.GroundCheck())
        {
            // reset the fall timeout timer
            fallTimeoutDelta = FallTimeout;

            OnJumpAction?.Invoke(this, new OnJumpActionEventArgs {
                isJumping = false
            });
            OnFreeFallAction?.Invoke(this, new OnFreeFallEventArgs
            {
                freeFall = false
            }); ;
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
                OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
                {
                    isJumping = true
                });
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
