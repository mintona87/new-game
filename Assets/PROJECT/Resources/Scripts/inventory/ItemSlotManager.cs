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


    void Start()
    {
        
    }

   public void ItemSlotButtonPressed()
   {
        UseItem(itemName);
   }

    public void SetupItem(string ItemName)
    {
        itemName = ItemName;
        itemNameText.text = itemName;
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

    }
}
