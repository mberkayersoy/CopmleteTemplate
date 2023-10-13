using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    // EVENTS
    public event EventHandler<OnSpeedChangeActionEventArgs> OnSpeedChangeAction;
    public event EventHandler<OnMotionBlendChangeActionEventArgs> OnMotionBlendChangeAction;
    public event EventHandler<OnGrondedChangeActionEventArgs> OnGrondedChangeAction;
    public event EventHandler<OnRollActionEventArgs> OnRollAction;
    public event EventHandler<OnSlideActionEventArgs> OnSlideAction;


    // EVENT ARGS
    public class OnSpeedChangeActionEventArgs : EventArgs { public float speed; }
    public class OnMotionBlendChangeActionEventArgs : EventArgs{ public float animationBlend; }
    public class OnGrondedChangeActionEventArgs : EventArgs { public bool isGrounded; }
    public class OnRollActionEventArgs : EventArgs { public bool isRolling; }
    public class OnSlideActionEventArgs : EventArgs { public bool isSliding; }

    // REFERENCES
    private Rigidbody rb;
    private GameInput gameInput;
    //private Jump jump;

    [Header("GROUND")]
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius = 0.3f;
    [Space(5)]
    [Header("BOOLEANS")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isRolling;
    [SerializeField] private bool isSliding;
    [Space(5)]
    [Header("MOVEMENT MULTS")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float aimSpeed = 5f;
    [SerializeField] private float speedMultOnJump = 0.9f;
    [SerializeField] private float speedChangeRate = 10f;  //Acceleration and deceleration
    [SerializeField] private float RotationSmoothTime = 0.15f;
    [SerializeField] private bool _rotateOnMove = true;
    private float _speed;


    // Collider height values of the character controller component when animations such as roll, slide are activated.
    [SerializeField] private float characterContorllerDefaultHeight = 1.8f;
    [SerializeField] private float characterContorllerAnimationHeight = 0.9f;

    [SerializeField] private float characterContorllerDefaultCenterY = 1f;
    [SerializeField] private float characterContorllerAnimationCenterY = 0.5f;

    private float _animationBlend; // Animation blend speed
    private float _targetRotation = 0.0f;  // Current Target Rotation
    private float _rotationVelocity; // SmoothDamp reference rotation velocity


    private void Awake()
    {
        //characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        //jump = GetComponent<Jump>();
    }
    private void Start()
    {
        gameInput = GameInput.Instance;

        Cursor.lockState = CursorLockMode.Locked;
        gameInput.OnSprintAction += GameInput_OnSprintAction; ;
        gameInput.OnVaultAction += GameInput_OnRollAction;
        gameInput.OnSlideAction += GameInput_OnSlideAction;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GameInput_OnSlideAction(object sender, EventArgs e)
    {
        if (!isRolling && !isSliding && GroundCheck() && _speed >= 5f)
        {
            isSliding = true;
            OnSlideAction?.Invoke(this, new OnSlideActionEventArgs
            {
                isSliding = isSliding
            });
        }
    }

    private void GameInput_OnRollAction(object sender, EventArgs e)
    {
        if (!isRolling && !isSliding && GroundCheck())
        {
            isRolling = true;
            OnRollAction?.Invoke(this, new OnRollActionEventArgs 
            { 
                isRolling = isRolling
            });

        }
    }

    private void GameInput_OnSprintAction(object sender, GameInput.OnSprintActionEventArgs e)
    {
        isSprinting = e.isSprint;
    }

    private void Move()
    {
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector2 movementVector = gameInput.GetMovementVectorNormalized();
        // If there is no input set the targetspeed to 0.
        if (movementVector == Vector2.zero)
        {
            targetSpeed = 0f;
        }

        // Reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = gameInput.GetMoveInputMagnitude();
        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.fixedDeltaTime * speedChangeRate);

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
        Vector3 inputDirection = new Vector3(movementVector.x, 0f ,movementVector.y).normalized;

        //note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (gameInput.GetMovementVectorNormalized() != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              Camera.main.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        rb.AddForce(targetDirection.normalized * (_speed * Time.fixedDeltaTime) +
                            new Vector3(0.0f, 0, 0.0f) * Time.fixedDeltaTime);


        OnSpeedChangeAction?.Invoke(this, new OnSpeedChangeActionEventArgs
        {
            speed = _speed
        });

        OnMotionBlendChangeAction?.Invoke(this, new OnMotionBlendChangeActionEventArgs
        {
            animationBlend = inputMagnitude
        });
        OnMotionBlendChangeAction?.Invoke(this, new OnMotionBlendChangeActionEventArgs
        {
            animationBlend = inputMagnitude
        });
    }

    public bool GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(transform.position + groundOffset, groundRadius, groundLayer);

        OnGrondedChangeAction?.Invoke(this, new OnGrondedChangeActionEventArgs
        {
            isGrounded = isGrounded
        });
        return isGrounded;
    }

    public bool GetIsRolling()
    {
        return isRolling;
    }

    public bool GetIsSliding()
    {
        return isSliding;
    }
    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);
    //}
}
