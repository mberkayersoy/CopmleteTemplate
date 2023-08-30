using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipTestWorld : MonoBehaviour
{
    private void Start()
    {
        ToolTip3DEventTrigger listener = GetComponent<ToolTip3DEventTrigger>();

        if (listener != null)
        {
            listener.onPointerEnter += OnCrosshairEnter;
            listener.onPointerExit += OnCrosshairExit;
        }
    }

    private void OnCrosshairEnter()
    {
        ToolTipWorld.ShowToolTipStatic(gameObject.name);
    }

    private void OnCrosshairExit()
    {
        ToolTipWorld.HideToolTipStatic();
    }
}
