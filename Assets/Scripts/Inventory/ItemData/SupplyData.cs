using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupplyData : StackableItemData
{
    private SupplyType supplyType;

    public SupplyData(string itemName, int amount, StackableType stackableType, GameObject itemPrefab, SupplyType supplyType)
    {
        this.itemName = itemName;
        this.amount = amount;
        this.stackableType = stackableType;
        this.itemPrefab = itemPrefab;
        this.supplyType = supplyType;
    }

    public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }
    public SupplyType GetSupplyType()
    {
        return supplyType;
    }
}
