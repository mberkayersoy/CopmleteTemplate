using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class ItemDataUI : MonoBehaviour, IPointerClickHandler
{
    private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private InventorySlot slotReference;
    GameInput gameInput;

    public event EventHandler OnDropedItemAction;
    private void Start()
    {
        gameInput = GameInput.Instance;
        rectTransform = GetComponent<RectTransform>();
        gameInput.OnItemDropAction += GameInput_OnItemDropAction;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(slotReference.GetItem().GetItemName()) && eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("onpointerclick: " + slotReference.GetItem());
            PlayerInventoryManager.Instance.DropItem(slotReference);
        }
    }
    private void GameInput_OnItemDropAction(object sender, GameInput.OnItemDropActionEventArgs e)
    {
        
    }

    public void SetItemData(InventorySlot inventorySlot)
    {
        if (inventorySlot == null) return;

        slotReference = inventorySlot;
        ItemData slotItem = inventorySlot.GetItem();
        itemNameText.text = slotItem.GetItemName();

        StackableItemData stackableItem = inventorySlot.GetItem() as StackableItemData;
        if (stackableItem != null)
        {
            if ((stackableItem.GetStackableType() != StackableType.NotStackable) && stackableItem.GetStackableType() != StackableType.Unique)
            {
                itemAmount.text = inventorySlot.currentCapacity.ToString();
            }
        }

    }

}
