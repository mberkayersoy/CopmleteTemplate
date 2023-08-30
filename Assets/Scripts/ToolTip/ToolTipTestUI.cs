using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipTestUI : MonoBehaviour
{   
    private void Start()
    {
        ToolTipUIEventTrigger listener = GetComponent<ToolTipUIEventTrigger>();

        if (listener != null)
        {
            listener.onPointerEnter += OnPointerEnterUIObject;
            listener.onPointerExit += OnPointerExitUIObject;
        }
    }

    private void OnPointerEnterUIObject()
    {
        ToolTipUI.ShowToolTipStatic(gameObject.name);
    }

    // This function is not Monobehavior function.
    private void OnPointerExitUIObject()
    {
        ToolTipUI.HideToolTipStatic();
    }
}