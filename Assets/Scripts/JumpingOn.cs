using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JumpingOn : MonoBehaviour
{
    public event EventHandler<OnJumpingOnActionStateEventArgs> OnJumpingOnActionState;

    public class OnJumpingOnActionStateEventArgs : EventArgs
    {
        public bool isJumpingOn;
    }

    [SerializeField] private LayerMask jumpingOnLayer;
    [SerializeField] private Transform upRayPoint;
    [SerializeField] private  Transform vaultRay;
    private float playerHeight = 2f;
    private float playerRadius = 1f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private bool isJumpingOn;
    public bool showDebug;

    private void Update()
    {
        JumpingOnAction();
    }

    private void JumpingOnAction()
    {
       // Debug.DrawRay(upRayPoint.position, upRayPoint.TransformDirection(Vector3.down) * 2, Color.red);
        if (Physics.Raycast(vaultRay.transform.position, vaultRay.transform.forward, out RaycastHit firstHit, 1f, jumpingOnLayer))
        {

            if (Physics.Raycast(upRayPoint.position, upRayPoint.TransformDirection(Vector3.down), out RaycastHit secondHit, 10f))
            {
                //Debug.Log("up ray hit");
                if (Input.GetKeyDown(KeyCode.R))
                {
                    print("ACTION JUMP ON");
                    GetComponent<CharacterController>().enabled = false;
                    StartCoroutine(LerpRelocation(secondHit.point));
                }
            }
        }

    }
    private void OnDrawGizmos()
    {
        if (!showDebug) return;
        //if (Physics.Raycast(vaultRay.transform.position, vaultRay.transform.forward, out RaycastHit firstHit, 1f, jumpingOnLayer))
        //{
        //    Gizmos.DrawSphere(firstHit.point, 0.1f);

        //    //if (Physics.Raycast(firstHit.point + (-firstHit.normal * playerRadius), Vector3.up, out RaycastHit secondHit, playerHeight))
        //    //{
        //    //    Gizmos.DrawSphere(secondHit.point, 0.1f);
        //    //}
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawSphere(upRayPoint.position, 0.1f);
 
        //    if (Physics.Raycast(upRayPoint.position, upRayPoint.TransformDirection(Vector3.down), out RaycastHit downHit, Mathf.Infinity, 7))
        //    {
        //        Gizmos.DrawSphere(downHit.point, 0.1f);

        //    }
        //}
    }

    IEnumerator LerpRelocation(Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        OnJumpingOnActionState?.Invoke(this, new OnJumpingOnActionStateEventArgs
        {
            isJumpingOn = true
        });

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        GetComponent<CharacterController>().enabled = true;
        transform.position = targetPosition;
        OnJumpingOnActionState?.Invoke(this, new OnJumpingOnActionStateEventArgs
        {
            isJumpingOn = false
        });
    }
}
