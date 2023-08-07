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
    string itemSpriteName;

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

    public void SetupItem(PlayerManager owner, string ItemName, Item Item,string ItemSpriteName)
    {
        itemName = ItemName;
        itemNameText.text = itemName;
        item = Item;
        ownerPlayerManager = owner;
        itemSpriteName = ItemSpriteName;

        UpdateItemUI();
    }


    void UseItem(string ItemName)
    {
        if (itemName == "Alpha Caller" || itemName == "Zeitaku Zealot") // Titles
        {
            OnTitleClicked(); // Show the title selection menu
            return; // Don't remove the item
        }

        switch (ItemName) 
        {
            case "testItem1":
                break;
            case "testItem2":
                break;
        }
        ownerPlayerManager.playfabManager.RemoveItemFromInventory(item); // Remove other items
        UpdateItemUI();
    }

    void UpdateItemUI()
    {

        itemImage.sprite = Resources.Load<Sprite>("Sprites/CustomUIimages/"+ itemSpriteName);
        itemQuantityText.text = item.itemQuantity.ToString();
        if (item.itemQuantity <= 0)
        {
            Destroy(gameObject);
        }

    }


    public void OnTitleClicked()
    {
        Debug.Log("Title clicked: " + itemName);
        Debug.Log("Item details: " + item.itemName + ", " + item.itemDescription);
        if (itemName == "Alpha Caller" || itemName == "Zeitaku Zealot") // Adjust as needed
        {
            // Get the position of the clicked item
            Vector3 position = transform.position;

            // Show the title selection menu at the clicked position
            FindObjectOfType<TitleSelectionMenu>().ShowMenu(item, position);
        }
    }





}
