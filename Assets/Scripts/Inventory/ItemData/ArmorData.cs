using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArmorData : UniqueItemData
{
    [SerializeField] protected ArmorType armorType;
    [SerializeField] protected int defaultArmorPower;
    [SerializeField] protected int currentArmorPower;

    public ArmorData(string itemName, GameObject itemPrefab, ArmorType armorType, int defaultArmorPower, int currentArmorPower)
    {
        this.itemName = itemName;
        this.itemPrefab = itemPrefab;
        this.armorType = armorType;
        this.defaultArmorPower = defaultArmorPower;
        this.currentArmorPower = currentArmorPower;
    }

    public ArmorType GetArmorType()
    {
        return armorType;
    }

    public int GetCurrentArmorPower()
    {
        return currentArmorPower;
    }

    public int GetDefaultArmorPower()
    {
        return defaultArmorPower;
    }

    public void SetCurrentArmorPower(int newArmorPower)
    {
        currentArmorPower = newArmorPower;
    }
}
