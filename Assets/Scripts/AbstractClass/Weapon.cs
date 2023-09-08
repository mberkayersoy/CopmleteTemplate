using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Unique
{
    [SerializeField] private WeaponData weaponData;

    public override ItemData GetItemData()
    {
        return weaponData;
    }

    public override void SetItemData(ItemData itemData)
    {
        weaponData = (WeaponData)itemData;
    }
}

public enum AmmoType
{
    Light,
    Heavy,
    Shotgun,
    Sniper
}
