using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Todo: some scriptable objects to read data.
    public void OnPointerEnter(PointerEventData eventData)
    {

        ToolTip.ShowToolTipStatic(gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.HideToolTipStatic();
    }
}
