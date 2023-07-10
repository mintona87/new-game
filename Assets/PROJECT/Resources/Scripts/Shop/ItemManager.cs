using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemManager : MonoBehaviour
{

    PlayfabManager playfabManager;

    public int itemPrice;
    public int itemId;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    public Image itemImage;
    public Sprite itemSprite;

    Item thisItem;
    ShopManager shopManager;

    private void Awake()
    {
        playfabManager = FindObjectOfType<PlayfabManager>();
        shopManager = FindObjectOfType<ShopManager>();
        
    }

    void Start()
    {
        itemPriceText.text = itemPrice.ToString() + " Gold";

        thisItem = new Item(itemNameText.text,itemId,itemDescriptionText.text, itemPrice);
    }

    
    public void BuyButtonPressed()
    {
        if (playfabManager.GetPlayerSavedData().Gold >= itemPrice)
        {
            playfabManager.GetPlayerSavedData().Gold -= itemPrice;

            playfabManager.AddItemToInventory(thisItem);

            shopManager.UpdatePlayerTextGold();

            playfabManager.SavePlayerSavedData(playfabManager.GetPlayerSavedData());
            playfabManager.SaveIventoryData(playfabManager.GetInventoryData());
        }
    }
}
