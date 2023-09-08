using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StackableItemData : ItemData
{
    [SerializeField] protected StackableType stackableType;

    public StackableType GetStackableType()
    {
        return stackableType;
    }

}
