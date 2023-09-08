using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : Stackable
{
    [SerializeField] private AmmoData ammoData;

    public override ItemData GetItemData()
    {
        return ammoData;
    }
    public override void SetItemData(ItemData itemData)
    {
        ammoData = (AmmoData)itemData;
    }



}
