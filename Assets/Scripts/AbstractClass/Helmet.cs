using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helmet : Unique
{
    [SerializeField] private HelmetData helmetData;

    public override ItemData GetItemData()
    {
        return helmetData;
    }

    public override void SetItemData(ItemData itemData)
    {
        helmetData = (HelmetData)itemData;
    }
}
