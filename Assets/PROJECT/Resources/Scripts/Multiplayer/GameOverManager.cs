using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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

    void Update()
    {
        
    }

    IEnumerator SpawnPlayerPrefab()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var playerEntry in PhotonNetwork.CurrentRoom.Players)
            {
                Photon.Realtime.Player player = playerEntry.Value;

                while (player.GetPlayerNumber() == -1)
                {
                    yield return null;
                }

                Debug.Log("SpawnPrefabCalled");
                GameObject PlayerGameOverObj = PhotonNetwork.Instantiate("Prefabs/" + PlayerGameOverObjPrefab.name, Vector3.zero, Quaternion.identity);
                pv.RPC("SynchronizePlayerNumber", RpcTarget.OthersBuffered, player.GetPlayerNumber(), PlayerGameOverObj.GetPhotonView().ViewID);
            }
        }
    }

    

    void DisplayPlayersGameOverObj()
    {

    }

    public void ReturnToHomeButtonClicked()
    {

    }
}
