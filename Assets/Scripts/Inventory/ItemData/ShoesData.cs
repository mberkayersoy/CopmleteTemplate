using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShoesData : UniqueItemData
{
    public ShoesData(string itemName, GameObject itemPrefab)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
    }
}
