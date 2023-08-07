using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleSelectionMenu : MonoBehaviour
{
    public PlayerUI playerUI;
    public TextMeshProUGUI titleText;
    public GameObject menuPanel;

    private Item selectedTitle;

    public void ShowMenu(Item title, Vector3 position)
    {
        Debug.Log("Showing menu for title: " + title.itemName);
        if (title == null)
        {
        Debug.LogError("Title is null!");
        return;
        }
        if (menuPanel == null)
        {
            Debug.LogError("Menu Panel is null!");
            return;
        }
        // Set the position of the panel
        menuPanel.transform.position = position;
        Debug.Log("Showing menu at position: " + position);
        selectedTitle = title;
        titleText.text = "Set " + title.itemName + " as Displayed Title?";
        menuPanel.SetActive(true);
    }

    public void OnYesClicked()
    {
        if (playerUI == null)
        {
        Debug.LogError("PlayerUI is not assigned!");
        return;
        }
        // Set the selected title as the displayed title
        playerUI.UpdatePlayerTitleWithItem(selectedTitle);
        menuPanel.SetActive(false);
    }

    public void OnNoClicked()
    {
        // Close the menu without changing the title
        menuPanel.SetActive(false);
    }
}
