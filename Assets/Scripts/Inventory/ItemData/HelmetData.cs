using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HelmetData : UniqueItemData
{
    public HelmetData(string itemName, GameObject itemPrefab)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
    }
}
