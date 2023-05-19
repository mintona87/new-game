using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using Photon.Realtime;

public class PlayerRoomObjHandler : MonoBehaviourPunCallbacks
{
    public Image PlayerImage;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI PlayerHonorText;
    public TextMeshProUGUI WonLostText;


    public void SetUpPlayerInfo(int playerNumber, string nickName, int playerHonor, string winOrLost, string type)
    {
        if (type != "leaderboard")
        {
            PlayerNumberText.text = "Player " + playerNumber.ToString();
            PlayerImage.gameObject.SetActive(true);
        }
        else
        {
            PlayerImage.gameObject.SetActive(false);
        }

        PlayerNameText.text = nickName;

        if (type != "gameover")
        {
            PlayerHonorText.text = playerHonor.ToString();
        }

        if(type == "gameover" || type == "matchmaking")
        {
            SetPlayerPicture();
        }

        if (type == "gameover")
        {
            WonLostText = transform.Find("WonLostText").GetComponent<TextMeshProUGUI>();

            StartCoroutine(WaitLostPropertyTobeSet(nickName, playerHonor, winOrLost));
        }
        //to do set the image
    }

    IEnumerator WaitLostPropertyTobeSet(string nickName, int playerHonor, string winOrLost)
    {
        PlayerHonorText.text = playerHonor.ToString();
        WonLostText.text = winOrLost;
        yield return null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("callback called" );
        // Check if the "Honor" property has been updated
        if (changedProps.ContainsKey("Honor"))
        {

            // Get the updated honor value
            int updatedHonor = Convert.ToInt32(changedProps["Honor"]);

            //Debug.Log("honorremoteplayerlocal3 " + PlayerNameText.text + " nickname " + targetPlayer.CustomProperties["Nickname"].ToString());
            // Do something with the updated honor value, e.g., update the UI
            if (PlayerNameText.text == targetPlayer.CustomProperties["Nickname"].ToString())
            {
                UpdateHonorUI(updatedHonor);
            }
        }
    }

    private void UpdateHonorUI(int updatedHonor)
    {
        // Update your UI elements here, e.g., the honor text
        PlayerHonorText.text = updatedHonor.ToString();
    }
    public void SetPlayerPicture()
    {
        if (PlayerNumberText.text == "Player 1")
        {
            PlayerImage.sprite = Resources.Load<Sprite>("Sprites/$decimalist");
        }
        else
        {
            PlayerImage.sprite = Resources.Load<Sprite>("Sprites/CardanoCroc1");
        }
    }
}
