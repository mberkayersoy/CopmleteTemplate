using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private GameInput gameInput;
    [SerializeField] private ThirdPersonCharacterController playerController;
    [SerializeField] private ThirdPersonCameraController cameraController;
    [SerializeField] private EnvironmentScan environmentScan;

    [SerializeField] private Animator animator;

    public event EventHandler OnDisableCharacterController;
    public event EventHandler OnEnableCharacterController;

    // public event EventHandler OnRollComplete;

    // Animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDMovementToHang;
    private int animIDHangIdle;
    private int animIDVault;
    private int animIDHorizontal;
    private int animIDVertical;
    private void Awake()
    {
        AssignAnimationIDs();
    }

    private void Start()
    {
        gameInput = GameInput.Instance;
        playerController.OnSpeedChangeAction += PlayerController_OnSpeedChangeAction;
        playerController.OnMotionBlendChangeAction += PlayerController_OnMotionBlendChangeAction;
        playerController.OnGroundedChangeAction += PlayerController_OnGroundedChangeAction;
        playerController.OnFreeFallAction += PlayerController_OnFreeFallAction;
        playerController.OnJumpAction += PlayerController_OnSpaceKeyAction;
        playerController.OnHangAction += PlayerController_OnHangAction;
        playerController.OnHangIdleAction += PlayerController_OnHangIdleAction;
        //gameInput.OnVaultAction += GameInput_OnVaultAction;
        //gameInput.OnHangAction += GameInput_OnHangAction;
    }

    private void Update()
    {
        SetMovementVectorParams();
    }
    private void SetMovementVectorParams()
    {
        Vector2 movementVector = gameInput.GetMovementVectorNormalized();

        animator.SetFloat(animIDHorizontal, movementVector.x * Time.smoothDeltaTime);
        animator.SetFloat(animIDVertical, movementVector.y * Time.smoothDeltaTime);
    }

    private void PlayerController_OnHangIdleAction(object sender, ThirdPersonCharacterController.OnHangActionEventArgs e)
    {
        animator.SetBool(animIDHangIdle, e.isHanging);
    }

    private void PlayerController_OnHangAction(object sender, ThirdPersonCharacterController.OnHangActionEventArgs e)
    {
        animator.SetBool(animIDMovementToHang, e.isHanging);
    }

    private void PlayerController_OnSpaceKeyAction(object sender, ThirdPersonCharacterController.OnJumpActionEventArgs e)
    {
        switch (e.relevantAction)
        {
            case RelevantAction.Edge:
                animator.SetBool(animIDMovementToHang, true);
                break;
            case RelevantAction.Vault:
                animator.SetBool(animIDVault, true);
                break;
            case RelevantAction.StepUp:
                break;
            case RelevantAction.None:
                animator.SetBool(animIDJump, e.isJumping);
                break;
            default:
                break;
        }
    }
    private void PlayerController_OnFreeFallAction(object sender, ThirdPersonCharacterController.OnFreeFallEventArgs e)
    {
        animator.SetBool(animIDFreeFall, e.freeFall);
    }

    private void VaultDone(AnimationEvent animationEvent)
    {
        OnEnableCharacterController?.Invoke(this, EventArgs.Empty);
        animator.SetBool(animIDVault, false);
    }

    private void PlayerController_OnGroundedChangeAction(object sender, ThirdPersonCharacterController.OnGrondedChangeActionEventArgs e)
    {
        animator.SetBool(animIDGrounded, e.isGrounded);
    }

    private void PlayerController_OnMotionBlendChangeAction(object sender, ThirdPersonCharacterController.OnMotionBlendChangeActionEventArgs e)
    {
        animator.SetFloat(animIDMotionSpeed, e.animationBlend);
    }

    private void PlayerController_OnSpeedChangeAction(object sender, ThirdPersonCharacterController.OnSpeedChangeActionEventArgs e)
    {
        animator.SetFloat(animIDSpeed, e.speed);
    }

    private void ActivateRootMotion(AnimationEvent animationEvent)
    {
        animator.applyRootMotion = true;
    }
    private void DeactivateRootMotion(AnimationEvent animationEvent)
    {
        animator.applyRootMotion = false;
        Debug.Log("DeActivateRootMotion(): " + animator.hasRootMotion);
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDMovementToHang = Animator.StringToHash("MovementToHang");
        animIDHangIdle = Animator.StringToHash("HangIdle");
        animIDVault = Animator.StringToHash("Vault");
        animIDHorizontal = Animator.StringToHash("Horizontal");
        animIDVertical = Animator.StringToHash("Vertical");
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
