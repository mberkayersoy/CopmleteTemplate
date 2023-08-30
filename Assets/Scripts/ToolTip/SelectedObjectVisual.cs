using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjectVisual : MonoBehaviour
{
    [SerializeField] private ToolTip3DEventTrigger selectedObject;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        selectedObject.OnSelectedObjectChanged += SelectedObject_OnSelectedObjectChanged;
        Hide();
    }

    private void SelectedObject_OnSelectedObjectChanged(object sender, ToolTip3DEventTrigger.OnSelectedObjectChangedEventArgs e)
    {
        if (e.selectedObject == selectedObject)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject item in visualGameObjectArray)
        {
            item.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject item in visualGameObjectArray)
        {
            item.SetActive(false);
        }
    }
}
