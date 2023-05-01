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


    public void SetUpPlayerInfo(int playerNumber, string nickName, int playerHonor, string type)
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

            StartCoroutine(WaitLostPropertyTobeSet(nickName, playerHonor));
        }
        //to do set the image
    }

    IEnumerator WaitLostPropertyTobeSet(string nickName, int playerHonor)
    {
        while (PhotonNetwork.LocalPlayer.CustomProperties["WonLost"].ToString() == "null")
        {
            yield return null;
        }

        if (nickName == PhotonNetwork.LocalPlayer.CustomProperties["Nickname"].ToString())
        {
            WonLostText.text = PhotonNetwork.LocalPlayer.CustomProperties["WonLost"].ToString();
                
            PlayerHonorText.text = playerHonor.ToString();
        }
        else
        {
            // Find the remote player with the given nickname
            Photon.Realtime.Player remotePlayer = null;
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.CustomProperties["Nickname"].ToString() == nickName && player != PhotonNetwork.LocalPlayer)
                {
                    remotePlayer = player;
                    break;
                }
            }

            if (remotePlayer != null)
            {
                while (remotePlayer.CustomProperties["WonLost"].ToString() == "null")
                {
                    yield return null;
                }
                WonLostText.text = remotePlayer.CustomProperties["WonLost"].ToString();
                    PlayerHonorText.text = playerHonor.ToString();
            }
            else
            {
                Debug.LogError("Remote player not found: " + nickName);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("callback called" );
        // Check if the "Honor" property has been updated
        if (changedProps.ContainsKey("Honor"))
        {
            // Get the updated honor value
            int updatedHonor = Convert.ToInt32(changedProps["Honor"]);

            // Do something with the updated honor value, e.g., update the UI
            UpdateHonorUI(updatedHonor);
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
