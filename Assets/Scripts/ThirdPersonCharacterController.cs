using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThirdPersonCharacterController : MonoBehaviour
{
    [Header("DEBUG")]
    [SerializeField] private TextMeshProUGUI currentStateText;
    [SerializeField] private TextMeshProUGUI previousStateText;

    // EVENTS
    public event EventHandler<OnSpeedChangeActionEventArgs> OnSpeedChangeAction;
    public event EventHandler<OnMotionBlendChangeActionEventArgs> OnMotionBlendChangeAction;
    public event EventHandler<OnGrondedChangeActionEventArgs> OnGroundedChangeAction;
    public event EventHandler<OnHangActionEventArgs> OnHangAction;
    public event EventHandler<OnHangActionEventArgs> OnHangIdleAction;
    public event EventHandler<OnHangJumpActionEventArgs> OnHangJumpAction;
    public event EventHandler<OnJumpActionEventArgs> OnJumpAction;
    public event EventHandler<OnFreeFallEventArgs> OnFreeFallAction;
    public event EventHandler<OnHangMovementActionEventArgs> OnHangMovementAction;
    public event EventHandler<OnHasWallChangeActionEventArgs> OnHasWallChangeAction;

    // EVENT ARGS
    public class OnSpeedChangeActionEventArgs : EventArgs { public float speed; }
    public class OnMotionBlendChangeActionEventArgs : EventArgs { public float animationBlend; }
    public class OnGrondedChangeActionEventArgs : EventArgs { public bool isGrounded; }
    public class OnFreeFallEventArgs : EventArgs { public bool freeFall; }
    public class OnHangActionEventArgs : EventArgs { public bool isHanging; }
    public class OnHangJumpActionEventArgs : EventArgs { public Vector2 movementVector; }
    public class OnHangMovementActionEventArgs : EventArgs { public Vector2 movementVector; }
    public class OnHasWallChangeActionEventArgs : EventArgs { public bool hasWall; }
    public class OnJumpActionEventArgs : EventArgs 
    { 
        public bool isJumping;
        public RelevantAction relevantAction;
    }

    [Header("REFERENCES")]
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private Animator animator;
    private CharacterController _characterController;
    private GameInput gameInput;
    private EnvironmentScan _environmentScan;

    [Header("MOVEMENT")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private bool isParkour;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 5.335f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float speedChangeRate = 10.0f;

    [Space(5)]

    [Header("JUMP")]
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isHangJumping;
    //Time required to pass before entering the fall state. Useful for walking down stairs
    [SerializeField] private float fallTimeout = 0.15f;
    //The height the player can jump from ground
    [SerializeField] private float jumpHeight = 1.2f;
    //The height the player can jump from hang
    [SerializeField] private float hangJumpHeight = 1f;
    // The character uses its own gravity value. The engine default is -9.81f
    [SerializeField] private float gravity = -15.0f;
    //Time required to pass before being able to jump again. Set to 0f to instantly jump again
    [SerializeField] private float jumpTimeout = 0.50f;
    [SerializeField] private float hangJumpTimeout = 0.50f;
    [SerializeField] private float verticalVelocity;
    private float terminalVelocity = 53.0f;

    [Header("CAMERA")]
    [SerializeField] private bool lockCameraPosition = false;
    [SerializeField] private bool rotateOnMove = true;

    [Space(5)]

    [Header("BOOLEANS")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isHanging;
    [SerializeField] private bool stopHanging;

    [Space(5)]

    [Header("GROUND")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius = 0.3f;

    [Space(5)]

    [Header("AUDIO")]
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("TIMER")]
    [SerializeField] private float stopScanTimer;
    private float stopHangingRemainingTime;

    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;



    #region GetAndSet
    public float VerticalVelocity { get => verticalVelocity; set => verticalVelocity = value; }
    public bool IsJumping { get => isJumping ; set => isJumping = value; }
    public GameInput GameInput { get => gameInput; set => gameInput = value; }
    public CharacterController CharacterController { get => _characterController; }
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public bool IsParkour { get => isParkour; set => isParkour = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float SprintSpeed { get => sprintSpeed; set => sprintSpeed = value; }
    public float RotationSmoothTime { get => rotationSmoothTime; set => rotationSmoothTime = value; }
    public float SpeedChangeRate { get => speedChangeRate; set => speedChangeRate = value; }
    public bool IsSprinting { get => isSprinting; set => isSprinting = value; }
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }
    public bool IsHanging { get => isHanging; set => isHanging = value; }
    public bool StopHanging { get => stopHanging; set => stopHanging = value; }
    public float TerminalVelocity { get => terminalVelocity;}
    public EnvironmentScan EnvironmentScan { get => _environmentScan; set => _environmentScan = value; }
    public float Speed { get => _speed; set => _speed = value; }
    public float AnimationBlend { get => _animationBlend; set => _animationBlend = value; }
    public float TargetRotation { get => _targetRotation; set => _targetRotation = value; }
    public float FallTimeout { get => fallTimeout; set => fallTimeout = value; }
    public float JumpHeight { get => jumpHeight; set => jumpHeight = value; }
    public float Gravity { get => gravity; set => gravity = value; }
    public float JumpTimeout { get => jumpTimeout; set => jumpTimeout = value; }
    public Animator Animator { get => animator; }
    public bool IsHangJumping { get => isHangJumping; set => isHangJumping = value; }
    public float HangJumpHeight { get => hangJumpHeight; set => hangJumpHeight = value; }
    public float StopScanTimer { get => stopScanTimer; set => stopScanTimer = value; }
    public float StopHangingRemainingTime { get => stopHangingRemainingTime; set => stopHangingRemainingTime = value; }
    public bool RotateOnMove { get => rotateOnMove; set => rotateOnMove = value; }
    public float HangJumpTimeout { get => hangJumpTimeout; set => hangJumpTimeout = value; }

    #endregion

    // STATES
    public State currentState;
    public IdleState idleState;
    public MovementState movementState;
    public RunState runState;
    public JumpState jumpState;
    public HangJumpState hangJumpState;
    public FreeFallState freeFallState;
    public HangState hangState;
    public VaultState vaultState;

    // timeout deltatime
    private float jumpTimeoutDelta;
    private float hangJumpTimeoutDelta;
    private float fallTimeoutDelta;
    private void Start()
    {
        GameInput = GameInput.Instance;
        _characterController = GetComponent<CharacterController>();
        _environmentScan = GetComponent<EnvironmentScan>();
        animationManager.OnEnableCharacterController += AnimationManager_OnEnableCharacterController;
        animationManager.OnDisableCharacterController += AnimationManager_OnDisableCharacterController;
        GameInput.OnSprintAction += GameInput_OnSprintAction;
        GameInput.OnJumpAction += GameInput_OnJumpAction;
        gameInput.OnStopHangingAction += GameInput_OnStopHangingAction;
        Cursor.lockState = CursorLockMode.Locked;

        idleState = new IdleState(this);
        movementState = new MovementState(this);
        runState = new RunState(this);
        jumpState = new JumpState(this);
        freeFallState = new FreeFallState(this);
        hangState = new HangState(this);
        vaultState = new VaultState(this);
        hangJumpState = new HangJumpState(this);
        ChangeState(movementState);
        //// reset our timeouts on start
        //jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;

    }

    private void GameInput_OnStopHangingAction(object sender, GameInput.OnStopHangingActionEventArgs e)
    {
        stopHanging = e.isPressing;

        if (stopHanging)
        {
            stopHangingRemainingTime = stopScanTimer;
        }

        if (currentState == hangState)
        {
            ChangeState(movementState);
        }


    }

    private void GameInput_OnJumpAction(object sender, EventArgs e)
    {
        if (isHanging == false)
        {
            switch (_environmentScan.relevantAction)
            {
                case RelevantAction.Vault:
                    ChangeState(vaultState);
                    break;

                case RelevantAction.StepUp:
                    break;

                case RelevantAction.None:
                    if (currentState != movementState) return;
                    ChangeState(jumpState);
                    break;

                default:
                    break;
            }
        }
        //else
        //{
        //    //isHangJumping = true;
        //    ChangeState(hangJumpState);
        //}

    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
        //Move();
       // Jump();
        ApplyGravity();
        //MoveTargetHangPosition();

        if (!GroundCheck())
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
                InvokeOnFreeFallAction(true);
            }

            // if we are not grounded, do not jump
            IsJumping = false;
            InvokeOnJumpAction(IsJumping, RelevantAction.None);

        }
        else
        {
            // reset the jump timeout timer
            jumpTimeoutDelta = JumpTimeout;

            InvokeOnFreeFallAction(false);
        }

        if (!IsHanging)
        {
            // reset the jump timeout timer
            hangJumpTimeoutDelta = hangJumpTimeout;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            //else
            //{
            //    InvokeOnFreeFallAction(true);
            //}

            // if we are not hanging, do not hang jump
            IsHangJumping = false;

        }
        else
        {
            // reset the jump timeout timer
            hangJumpTimeoutDelta = hangJumpTimeout;
        }


        if (!GroundCheck() && !IsHanging && !stopHanging)
        {
            if (stopHangingRemainingTime >= 0)
            {
                stopHangingRemainingTime -= Time.deltaTime;

                return;
            }
            switch (_environmentScan.relevantAction)
            {
                case RelevantAction.Edge:

                    ChangeState(hangState);
                    break;

                default:
                    break;
            }
        }
    }
    public void ChangeState(State newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
            previousStateText.text = "Previous State: " + currentState.ToString();
        }
        currentState = newState;
        currentState.OnStart();
        currentStateText.text = "Current State: " + currentState.ToString();
    }

    private void Move()
    {
        if (isParkour) return;
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector2 movementVector = GameInput.GetMovementVectorNormalized();

        // If there is no input set the targetspeed to 0.
        if (movementVector == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(CharacterController.velocity.x, 0.0f, CharacterController.velocity.z).magnitude;

        float speedOffset = 0.1f;

        float inputMagnitude = GameInput.GetMoveInputMagnitude();

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
       
        if (CharacterController.enabled == true)
        {
            // if there is a move input rotate player when the player is moving
            if (GameInput.GetMovementVectorNormalized() != Vector2.zero)
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
        CharacterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
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

    // If there is an obstacle above the player, update jumpHeight
    public void CalculateJumpHeight()
    {
        jumpHeight = 1.2f;
        float yOffSet = 0.07f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, CharacterController.height + yOffSet, 0), Vector3.up, out hit, jumpHeight)) 
        {
            Debug.DrawLine(transform.position + new Vector3(0, CharacterController.height + yOffSet, 0), hit.point);
            float distance = Vector3.Distance(transform.position + new Vector3(0, 1.77f, 0), hit.point);

            if (distance < jumpHeight)
            {
                jumpHeight -= distance ;
            }
        }
    }
    private void Jump()
    {
        if (isGrounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = fallTimeout;
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
                VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
            jumpTimeoutDelta = jumpTimeout;

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
        // stop our velocity dropping infinitely when grounded
        if (GroundCheck() && VerticalVelocity < 0.0f)
        {
            VerticalVelocity = -2f;
        }
        else
        {
            if (currentState != hangState)
            {
                // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                if (VerticalVelocity < TerminalVelocity)
                {
                    VerticalVelocity += gravity * Time.deltaTime;
                }
            }

        }
    }
    public bool GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundOffset, groundRadius, groundLayer);
        //if (IsGrounded) IsHanging = false;
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
        CharacterController.enabled = false;
    }

    private void AnimationManager_OnEnableCharacterController(object sender, EventArgs e)
    {
        CharacterController.enabled = true;
    }

    private void GameInput_OnSprintAction(object sender, GameInput.OnSprintActionEventArgs e)
    {
        isSprinting = e.isSprint;
    }

    public void InvokeOnFreeFallAction(bool isFreeFalling)
    {
        OnFreeFallAction?.Invoke(this, new OnFreeFallEventArgs
        {
            freeFall = isFreeFalling
        });
    }

    public void InvokeOnJumpAction(bool isJumping, RelevantAction relevantAction)
    {
        OnJumpAction?.Invoke(this, new OnJumpActionEventArgs
        {
            relevantAction = relevantAction,
            isJumping = isJumping
        });
    }    
    public void InvokeOnHangAction(bool isHanging)
    {
        OnHangAction?.Invoke(this, new OnHangActionEventArgs
        {
            isHanging  = isHanging
        });
    }

    public void InvokeOnHasWallChangeAction(bool hasWall)
    {
        OnHasWallChangeAction?.Invoke(this, new OnHasWallChangeActionEventArgs
        {
            hasWall = hasWall
        });
    }
    public void InvokeOnHangIdleAction(bool isHanging)
    {
        OnHangIdleAction?.Invoke(this, new OnHangActionEventArgs
        {
            isHanging = isHanging
        });
    }

    public void InvokeOnSpeedChangeAction(float speed)
    {
        OnSpeedChangeAction?.Invoke(this, new OnSpeedChangeActionEventArgs
        {
            speed = _speed
        });

    }

    public void InvokeOnMotionBlendChangeAction(float inputMagnitude)
    {
        OnMotionBlendChangeAction?.Invoke(this, new OnMotionBlendChangeActionEventArgs
        {
            animationBlend = inputMagnitude
        });
    }

    public void InvokeOnHangJumpAction(Vector2 movementVector)
    {
        OnHangJumpAction?.Invoke(this, new OnHangJumpActionEventArgs
        {
            movementVector = movementVector
        });
    }

    public void InvokeOnHangMovementAction(Vector2 movementVector)
    {
        OnHangMovementAction?.Invoke(this, new OnHangMovementActionEventArgs
        {
            movementVector = movementVector
        });
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
