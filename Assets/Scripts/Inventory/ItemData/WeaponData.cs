using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData : UniqueItemData
{
    private AmmoType ammoType;
    private int defaultMagSize;
    private int currentAmmoInsideMag;
    public WeaponData(string itemName, GameObject itemPrefab, AmmoType ammoType, int defaultMagSize, int currentAmmoInsideMag)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
        this.ammoType = ammoType;
        this.defaultMagSize = defaultMagSize;
        this.currentAmmoInsideMag = currentAmmoInsideMag;
    }

    public AmmoType GetAmmoType()
    {
        return ammoType;
    }
    public int GetCurrentAmmoInsideMag()
    {
        return currentAmmoInsideMag;
    }
    public void SetCurrentAmmoInsideMag(int newAmmoInsideMag)
    {
        currentAmmoInsideMag = newAmmoInsideMag;
    }

    public int GetDefaultMagSize()
    {
        return defaultMagSize;
    }
}
