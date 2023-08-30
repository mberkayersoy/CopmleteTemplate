using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    private GameInput gameInput;
    private PlayerMovement playerMovement;


    public event EventHandler<OnAimStateChangeEventArgs> OnAimStateChange;

    public class OnAimStateChangeEventArgs : EventArgs { public bool isAiming; }

    private const float _threshold = 0.01f;
    private const float sensitivityMultiplier = 100f;
    private bool isAiming;

    [SerializeField] private CinemachineVirtualCamera aimVirtucalCamera;

    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField] private float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    [SerializeField] private bool LockCameraPosition = false;

    [Header("SENSITIVITY")]
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.5f;
    [SerializeField] private float aimRotationSpeed = 15f;
    private float currentSensitivity;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;


    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void Start()
    {
        gameInput = GameInput.Instance;
        playerMovement = GetComponent<PlayerMovement>();

        gameInput.OnAimAction += GameInput_OnAimAction;

        currentSensitivity = normalSensitivity;
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }


    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;

        }

        if (isAiming)
        {
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimRotationSpeed);
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void GameInput_OnAimAction(object sender, System.EventArgs e)
    {

        isAiming = !isAiming;
        OnAimStateChange?.Invoke(this, new OnAimStateChangeEventArgs
        {
            isAiming = isAiming
        });
        aimVirtucalCamera.gameObject.SetActive(isAiming);

        if (isAiming)
        {
            currentSensitivity = aimSensitivity;
            playerMovement.SetRotateOnMove(isAiming);
        }
        else
        {
            currentSensitivity = normalSensitivity;
            playerMovement.SetRotateOnMove(!isAiming);
        }

    }

    private void CameraRotation()
    {
        Vector2 mousePositionDelta = gameInput.GetMousePositionMovementDelta();
        // if there is an input and camera position is not fixed
        if (mousePositionDelta.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += mousePositionDelta.x * currentSensitivity * sensitivityMultiplier * Time.deltaTime;
            _cinemachineTargetPitch += mousePositionDelta.y * currentSensitivity * sensitivityMultiplier * Time.deltaTime;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);


        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public bool GetIsAiming()
    {
        return isAiming;
    }
}
