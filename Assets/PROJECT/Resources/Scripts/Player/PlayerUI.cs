using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.EconomyModels;
using Image = UnityEngine.UI.Image;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI ActionText;
    //public TextMeshProUGUI playerNumberText;
    public TextMeshProUGUI playerUsernameText;
    public TextMeshProUGUI HPText;

    public Button playerAttackButton;
    public Button playerHealButton;
    public Button playerDefendButton;
    public Button playerChargeButton;

    public Slider hpSlider;
    PlayerManager player;

    public Image PlayerPicture;

    public GameObject itemSlotPrefab;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void SetMaxHealthSlider()
    {
        float sliderValue = GetNormalizedHP(player.HP, player.MaxHP);
        hpSlider.maxValue = sliderValue;
        hpSlider.value = sliderValue;
        HPText.text = player.HP.ToString() + "/" + player.MaxHP.ToString();
    }

    public void SetHealthSlider(int hp)
    {
        float sliderValue = GetNormalizedHP(hp, player.MaxHP);
        player.pv.RPC("SetHealthSliderRPC", RpcTarget.AllBuffered, hp, sliderValue);
    }

    [PunRPC]
    public void SetHealthSliderRPC(int hp, float sliderValue)
    {
        hpSlider.value = sliderValue;
        HPText.text = hp.ToString() + "/" + player.MaxHP.ToString();
    }
    public void SetActiveButtons(bool condition)
    {
        playerAttackButton.gameObject.SetActive(condition);
        playerHealButton.gameObject.SetActive(condition);
        playerDefendButton.gameObject.SetActive(condition);
        playerChargeButton.gameObject.SetActive(condition);
    }

    public void SetActiveTargetButtons(bool condition)
    {
        player.playerCombat.targetScript.playerUI.playerAttackButton.gameObject.SetActive(condition);
        player.playerCombat.targetScript.playerUI.playerHealButton.gameObject.SetActive(condition);
        player.playerCombat.targetScript.playerUI.playerDefendButton.gameObject.SetActive(condition);
        player.playerCombat.targetScript.playerUI.playerChargeButton.gameObject.SetActive(condition);
    }


    public void SetPlayerPicture(string imageURL,string defaultImageName)
    {
        if (imageURL != "")
        {
            if (player.playfabManager.getSelectedNFTData.rawImage)
            {
                OnNFTImgDownloaded(GlobalData.instance.GetTextureFromBase64(imageURL));
            }
            else
            {
                DownloadManager.instance.BookDownload(imageURL, OnNFTImgDownloaded);
            }
        }
        else
        {
            PlayerPicture.sprite = Resources.Load<Sprite>("Sprites/DefaultSprite/" + defaultImageName);
        }
    }
    void OnNFTImgDownloaded(Texture2D nftTexture)
    {
        player.playfabManager.getNFTSprite = GlobalData.instance.LoadSpriteFromTexture(nftTexture);
        PlayerPicture.sprite = player.playfabManager.getNFTSprite;
    }

    public float GetNormalizedHP(int hp, int maxHealth)
    {
        return (float)hp / (float)maxHealth;
    }

    public void OpenIventoryUiButtonPressed()
    {
        UpdateInventoryUI();
    }

   
    void UpdateInventoryUI()
    {
        // Clear the inventory UI
        foreach (Transform child in player.gameController.InventoryContentObj.transform)
        {
            Destroy(child.gameObject);
        }

        // Create a new item slot for each item in the inventory
        foreach (Item item in player.inventory.itemList)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, Vector3.zero,Quaternion.identity);
            itemSlot.transform.SetParent(player.gameController.InventoryContentObj.transform);
            //itemSlot.GetComponent<Image>().sprite = item.icon;
            //itemSlot.GetComponentInChildren<Text>().text = item.quantity.ToString();
        }
    }

}
