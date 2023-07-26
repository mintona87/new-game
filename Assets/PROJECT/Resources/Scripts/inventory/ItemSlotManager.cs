using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotManager : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;

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
        UpdateItemUI();
    }
    void UpdateItemUI()
    {
        ownerPlayerManager.playfabManager.RemoveItemFromInventory(item);
        Destroy(gameObject);

    }
}
