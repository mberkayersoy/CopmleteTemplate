using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip3DEventTrigger : MonoBehaviour
{
    public Action onPointerEnter;
    public Action onPointerExit;

    public event EventHandler<OnSelectedObjectChangedEventArgs> OnSelectedObjectChanged;
    public class OnSelectedObjectChangedEventArgs : EventArgs { public ToolTip3DEventTrigger selectedObject; }
    private void OnMouseEnter()
    {
        onPointerEnter?.Invoke();
        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedEventArgs
        {
            selectedObject = this
        });

    }
    private void OnMouseExit()
    {
        onPointerExit?.Invoke();

        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedEventArgs
        {
            selectedObject = null
        });
    }
}
