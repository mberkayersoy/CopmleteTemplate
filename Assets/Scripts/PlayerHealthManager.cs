using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private PlayerInventoryManager playerInventory;
    private int curretArmorPower;
    private int maxArmorPower;
    private void Start()
    {
        playerInventory = GetComponent<PlayerInventoryManager>();

        //curretArmorPower = playerInventory.GetInventory().GetUniqueItemList();
    }
}
