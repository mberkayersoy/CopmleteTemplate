using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//LICENSE, YOU ARE LEGALLY REQUIRED TO LIKE THE VIDEO IF YOU COPY THIS CODE :) (i promise its legally binding, 50 years prison minimum)
public class Vaulting : MonoBehaviour
{
    public event EventHandler<OnVaultActionStateEventArgs> OnVaultActionState;

    public class OnVaultActionStateEventArgs : EventArgs
    {
        public bool isVaulting;
    }

    [SerializeField] private LayerMask vaultLayer;
    public Transform vaultRay;
    private float playerHeight = 2f;
    private float playerRadius = 1f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private bool isVaulting;
    public bool showDebug;

    void Update()
    {
        Vault();
    }

    //private void OnDrawGizmos()
    //{
    //    if (!showDebug) return;

    //    if (Physics.Raycast(vaultRay.transform.position, vaultRay.transform.forward, out RaycastHit firstHit, 1f, vaultLayer))
    //    {
           
    //        Gizmos.DrawSphere(firstHit.point, 0.1f);
    //        if (Physics.Raycast(firstHit.point + (-firstHit.normal * playerRadius), Vector3.down, out RaycastHit secondHit, playerHeight))
    //        {
               
    //            Gizmos.DrawSphere(secondHit.point, 0.1f);
    //        }
    //    }
    //}

    private void Vault()
    {

        if (Physics.Raycast(vaultRay.transform.position, vaultRay.transform.forward, out RaycastHit firstHit, 1f, vaultLayer))
        {
                //print("vaultable in front");

            if (Physics.Raycast(firstHit.point + (-firstHit.normal * playerRadius), Vector3.down, out RaycastHit secondHit, playerHeight))
            {         
                //print("found place to land");
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    GetComponent<CharacterController>().enabled = false;
                    StartCoroutine(LerpRelocation(secondHit.point));
                }
            }
        }
    }

    IEnumerator LerpRelocation(Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        OnVaultActionState?.Invoke(this, new OnVaultActionStateEventArgs
        {
            isVaulting = true
        });

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        GetComponent<CharacterController>().enabled = true;
        transform.position = targetPosition;
        OnVaultActionState?.Invoke(this, new OnVaultActionStateEventArgs
        {
            isVaulting = false
        });
    }
}