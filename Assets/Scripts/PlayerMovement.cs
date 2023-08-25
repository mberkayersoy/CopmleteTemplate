using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // EVENTS
    public event EventHandler<OnSpeedChangeActionEventArgs> OnSpeedChangeAction;
    public event EventHandler<OnMotionBlendChangeActionEventArgs> OnMotionBlendChangeAction;
    public event EventHandler<OnGrondedChangeActionEventArgs> OnGrondedChangeAction;


    // EVENT ARGS
    public class OnSpeedChangeActionEventArgs : EventArgs { public float speed; }
    public class OnMotionBlendChangeActionEventArgs : EventArgs{ public float animationBlend; }
    public class OnGrondedChangeActionEventArgs : EventArgs { public bool isGrounded; }

    private CharacterController characterController;
    private GameInput gameInput;
    private Jump jump;

    [SerializeField] private Vector2 Look;
    [SerializeField] private bool isSprinting;
    [SerializeField] private Vector3 groundOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRadius = 0.3f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultOnJump = 0.9f;
    [SerializeField] private float _speed;
    [SerializeField] private float moveSpeedOnJump;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float speedChangeRate = 10f;  //Acceleration and deceleration
    [SerializeField] private float RotationSmoothTime = 0.12f;
    [SerializeField] private float Sensitivity = 1f;
    [SerializeField] private float characterContorllerDefaultHeight = 1.9f;
    [SerializeField] private float characterContorllerRollHeight = 1.2f;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    [SerializeField] private bool _rotateOnMove = true;


    private const float _threshold = 0.01f;
    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        jump = GetComponent<Jump>();
    }
    private void Start()
    {
        gameInput = GameInput.Instance;

        Cursor.lockState = CursorLockMode.Locked;
        gameInput.OnSprintAction += GameInput_OnSprintAction; ;

       _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void GameInput_OnSprintAction(object sender, GameInput.OnSprintActionEventArgs e)
    {
        isSprinting = e.isSprint;
    }

    private void Update()
    {
        Move();
        GroundCheck();
        Look.x = gameInput.GetMousePositionMovementDelta().x;
        Look.y = gameInput.GetMousePositionMovementDelta().y;
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
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

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

        if (GroundCheck())
        {
            // move the player
            characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                new Vector3(0.0f, jump.VerticalVelocity, 0.0f) * Time.deltaTime);
        }
        else
        {
            // move the player
            characterController.Move(targetDirection.normalized * (_speed * speedMultOnJump * Time.deltaTime) +
                             new Vector3(0.0f, jump.VerticalVelocity, 0.0f) * Time.deltaTime);
        }


        OnSpeedChangeAction?.Invoke(this, new OnSpeedChangeActionEventArgs
        {
            speed = _speed
        });

        OnMotionBlendChangeAction?.Invoke(this, new OnMotionBlendChangeActionEventArgs
        {
            animationBlend = inputMagnitude
        });
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        Vector2 mousePositionDelta = gameInput.GetMousePositionMovementDelta();
        // if there is an input and camera position is not fixed
        if (mousePositionDelta.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += mousePositionDelta.x * 1;
            _cinemachineTargetPitch += mousePositionDelta.y * 1;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);


        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
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

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + groundOffset, groundRadius);
    }
}
