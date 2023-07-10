using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory 
{
    public List<Item> itemList;

    public Inventory()
    {
        itemList = new List<Item>();
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
    }

    public bool HasItem(Item item)
    {
        return itemList.Contains(item);
    }


}
