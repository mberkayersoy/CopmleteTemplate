using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : MonoBehaviour
{
    public static PlayerInventoryManager Instance;


    private Inventory inventory;

    private void Awake()
    {
        Instance = this;

        HelmetData helmetData = new HelmetData(
            "GreyHelmet",
            Resources.Load<GameObject>("GreyHelmet"));

        ArmorData armorData = new ArmorData(
            "ArmorGrey",
            Resources.Load<GameObject>("ArmorGrey"),
            ArmorType.Grey,
            50,
            10);

        ShoesData shoesData = new ShoesData(
            "GreyShoes",
            Resources.Load<GameObject>("GreyShoes"));

        BackpackData backpackData = new BackpackData(
            "GreyBackpack",
            Resources.Load<GameObject>("GreyBackpack"),
            12,
            inventory);

        inventory = new Inventory(helmetData, armorData, shoesData, backpackData);

    }
    private void Start()
    {
        InventoryUI.Instance.SetInventory(inventory);
        inventory.FirstInvokes();

        inventory.OnDropItem += Inventory_OnDropItem;
    }

    private void Inventory_OnDropItem(object sender, Inventory.OnDropItemEventArgs e)
    {
        DropItem(e.dropedSlot);
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public void DropItem(InventorySlot dropedSlot)
    {
        ItemData dropedItemData = dropedSlot.GetItem();
        GameObject dropedObject = dropedItemData.GetItemPrefab();
        GameObject dropedItem = Instantiate(dropedObject, new Vector3(0, 2, 0), Quaternion.identity);
        dropedItem.GetComponent<Item>().SetItemData(dropedItemData);

        if (dropedItemData is UniqueItemData)
        {
            inventory.RemoveUniqueItem(dropedSlot);
        }
        else
        {   
            inventory.RemoveItem(dropedSlot);
        }
    }
}
