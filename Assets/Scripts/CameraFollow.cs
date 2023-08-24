using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    private GameInput gameInput;

    public float rotationSpeed = 2.0f;
    public float smoothTime = 0.2f;
    public float pitchClampMin = -80.0f;
    public float pitchClampMax = 80.0f;
    public float distanceToTarget = 5f;
    private float currentYaw = 0.0f;
    private float currentPitch = 0.0f;
    private float yawVelocity = 0.0f;   
    private float pitchVelocity = 0.0f;


    private void Start()
    {
        gameInput = GameInput.Instance;
    }
    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // Get input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate new yaw and pitch
        currentYaw += mouseX * rotationSpeed;
        currentPitch -= mouseY * rotationSpeed;
        currentPitch = Mathf.Clamp(currentPitch, pitchClampMin, pitchClampMax);

        // Smoothly rotate the camera
        float newYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentYaw, ref yawVelocity, smoothTime);
        float newPitch = Mathf.SmoothDampAngle(transform.eulerAngles.x, currentPitch, ref pitchVelocity, smoothTime);

        // Apply rotation to the camera
        transform.eulerAngles = new Vector3(newPitch, newYaw, 0.0f);

        // Rotate the target object based on camera rotation
        Vector3 targetEulerAngles = target.eulerAngles;
        targetEulerAngles.y = newYaw;
        target.eulerAngles = targetEulerAngles;

        // Position the camera behind the target
        transform.position = target.position - transform.forward * distanceToTarget;
    }
}
