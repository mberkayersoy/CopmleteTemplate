using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipTestWorld : MonoBehaviour
{
    ToolTip3DEventTrigger listener;
    private void Start()
    {
        listener = GetComponent<ToolTip3DEventTrigger>();

        if (listener != null)
        {
            listener.onPointerEnter += OnCrosshairEnter;
            listener.onPointerExit += OnCrosshairExit;
        }
    }

    private void OnCrosshairEnter()
    {
        ToolTipWorld.ShowToolTipStatic(gameObject.name, transform.position + new Vector3(0, 0.3f, 0));
    }

    private void OnCrosshairExit()
    {
        ToolTipWorld.HideToolTipStatic();
    }
}
