using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    private ItemData storedItem;
    public int slotMaxCapacity = 1;
    public int currentCapacity;

    // Unique Items
    public InventorySlot(ItemData itemData)
    {
        storedItem = itemData;
    }

    // Stackable Items
    public InventorySlot (Item item)
    {
        storedItem = CreateItemData(item);
        SetCapacity(storedItem);
    }

    private ItemData CreateItemData(Item item)
    {
        if (item is Ammo ammoItem)
        {
            AmmoData ammoData = ammoItem.GetItemData() as AmmoData;
            return new AmmoData(
                ammoData.GetItemName(),
                ammoData.GetAmount(),
                ammoData.GetStackableType(),
                GameAssets.Instance.GetAsset(ammoData.GetItemName()),
                ammoData.GetAmmoType());
        }
        else if (item is Supply supplyItem)
        {
            SupplyData supplyData = supplyItem.GetItemData() as SupplyData;
            return new SupplyData(
                supplyData.GetItemName(),
                supplyData.GetAmount(),
                supplyData.GetStackableType(),
                GameAssets.Instance.GetAsset(supplyData.GetItemName()),
                supplyData.GetSupplyType());
        }
        else if (item is Weapon weaponItem)
        {
            WeaponData weaponData = weaponItem.GetItemData() as WeaponData;
            return new WeaponData(
                weaponData.GetItemName(),
                weaponData.GetItemPrefab(),
                weaponData.GetAmmoType(),
                weaponData.GetDefaultMagSize(),
                weaponData.GetCurrentAmmoInsideMag());
        }
        else if (item is Armor armorItem)
        {
            ArmorData armorData = armorItem.GetItemData() as ArmorData;
            return new ArmorData(
                armorData.GetItemName(),
                GameAssets.Instance.GetAsset(armorData.GetItemName()),
                armorData.GetArmorType(),
                armorData.GetDefaultArmorPower(),
                armorData.GetCurrentArmorPower());
        }
        else if (item is Helmet helmetItem)
        {
            HelmetData helmetData = helmetItem.GetItemData() as HelmetData;
            return new HelmetData(
                helmetData.GetItemName(),
                GameAssets.Instance.GetAsset(helmetData.GetItemName()));
        }
        else if (item is Shoes shoesItem)
        {
            ShoesData shoesData = shoesItem.GetItemData() as ShoesData;
            return new ShoesData(
                shoesData.GetItemName(),
                GameAssets.Instance.GetAsset(shoesData.GetItemName()));
        }
        else if (item is Backpack backpackItem)
        {
            BackpackData backpackData = backpackItem.GetItemData() as BackpackData;
            return new BackpackData(
                backpackData.GetItemName(),
                GameAssets.Instance.GetAsset(backpackData.GetItemName()),
                backpackData.GetBackpackMaxCapacity());
        }
        else
        {
            Debug.LogError("There is no match!");
            return null;
        }

    }

    public ItemData GetItem()
    {
        return storedItem;
    }

    public void SetItem(ItemData newItem)
    {
        storedItem = newItem; ;
    }

    public void ReplaceStoredItem(Unique uniqueItem)
    {
        if (storedItem == null)
        {
            storedItem = null;
        }

        storedItem = CreateItemData(uniqueItem);
        uniqueItem.DestroySelf();
    }
    private void SetCapacity(ItemData storedItem)
    {
        Debug.Log(storedItem);
        StackableItemData stackableItem = storedItem as StackableItemData;
        switch (stackableItem.GetStackableType())    
        {
            case StackableType.NotStackable:
                slotMaxCapacity = 1;
                break;
            case StackableType.Light:
                slotMaxCapacity = 60;
                break;
            case StackableType.Heavy:
                slotMaxCapacity = 60;
                break;
            case StackableType.Shotgun:
                slotMaxCapacity = 16;
                break;
            case StackableType.Sniper:
                slotMaxCapacity = 24;
                break;
            case StackableType.Syringe:
                slotMaxCapacity = 4;
                break;
            case StackableType.Medkit:
                slotMaxCapacity = 2;
                break;
            case StackableType.ShildCell:
                slotMaxCapacity = 4;
                break;
            case StackableType.ShildBaterry:
                slotMaxCapacity = 2;
                break;
            default:
                break;
        }

        currentCapacity += storedItem.GetAmount();
    }
}
