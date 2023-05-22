using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Linq;
using System;

public class GameOverManager : MonoBehaviourPunCallbacks
{
    public GameObject returnToHomeButtonObj;
    public GameObject PlayerGameOverObjContainer;
    public GameObject PlayerGameOverObjPrefab;
    public GameObject GameOverPanelObj;

    public bool isGameOver;

    PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void DisplayPlayersGameOverObj()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            pv.RPC("DisplayPlayersGameOverObjRPC", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DisplayPlayersGameOverObjRPC()
    {
        GameOverPanelObj.SetActive(true);
        // Create a sorted list of players by player number
        List<Player> sortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy(p => p.GetPlayerNumber()).ToList();

        foreach (Player getPlayer in sortedPlayers)
        {
            Debug.Log("Called1" + getPlayer.CustomProperties["Nickname"].ToString());

            GameObject playerRoomObj = Instantiate(PlayerGameOverObjPrefab, Vector3.zero, Quaternion.identity);

            playerRoomObj.transform.SetParent(PlayerGameOverObjContainer.transform);
            playerRoomObj.transform.localScale = Vector3.one;
            int playerHonor = Convert.ToInt32(getPlayer.CustomProperties["Honor"]);
            string winOrLost = getPlayer.CustomProperties["WonLost"].ToString();
            playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
            (
                getPlayer.GetPlayerNumber() + 1,
                getPlayer.CustomProperties["Nickname"].ToString(),
                playerHonor,
                winOrLost,
                "gameover"
            );
        }
    }

    public void ReturnToHomeButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("onleftroom");
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
