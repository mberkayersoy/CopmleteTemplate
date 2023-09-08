using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackpackData : UniqueItemData
{
    private int backpackMaxCapacity;


    public BackpackData(string itemName, GameObject itemPrefab, int backpackMaxCapacity)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
        this.backpackMaxCapacity = backpackMaxCapacity;

    }
    public BackpackData(string itemName, GameObject itemPrefab, int backpackMaxCapacity, Inventory inventory)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
        this.backpackMaxCapacity = backpackMaxCapacity;

        if (inventory != null)
        {
            inventory.SetInventorySlotCapacity(this.backpackMaxCapacity);
        }

    }

    public int GetBackpackMaxCapacity()
    {
        return backpackMaxCapacity;
    }
}
