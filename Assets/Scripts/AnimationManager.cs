using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private enum State
    {
        Knife,
        Bow,
    }

    private GameInput gameInput;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Jump jump;
    [SerializeField] private ThirdPersonCameraController cameraController;

    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController[] overrideControllers;

//    public event EventHandler OnRollComplete;

    // Animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDRoll;
    private int animIDSlide;
    private int animIDAiming;

    private void Awake()
    {
        AssignAnimationIDs();
        cameraController = GetComponent<ThirdPersonCameraController>();
    }

    private void Start()
    {
        gameInput = GameInput.Instance;


        playerMovement.OnSlideAction += PlayerMovement_OnSlideAction;
        playerMovement.OnRollAction += PlayerMovement_OnRollAction; ;
        playerMovement.OnSpeedChangeAction += PlayerMovement_OnSpeedChangeAction;
        playerMovement.OnMotionBlendChangeAction += PlayerMovement_OnMotionBlendChangeAction;
        playerMovement.OnGrondedChangeAction += PlayerMovement_OnGrondedChangeAction;
        cameraController.OnAimStateChange += CameraController_OnAimStateChange;
        jump.OnFreeFallAction += Jump_OnFreeFallAction;
        jump.OnJumpAction += Jump_OnJumpAction;
        

    }

    private void CameraController_OnAimStateChange(object sender, ThirdPersonCameraController.OnAimStateChangeEventArgs e)
    {
        animator.SetBool(animIDAiming, e.isAiming);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetAnimator(State.Knife);
        }
    }


    private void PlayerMovement_OnSlideAction(object sender, PlayerMovement.OnSlideActionEventArgs e)
    {
        animator.SetBool(animIDSlide, e.isSliding);
    }

    private void PlayerMovement_OnRollAction(object sender, PlayerMovement.OnRollActionEventArgs e)
    {
        animator.SetBool(animIDRoll, e.isRolling);
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

    private void Jump_OnJumpAction(object sender, Jump.OnJumpActionEventArgs e)
    {
        animator.SetBool(animIDJump, e.isJumping);
    }

    private void Jump_OnFreeFallAction(object sender, Jump.OnFreeFallEventArgs e)
    {
        animator.SetBool(animIDFreeFall, e.freeFall);
    }

    private void PlayerMovement_OnGrondedChangeAction(object sender, PlayerMovement.OnGrondedChangeActionEventArgs e)
    {
        animator.SetBool(animIDGrounded, e.isGrounded);
    }

    private void PlayerMovement_OnMotionBlendChangeAction(object sender, PlayerMovement.OnMotionBlendChangeActionEventArgs e)
    {
        animator.SetFloat(animIDMotionSpeed, e.animationBlend);
    }

    private void PlayerMovement_OnSpeedChangeAction(object sender, PlayerMovement.OnSpeedChangeActionEventArgs e)
    {
        animator.SetFloat(animIDSpeed, e.speed);
    }

    private void SetAnimator(State animatorState)
    {
        //animator.CrossFade(animatorState.ToString(), 1f);
        animator.runtimeAnimatorController = overrideControllers[(int)animatorState];
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDRoll = Animator.StringToHash("Roll");
        animIDSlide = Animator.StringToHash("Slide");
        animIDAiming = Animator.StringToHash("Aim");
    }

}
