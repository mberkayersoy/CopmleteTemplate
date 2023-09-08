using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Inventory
{
    public Inventory(HelmetData helmetData, ArmorData armorData, ShoesData shoesData, BackpackData backpackData)
    {
        itemList = new List<InventorySlot>();
        HelmetSlot = new InventorySlot(helmetData);
        ArmorSlot = new InventorySlot(armorData);
        ShoesSlot = new InventorySlot(shoesData);
        BackpackSlot = new InventorySlot(backpackData);

    }

    public void FirstInvokes()
    {
        OnUniqueItemChanged?.Invoke(this, new OnUniqueItemChangedEventArgs
        {
            uniqueItemSlot = HelmetSlot
        });
        OnUniqueItemChanged?.Invoke(this, new OnUniqueItemChangedEventArgs
        {
            uniqueItemSlot = ArmorSlot
        });
        OnUniqueItemChanged?.Invoke(this, new OnUniqueItemChangedEventArgs
        {
            uniqueItemSlot = ShoesSlot
        });
        OnUniqueItemChanged?.Invoke(this, new OnUniqueItemChangedEventArgs
        {
            uniqueItemSlot = BackpackSlot
        });
    }

    private int inventorySlotCapacity = 10;
    private List<InventorySlot> itemList;

    private List<InventorySlot> uniqueItemList;

    private InventorySlot helmetSlot;
    private InventorySlot armorSlot;
    private InventorySlot shoesSlot;
    private InventorySlot backpackSlot;
    private InventorySlot weapon1Slot;
    private InventorySlot weapon2Slot;

    public InventorySlot HelmetSlot { get => helmetSlot; set => helmetSlot = value ; }
    public InventorySlot ArmorSlot { get => armorSlot; set => armorSlot = value; }
    public InventorySlot ShoesSlot { get => shoesSlot; set => shoesSlot = value; }
    public InventorySlot BackpackSlot { get => backpackSlot; set => backpackSlot = value; }
    public InventorySlot Weapon1Slot { get => weapon1Slot; set => weapon1Slot = value; }
    public InventorySlot Weapon2Slot { get => weapon2Slot; set => weapon2Slot = value; }

    public event EventHandler<OnDropItemEventArgs> OnDropItem;
    public class OnDropItemEventArgs : EventArgs
    {
        public InventorySlot dropedSlot;
    }
    public event EventHandler OnItemListChanged;
    public event EventHandler<OnUniqueItemChangedEventArgs> OnUniqueItemChanged;
    public class OnUniqueItemChangedEventArgs : EventArgs
    {
        public InventorySlot uniqueItemSlot;
    }
    public event EventHandler OnHelmetChanged;
    public event EventHandler OnArmorChanged;
    public event EventHandler OnShoesChanged;
    public event EventHandler OnBackpackChanged;
    public event EventHandler OnWeapon1Changed;
    public event EventHandler OnWeapon2Changed;

    public int GetInventoryCapacity()
    {
        return inventorySlotCapacity;
    }

    public void SetInventorySlotCapacity(int newCapacity)
    {
        inventorySlotCapacity = newCapacity;
    }

    public List<InventorySlot> GetItemList()
    {
        return itemList;
    }

    public List<InventorySlot> GetUniqueItemList()
    {
        return uniqueItemList;
    }

    public void AddItem(Item addedItem)
    {
        if (addedItem is Unique uniqueItem)
        {
            ReplaceUniqueItem(uniqueItem);

            return;
        }
        bool itemAdded = TryAddToExistingSlot(addedItem as Stackable);

        if (!itemAdded)
        {
            CreateNewSlotForItem(addedItem);
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }
    public void RemoveItem(InventorySlot dropedItem)
    {
        itemList.Remove(dropedItem);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveUniqueItem(InventorySlot dropedUniqueItem)
    {
        if (dropedUniqueItem == ArmorSlot)
        {
            ArmorSlot.SetItem(null);
        }
    }

    private bool TryAddToExistingSlot(Stackable addedItem)
    {
        foreach (InventorySlot existingSlot in itemList)
        {
            StackableItemData stackableItem = existingSlot.GetItem() as StackableItemData;

            if (stackableItem != null)
            {
                StackableType itemType = stackableItem.GetStackableType();
            
                if (IsItemStackableTypeMatch(itemType, addedItem.GetStackableType()))
                {
                    int availableCapacity = existingSlot.slotMaxCapacity - existingSlot.currentCapacity;

                    if (availableCapacity <= 0)
                    {
                        //This slot capacity is full, can add anything.
                        continue;
                    }

                    if (addedItem.GetAmount() <= availableCapacity)
                    {
                        existingSlot.currentCapacity += addedItem.GetAmount();
                        addedItem.DestroySelf();
                        return true;
                    }
                    else
                    {
                        existingSlot.currentCapacity += availableCapacity;
                        //addedItem.GetAmount() -= availableCapacity;
                        addedItem.SetAmount(addedItem.GetAmount() - availableCapacity);
                    }
                }
            }
        }

        return false;
    }

    private bool IsItemStackableTypeMatch(StackableType itemA, StackableType itemB)
    {
        return  itemA == itemB;
    }

    private void CreateNewSlotForItem(Item addedItem)
    {
        if (itemList.Count == inventorySlotCapacity) return;
        InventorySlot inventorySlot = new InventorySlot(addedItem);
        itemList.Add(inventorySlot);
        addedItem.DestroySelf();
    }

    private void ReplaceUniqueItem(Unique newItem)
    {
        if (newItem is Armor armorItem)
        {
            if (!string.IsNullOrEmpty(armorSlot.GetItem().GetItemName()))
            {
                OnDropItem?.Invoke(this, new OnDropItemEventArgs
                {
                    dropedSlot = armorSlot

                });
            }

            armorSlot.ReplaceStoredItem(armorItem);
        }
        else if (newItem is Helmet helmetItem)
        {
            OnDropItem?.Invoke(this, new OnDropItemEventArgs
            {
                dropedSlot = helmetSlot

            });
            helmetSlot.ReplaceStoredItem(helmetItem);
        }
        else if (newItem is Backpack backpackItem)
        {
            OnDropItem?.Invoke(this, new OnDropItemEventArgs
            {
                dropedSlot = backpackSlot

            });
            BackpackSlot.ReplaceStoredItem(backpackItem);
        }
        else if (newItem is Shoes shoesItem)
        {
            OnDropItem?.Invoke(this, new OnDropItemEventArgs
            {
                dropedSlot = shoesSlot

            });
            ShoesSlot.ReplaceStoredItem(shoesItem);
        }

    }
}
