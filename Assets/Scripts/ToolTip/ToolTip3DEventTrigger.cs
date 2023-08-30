using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip3DEventTrigger : MonoBehaviour
{
    public Action onPointerEnter;
    public Action onPointerExit;
    private void OnMouseEnter()
    {
        onPointerEnter?.Invoke();
    }
    private void OnMouseExit()
    {
        onPointerExit?.Invoke();
    }
}
