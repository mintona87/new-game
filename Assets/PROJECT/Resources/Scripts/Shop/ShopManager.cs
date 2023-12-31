using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI playerGoldText;

    PlayfabManager playfabManager;

    private void Awake()
    {
        playfabManager = FindObjectOfType<PlayfabManager>();
    }

    private void Start()
    {
        UpdatePlayerTextGold();
    }

    public void UpdatePlayerTextGold()
    {
        GoldBalanceChanged();
        //playfabManager.GetLoadedPlayerDatas();
    }

    private void GoldBalanceChanged()
    {
        playerGoldText.text = "Gold : " + playfabManager.GetPlayerSavedData().Gold.ToString();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    
}
