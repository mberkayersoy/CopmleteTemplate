using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipUIEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action onPointerEnter;
    public Action onPointerExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        onPointerExit?.Invoke();
    }
}
