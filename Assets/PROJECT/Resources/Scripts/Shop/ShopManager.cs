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
        playfabManager.OnSavedDataChanged += GoldBalanceChanged;
    }

    private void Start()
    {
        UpdatePlayerTextGold();
    }

    public void UpdatePlayerTextGold()
    {
        playfabManager.GetLoadedPlayerDatas();
    }

    private void GoldBalanceChanged(PlayerSavedData playerSavedData)
    {
        playerGoldText.text = "Gold : " + playerSavedData.Gold.ToString();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        playfabManager.OnSavedDataChanged -= GoldBalanceChanged;
    }
}
