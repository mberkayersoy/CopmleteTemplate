using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Stackable
{
    [SerializeField] private SupplyData supplyData;
    [SerializeField] private SupplyType supplyType;

    public override ItemData GetItemData()
    {
        return supplyData;
    }
    public override void SetItemData(ItemData itemData)
    {
        supplyData = (SupplyData)itemData;
    }
}

public enum SupplyType
{
    Health,
    Shild,
    Both
}

