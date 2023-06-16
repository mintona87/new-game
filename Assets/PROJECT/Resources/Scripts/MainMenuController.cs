using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Photon.Pun;

public class MainMenuController : MonoBehaviour
{
    public GameObject walletConnectContent;
    public GameObject selectCharacterContent;
    public Image selectedNFTImg;
    PlayfabManager playfabManager;
    
    private void Start()
    {
        walletConnectContent.SetActive(false);
        selectCharacterContent.SetActive(false);
        GlobalData.instance.nftSelectAction = OnNFTSelected;
        playfabManager = FindObjectOfType<PlayfabManager>();

        if (playfabManager.getNFTSprite != null)
        {
            Debug.Log("get nft sprite is not null");
            FindObjectOfType<LaunchManager>().ModifyPlayerCustomImageURL(playfabManager.SelectedNftImageURL);
            GlobalData.instance.SaveSelectedNFTData(playfabManager.getSelectedNFTData);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BattleScreen");

    }
    public void OnClickSelectCharacter()
    {
        if(GlobalData.instance.isWalletConnected)
        {
            selectCharacterContent.SetActive(true);
        }
        else
        {
            walletConnectContent.SetActive(true);
        }
    }
    void OnNFTSelected(NFTMEtadata _selectedNFTData)
    {
        if(_selectedNFTData.rawImage)
        {

            OnNFTImgDownloaded(GlobalData.instance.GetTextureFromBase64(_selectedNFTData.imageUrl));
        }
        else
        {
            DownloadManager.instance.BookDownload(_selectedNFTData.imageUrl, OnNFTImgDownloaded);
        }
    }
    void OnNFTImgDownloaded(Texture2D nftTexture)
    {
        selectedNFTImg.sprite = GlobalData.instance.LoadSpriteFromTexture(nftTexture);
        playfabManager.getNFTSprite = selectedNFTImg.sprite;


    }
    public void OnClickChatButton()
    {
        WndManager.instance.OpenDialog(DLGType.CHAT_DLG);
    }
}
