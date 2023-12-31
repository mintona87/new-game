using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item 
{
    public string itemName;
    public string itemDescription;
    public int itemPrice;
    public int itemQuantity;
    public int itemId;
    public string spriteName;
    public Item(string name, int id, string description, int price, int quantity,string SpriteName)
    {
        itemName = name;
        itemId = id;
        itemDescription = description;
        itemQuantity = quantity;
        itemPrice = price;
        spriteName = SpriteName;
    }
}
