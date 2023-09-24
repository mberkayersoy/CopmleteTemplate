using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{
    // EVENTS
    public event EventHandler<OnSpeedChangeActionEventArgs> OnSpeedChangeAction;
    public event EventHandler<OnMotionBlendChangeActionEventArgs> OnMotionBlendChangeAction;
    public event EventHandler<OnGrondedChangeActionEventArgs> OnGroundedChangeAction;
    public event EventHandler OnHangAction;
    public event EventHandler<OnJumpActionEventArgs> OnJumpAction;
    public event EventHandler<OnFreeFallEventArgs> OnFreeFallAction;

    // EVENT ARGS
    public class OnSpeedChangeActionEventArgs : EventArgs { public float speed; }
    public class OnMotionBlendChangeActionEventArgs : EventArgs { public float animationBlend; }
    public class OnGrondedChangeActionEventArgs : EventArgs { public bool isGrounded; }
    public class OnFreeFallEventArgs : EventArgs { public bool freeFall; }
    public class OnJumpActionEventArgs : EventArgs 
    { 
        public bool isJumping;
        public RelevantAction relevantAction;
    }

    [Header("REFERENCES")]
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private Animator animator;
    private CharacterController controller;
    private GameInput gameInput;
    private EnvironmentScan environmentScan;

    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float speedChangeRate = 10.0f;

    [Header("JUMP")]
    [SerializeField] private bool isJumping;
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
    public float VerticalVelocity { get => verticalVelocity; private set => verticalVelocity = value; }
    public bool IsJumping { get => isJumping ; private set => isJumping = value; }

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    [Header("CAMERA")]
    [SerializeField] private bool lockCameraPosition = false;
    [SerializeField] private bool _rotateOnMove = true;

    [Space(5)]
    [Header("BOOLEANS")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isRolling;
    [SerializeField] private bool isSliding;
    [SerializeField] private bool isGrounded;

    [Header("GROUND")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius = 0.3f;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    private void Start()
    {
        gameInput = GameInput.Instance;
        controller = GetComponent<CharacterController>();
        environmentScan = GetComponent<EnvironmentScan>();
        animationManager.OnEnableCharacterController += AnimationManager_OnEnableCharacterController;
        animationManager.OnDisableCharacterController += AnimationManager_OnDisableCharacterController;
        Cursor.lockState = CursorLockMode.Locked;
        gameInput.OnSprintAction += GameInput_OnSprintAction;
        gameInput.OnJumpAction += GameInput_OnJumpAction;

        // reset our timeouts on start
        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;
    }
    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        switch (environmentScan.relevantAction)
        {
            case RelevantAction.Edge:
                OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
                {
                    relevantAction = environmentScan.relevantAction,
                });
                break;
            case RelevantAction.Vault:
                OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
                {
                    relevantAction = environmentScan.relevantAction,
                });
                break;
            case RelevantAction.StepUp:
                break;
            case RelevantAction.None:
                //if (isJumping) return;
                isJumping = true;
                OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
                {
                    relevantAction = environmentScan.relevantAction,
                    isJumping = isJumping
                });
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        Move();
        Jump();
        ApplyGravity();
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector2 movementVector = gameInput.GetMovementVectorNormalized();

        // If there is no input set the targetspeed to 0.
        if (movementVector == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        float inputMagnitude = gameInput.GetMoveInputMagnitude();

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(movementVector.x, 0f, movementVector.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
       
        if (controller.enabled == true)
        {
            // if there is a move input rotate player when the player is moving
            if (gameInput.GetMovementVectorNormalized() != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  Camera.main.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }



        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

        OnSpeedChangeAction?.Invoke(this, new OnSpeedChangeActionEventArgs
        {
            speed = _speed
        });

        OnMotionBlendChangeAction?.Invoke(this, new OnMotionBlendChangeActionEventArgs
        {
            animationBlend = inputMagnitude
        });

    }

    private void Jump()
    {
        if (isGrounded)
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
            isJumping = false;
            OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
            {
                relevantAction = RelevantAction.None,
                isJumping = isJumping
            });
        }

        //// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        //if (VerticalVelocity < terminalVelocity)
        //{
        //    VerticalVelocity += Gravity * Time.deltaTime;
        //}
    }

    private void ApplyGravity()
    {
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (VerticalVelocity < terminalVelocity)
        {
            VerticalVelocity += Gravity * Time.deltaTime;
        }
    }
    public bool GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundOffset, groundRadius, groundLayer);

        OnGroundedChangeAction?.Invoke(this, new OnGrondedChangeActionEventArgs
        {
            isGrounded = isGrounded
        });
        
        //if (isGrounded)
        //{
        //    enableFeetIK = true;
        //}
        //else
        //{
        //    enableFeetIK = false;
        //}
        return isGrounded;
    }

    private void AnimationManager_OnDisableCharacterController(object sender, EventArgs e)
    {
        controller.enabled = false;
    }

    private void AnimationManager_OnEnableCharacterController(object sender, EventArgs e)
    {
        controller.enabled = true;
    }

    private void GameInput_OnSprintAction(object sender, GameInput.OnSprintActionEventArgs e)
    {
        isSprinting = e.isSprint;
    }


    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y + groundOffset.y, transform.position.z),
            groundRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        //{
        //    if (FootstepAudioClips.Length > 0)
        //    {
        //        var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
        //        AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
        //    }
        //}
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        //if (animationEvent.animatorClipInfo.weight > 0.5f)
        //{
        //    AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        //}
    }
}
