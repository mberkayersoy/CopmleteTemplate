using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ToolTipWorld : MonoBehaviour
{
    public static ToolTipWorld Instance;

    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private RectTransform backgroundRectTransform;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideToolTipStatic();
    }
    private void ShowTooltip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        float textPaddingSize = 0.1f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f,
            tooltipText.preferredHeight + textPaddingSize * 2f);

        backgroundRectTransform.sizeDelta = backgroundSize;

    }
    private void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowToolTipStatic(string tooltipString)
    {
        Instance.ShowTooltip(tooltipString);
    }

    public static void HideToolTipStatic()
    {
        Instance.HideToolTip();
    }
}
