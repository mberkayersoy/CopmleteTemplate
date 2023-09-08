using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoes : Unique
{
    [SerializeField] private ShoesData shoesData;

    public override ItemData GetItemData()
    {
        return shoesData;
    }
    public override void SetItemData(ItemData itemData)
    {
        shoesData = (ShoesData)itemData;
    }
}
