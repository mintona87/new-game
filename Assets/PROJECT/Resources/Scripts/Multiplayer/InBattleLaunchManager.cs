using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class InBattleLaunchManager : MonoBehaviourPunCallbacks
{
    PlayerManager playerManager;
    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Remove the playerRoomObj when the player leaves the room
        int otherPlayerNumber = otherPlayer.GetPlayerNumber();
       
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("isGameOver", out object value))
        {
            if (playerManager.gameController.gameOverManager.isGameOver && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                OnConnectionErrorPanel.Instance.SetErrorPanelScreenActive(true);
            }
        }
    }
}
