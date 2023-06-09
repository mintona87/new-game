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
        StartCoroutine(WaitForActorNum());
    }
    // for testing in pc
    IEnumerator WaitForActorNum()
    {
        while (PhotonNetwork.LocalPlayer.ActorNumber == -1)
        {
            yield return null;
        }
        Debug.Log("actornum " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            playfabManager.getTESTNFTSprite = Resources.Load<Sprite>("Sprites/$decimalist");
        }
        else
        {
            playfabManager.getTESTNFTSprite = Resources.Load<Sprite>("Sprites/CardanoCroc1");
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
}
