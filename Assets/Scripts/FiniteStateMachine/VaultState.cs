using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultState : State
{
    public VaultState(ThirdPersonCharacterController sm) : base(sm)
    {
    }
    public override void OnStart()
    {
        playerController.InvokeOnJumpAction(false, RelevantAction.Vault);
    }

    public override void OnUpdate()
    {
        // After the player finishes the vault movement, switch to movementState.
        base.OnUpdate();
    }


    public override void OnExit()
    {
        base.OnExit();
    }
}
