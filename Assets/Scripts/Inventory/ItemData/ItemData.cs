using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    [SerializeField] protected string itemName;

    [SerializeField] protected GameObject itemPrefab;
    [SerializeField] protected int amount;

    public int GetAmount()
    {
        return amount;
    }

    public GameObject GetItemPrefab()
    {
        return itemPrefab;
    }
    public string GetItemName()
    {
        return itemName;
    }

}
