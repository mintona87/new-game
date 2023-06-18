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
    DefaultImageManager defaultImageManager;

    private void Start()
    {
        walletConnectContent.SetActive(false);
        selectCharacterContent.SetActive(false);
        GlobalData.instance.nftSelectAction = OnNFTSelected;
        playfabManager = FindObjectOfType<PlayfabManager>();
        defaultImageManager = FindObjectOfType<DefaultImageManager>();
        SetPlayerSprite();
    }

    public void SetPlayerSprite()
    {
        if (playfabManager.SelectedNftImageURL != "")
        {
            Debug.Log("get nft sprite is not null" + playfabManager.SelectedNftImageURL + " data " + playfabManager.getSelectedNFTData.name);

            LaunchManager.Instance.ModifyPlayerCustomImageURL(playfabManager.SelectedNftImageURL);
            GlobalData.instance.SaveSelectedNFTData(playfabManager.getSelectedNFTData);
        }
        else
        {
            Debug.Log("get default sprite");
            SetDefaultSprite();
        }
    }


    void SetDefaultSprite()
    {
        selectedNFTImg.sprite = defaultImageManager.defaultSpriteList[0];
        playfabManager.SelectedDefaultSprite = selectedNFTImg.sprite;
        LaunchManager.Instance.ModifyPlayerCustomImageURL("");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DefaultSpriteName", playfabManager.SelectedDefaultSprite.name } });
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
