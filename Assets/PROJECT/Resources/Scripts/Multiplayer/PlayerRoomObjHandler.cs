using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;

public class PlayerRoomObjHandler : MonoBehaviour
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
        }

        PlayerNameText.text = nickName;
        if (type != "gameover")
        {
            PlayerHonorText.text = playerHonor.ToString();
        }

        if (type == "gameover")
        {
            WonLostText = transform.Find("WonLostText").GetComponent<TextMeshProUGUI>();

            StartCoroutine(WaitLostPropertyTobeSet(nickName, playerHonor, updatedHonor => {
                PlayerHonorText.text = updatedHonor.ToString();
            }));
        }
        //to do set the image
    }

    IEnumerator WaitLostPropertyTobeSet(string nickName, int playerHonor, Action<int> onHonorUpdated)
    {
        while (PhotonNetwork.LocalPlayer.CustomProperties["WonLost"].ToString() == "null")
        {
            yield return null;
        }

        if (nickName == PhotonNetwork.LocalPlayer.CustomProperties["Nickname"].ToString())
        {
            WonLostText.text = PhotonNetwork.LocalPlayer.CustomProperties["WonLost"].ToString();
            if (WonLostText.text == "Won")
            {
                onHonorUpdated(playerHonor + 10);
            }
            else
            {
                onHonorUpdated(playerHonor);
            }
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
                if(WonLostText.text == "Won")
                {
                    onHonorUpdated(playerHonor + 10);
                }
                else
                {
                    onHonorUpdated(playerHonor);
                }
            }
            else
            {
                Debug.LogError("Remote player not found: " + nickName);
            }
        }

        
    }
}
