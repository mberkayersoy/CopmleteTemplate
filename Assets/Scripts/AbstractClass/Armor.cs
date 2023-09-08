using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Unique
{
    [SerializeField] private ArmorData armorData;

    public override ItemData GetItemData()
    {
        return armorData;
    }
    public override void SetItemData(ItemData itemData)
    {
        armorData = (ArmorData)itemData;
    }

    //[SerializeField] protected ArmorType armorType;
    //[SerializeField] protected int defaultArmorPower;
    //[SerializeField] protected int currentArmorPower;


    //public ArmorType GetArmorType()
    //{
    //    return armorType;
    //}

    //public int GetCurrentArmorPower()
    //{
    //    return currentArmorPower;
    //}

    //public int GetDefaultArmorPower()
    //{
    //    return defaultArmorPower;
    //}

    //public void SetCurrentArmorPower(int newArmorPower)
    //{
    //    currentArmorPower = newArmorPower;
    //}
}

public enum ArmorType
{
    Grey,
    Blue,
    Purple,
    Yellow,
    Red
}
