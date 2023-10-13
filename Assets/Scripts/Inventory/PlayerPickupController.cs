using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupController : MonoBehaviour
{
    GameInput gameInput;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private CollectibleItem selectedCollectibleItem;
    private bool canTryPickUp;

    private void Start()
    {
        gameInput = GameInput.Instance;

        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (canTryPickUp)
        {
            TryPickup();
        }
    }

    void Update()
    {
        // Get Mouse position
        Vector3 mousePosition = gameInput.GetMousePosition();
        // Convert mouse position
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Get the object the mouse is pointing to
            GameObject hitObject = hit.collider.gameObject;

            // If the object is one of the collectible objects and the distance is appropriate, perform the operations
            if (hitObject.TryGetComponent(out CollectibleItem collectibleItem)) 
            {
                // Debug.Log("Collectibe object find");
                float distanceToObjects = Vector3.Distance(transform.position, hitObject.transform.position);
                if (distanceToObjects <= interactionDistance)
                {
                    hitObject.GetComponent<ToolTip3DEventTrigger>().OnCrosshairEnter();
                    canTryPickUp = true;
                    selectedCollectibleItem = collectibleItem;
                }
                else
                {
                    hitObject.GetComponent<ToolTip3DEventTrigger>().OnCrosshairEnter();
                    canTryPickUp = false;
                    selectedCollectibleItem = null;
                }
            }
            else
            {
                canTryPickUp = false;
                selectedCollectibleItem = null;
            }
        }
        else
        {
            canTryPickUp = false;
            selectedCollectibleItem = null;
        }
    }

    private void TryPickup()
    {
        if (selectedCollectibleItem == null) return;
   
        //Debug.Log(selectedCollectibleItem.GetItem().ItemName + " taken");
    }
}





