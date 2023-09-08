using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoData : StackableItemData
{
    [SerializeField] private AmmoType ammoType;

    public AmmoData(string itemName, int amount, StackableType stackableType, GameObject itemPrefab, AmmoType ammoType)
    {
        this.itemName = itemName;
        this.amount = amount;
        this.stackableType = stackableType;
        this.itemPrefab = itemPrefab;
        this.ammoType = ammoType;
    }

    public AmmoType GetAmmoType()
    {
        return ammoType;
    }

    public void SetAmount(int takenAmount)
    {
        amount -= takenAmount;
    }

}
