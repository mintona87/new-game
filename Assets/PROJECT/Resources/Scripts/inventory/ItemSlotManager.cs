using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotManager : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemQuantityText;

    public Image itemImage;

     string itemName;

    Item item;

    PlayerManager ownerPlayerManager;

    void Start()
    {
        
    }

   public void ItemSlotButtonPressed()
   {
        UseItem(itemName);
   }

    public void SetupItem(PlayerManager owner, string ItemName, Item Item)
    {
        itemName = ItemName;
        itemNameText.text = itemName;
        item = Item;
        itemQuantityText.text = item.itemQuantity.ToString();
        ownerPlayerManager = owner;
    }


    void UseItem(string ItemName)
    {
        switch (ItemName) 
        {
            case "testItem1":
                break;
            case "testItem2":
                break;
        }
        ownerPlayerManager.playfabManager.RemoveItemFromInventory(item);
        UpdateItemUI();
    }
    void UpdateItemUI()
    {
        itemQuantityText.text = item.itemQuantity.ToString();
        if (item.itemQuantity <= 0)
        {
            Destroy(gameObject);
        }

    }
}
