using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    private GameInput gameInput;
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

    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.5f;
    private float currentSensitivity;


    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void Start()
    {
        gameInput = GameInput.Instance;

        gameInput.OnAimAction += GameInput_OnAimAction;

        currentSensitivity = normalSensitivity;
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void GameInput_OnAimAction(object sender, System.EventArgs e)
    {
        isAiming = !isAiming;
        if (isAiming)
        {
            currentSensitivity = aimSensitivity;
        }
        else
        {
            currentSensitivity = normalSensitivity;
        }
        aimVirtucalCamera.gameObject.SetActive(isAiming);
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
}
