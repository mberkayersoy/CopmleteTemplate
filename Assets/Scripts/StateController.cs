using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    IState currentState;
    void Update()
    {
        currentState.UpdateState();
    }
    public void ChangeState(IState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }
}
