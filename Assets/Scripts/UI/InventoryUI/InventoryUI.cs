using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private Inventory playerInventory;
    private GameInput gameInput;

    // Standart Items
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;

    // Unique Items
    [SerializeField] private Transform uniqueItemSlotContainer;
    // Helmet Item
    [SerializeField] private Transform helmetContainer;
    // Armor Item
    [SerializeField] private Transform armorContainer;
    // Shoes Item
    [SerializeField] private Transform shoesContainer;
    // Backpack Item
    [SerializeField] private Transform backpackContainer;
    // Weapon1 Item
    [SerializeField] private Transform weapon1Container;
    // Weapon2 Item
    [SerializeField] private Transform weapon2Container;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameInput = GameInput.Instance;

        gameInput.OnUIInventoryAction += GameInput_OnUIInventoryAction; ;
        Hide();
    }

    public void SetInventory(Inventory inventory)
    {
        if (inventory != null) 
        {
            playerInventory = inventory;
        }

        playerInventory.OnItemListChanged += PlayerInventory_onItemListChanged;
        playerInventory.OnUniqueItemChanged += PlayerInventory_OnUniqueItemChanged;
        //playerInventory.OnArmorChanged += PlayerInventory_OnArmorChanged;
        //playerInventory.OnHelmetChanged += PlayerInventory_OnHelmetChanged;
        //playerInventory.OnShoesChanged += PlayerInventory_OnShoesChanged;
        //playerInventory.OnBackpackChanged += PlayerInventory_OnBackpackChanged;
        //playerInventory.OnWeapon1Changed += PlayerInventory_OnWeapon1Changed;
        //playerInventory.OnWeapon2Changed += PlayerInventory_OnWeapon2Changed;
        RefreshInventoryItems();
    }

    private void PlayerInventory_OnUniqueItemChanged(object sender, Inventory.OnUniqueItemChangedEventArgs e)
    {
        UniqueItemData uniqueItem = e.uniqueItemSlot.GetItem() as UniqueItemData;

        if (uniqueItem is HelmetData)
        {
            RefreshHelmetSlot();
        }
        else if (uniqueItem is ArmorData)
        {
            RefreshArmorSlot();
        }
        else if (uniqueItem is ShoesData)
        {
            RefreshShoesSlot();
        }
        else if (uniqueItem is BackpackData)
        {
            RefreshBackpackSlot();
        }
        else if (uniqueItem is WeaponData)
        {
            RefreshWeapon1Slot();
            RefreshWeapon2Slot();
        }
    }

    //private void PlayerInventory_OnWeapon2Changed(object sender, System.EventArgs e)
    //{
    //    RefreshWeapon2Slot();
    //}

    //private void PlayerInventory_OnWeapon1Changed(object sender, System.EventArgs e)
    //{
    //    RefreshWeapon1Slot();
    //}

    //private void PlayerInventory_OnBackpackChanged(object sender, System.EventArgs e)
    //{
    //    RefreshBackpackSlot();
    //}

    //private void PlayerInventory_OnShoesChanged(object sender, System.EventArgs e)
    //{
    //    RefreshShoesSlot();
    //}

    //private void PlayerInventory_OnHelmetChanged(object sender, System.EventArgs e)
    //{
    //    RefreshHelmetSlot();
    //}

    //private void PlayerInventory_OnArmorChanged(object sender, System.EventArgs e)
    //{
    //    RefreshArmorSlot();
    //}

    private void RefreshArmorSlot()
    {
        armorContainer.GetComponent<ItemDataUI>().SetItemData(playerInventory.ArmorSlot);
    }
    private void RefreshHelmetSlot()
    {
        helmetContainer.GetComponent<ItemDataUI>().SetItemData(playerInventory.HelmetSlot);
    }
    private void RefreshShoesSlot()
    {
        shoesContainer.GetComponent<ItemDataUI>().SetItemData(playerInventory.ShoesSlot);
    }
    private void RefreshBackpackSlot()
    {
        backpackContainer.GetComponent<ItemDataUI>().SetItemData(playerInventory.BackpackSlot);
    }
    private void RefreshWeapon1Slot()
    {
        weapon1Container.GetComponent<ItemDataUI>().SetItemData(playerInventory.Weapon1Slot);
    }
    private void RefreshWeapon2Slot()
    {
        weapon2Container.GetComponent<ItemDataUI>().SetItemData(playerInventory.Weapon2Slot);
    }

    private void PlayerInventory_onItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
    {
        foreach (Transform child in itemSlotContainer) 
        {
            if(child == itemSlotTemplate) continue;

            Destroy(child.gameObject);
        }

        foreach (InventorySlot item in playerInventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.GetComponent<ItemDataUI>().SetItemData(item);
        }

        for (int i = 0; i < playerInventory.GetInventoryCapacity() - playerInventory.GetItemList().Count; i++)
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
        }
    }

    private void GameInput_OnUIInventoryAction(object sender, GameInput.OnUIInventoryActionEventArgs e)
    {
        if (e.isActive)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
    }
}
