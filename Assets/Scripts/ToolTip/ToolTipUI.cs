using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    public static ToolTipUI Instance;
    private TextMeshProUGUI tooltipText;
    private RectTransform backgroundRectTransform;

   [SerializeField] private RectTransform canvasRectTransform;

    private GameInput gameInput;


    private void Awake()
    {
        Instance = this;
        backgroundRectTransform = GetComponentInChildren<RectTransform>();
        tooltipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameInput = GameInput.Instance;
    }

    private void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
            gameInput.GetMousePosition(), null, out Vector2 localPoint);

        transform.localPosition = localPoint;

        Vector2 anchoredPosition = transform.GetComponent<RectTransform>().anchoredPosition;

        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }

        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }

        // Apply the adjusted anchored position to the tooltip's RectTransform
        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
    }

    private void ShowTooltip(string tooltipString)
    {
        gameObject.SetActive(true);

        tooltipText.text = tooltipString;
        float textPaddingSize = 6f;
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
