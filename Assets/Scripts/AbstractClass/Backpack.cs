using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : Unique
{
    [SerializeField] private BackpackData backpackData;


    public override ItemData GetItemData()
    {
        return backpackData;
    }
    public override void SetItemData(ItemData itemData)
    {
        backpackData = (BackpackData)itemData;
    }
}
