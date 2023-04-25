using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Linq;

public class GameOverManager : MonoBehaviourPunCallbacks
{
    public GameObject returnToHomeButtonObj;
    public GameObject PlayerGameOverObjContainer;
    public GameObject PlayerGameOverObjPrefab;
    public GameObject GameOverPanelObj;

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

            playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
            (
                getPlayer.GetPlayerNumber() + 1,
                getPlayer.CustomProperties["Nickname"].ToString(),
                getPlayer.CustomProperties["Honor"].ToString(),
                "gameover"
            );
        }
    }

    public void ReturnToHomeButtonClicked()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }
}
