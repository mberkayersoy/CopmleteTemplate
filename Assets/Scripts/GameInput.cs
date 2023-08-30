using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public event EventHandler OnPlayerMovementAction;
    public event EventHandler OnJumpAction;
    public event EventHandler OnMousePositionDeltaAction;
    public event EventHandler OnRollAction;
    public event EventHandler OnAimAction;
    public event EventHandler OnSlideAction;
    public event EventHandler<OnSprintActionEventArgs> OnSprintAction;
    public event EventHandler OnMousePosition;
    public class OnSprintActionEventArgs : EventArgs{ public bool isSprint; }

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Movement.performed += Movement_performed;
        playerInputActions.Player.Sprint.performed += Sprint_performed;
        playerInputActions.Player.MouseDeltaPosition.performed += MouseDeltaPosition_performed;
        playerInputActions.Player.Roll.performed += Roll_performed;
        playerInputActions.Player.Aim.performed += Aim_performed;
        playerInputActions.Player.Slide.performed += Slide_performed;
        playerInputActions.Player.ToolTip.performed += ToolTip_performed;
    }

    private void ToolTip_performed(InputAction.CallbackContext obj)
    {
        OnMousePosition?.Invoke(this, EventArgs.Empty);
    }

    private void Slide_performed(InputAction.CallbackContext obj)
    {
        OnSlideAction?.Invoke(this, EventArgs.Empty);
    }

    private void Aim_performed(InputAction.CallbackContext obj)
    {
        OnAimAction?.Invoke(this, EventArgs.Empty);
    }

    private void Roll_performed(InputAction.CallbackContext obj)
    {
        OnRollAction?.Invoke(this, EventArgs.Empty);
    }

    private void MouseDeltaPosition_performed(InputAction.CallbackContext obj)
    {
        OnMousePositionDeltaAction?.Invoke(this, EventArgs.Empty);
        GetMousePositionMovementDelta();
    }

    private void Sprint_performed(InputAction.CallbackContext obj)
    {
        OnSprintAction?.Invoke(this, new OnSprintActionEventArgs 
        {
            isSprint = playerInputActions.Player.Sprint.IsPressed()
        });
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        OnPlayerMovementAction?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public float GetMoveInputMagnitude()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        return inputVector.magnitude;
    }
    public Vector2 GetMousePositionMovementDelta()
    {
        Vector2 inputVector = playerInputActions.Player.MouseDeltaPosition.ReadValue<Vector2>();
        //Debug.Log(inputVector);

        return inputVector;
    }

    public Vector2 GetMousePosition()
    {
        Vector2 mousePosition = playerInputActions.Player.ToolTip.ReadValue<Vector2>();

        return mousePosition;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Jump.performed -= Jump_performed;
        playerInputActions.Player.Movement.performed -= Movement_performed;

        playerInputActions.Dispose();
    }
}
