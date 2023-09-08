using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected GameObject itemPrefab;
    [SerializeField] protected int amount;

    public abstract void SetItemData(ItemData itemData);
    public abstract ItemData GetItemData();
    public int GetAmount()
    {
        return amount;
    }

    public void SetAmount(int newAmount)
    {
        amount = newAmount;
    }
    
    public GameObject GetItemPrefab()
    {
        return itemPrefab;
    }
    public string GetItemName()
    {
        return itemName;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}


