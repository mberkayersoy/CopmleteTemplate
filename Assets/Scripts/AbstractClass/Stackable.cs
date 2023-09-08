using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : Item
{
    [SerializeField] protected StackableType stackableType;

    public override ItemData GetItemData()
    {
        throw new System.NotImplementedException();
    }

    public StackableType GetStackableType()
    {
        return stackableType;
    }

    public override void SetItemData(ItemData itemData)
    {
        throw new System.NotImplementedException();
    }
}

public enum StackableType
{
    Unique, // Unique's have own inventory slot.
    NotStackable,
    Light,
    Heavy,
    Shotgun,
    Sniper,
    Syringe,
    Medkit,
    ShildCell,
    ShildBaterry,
}
