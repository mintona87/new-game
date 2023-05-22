using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class PlayerLeftErrorPanelHandler : MonoBehaviour
{

    public GameObject PlayerLeftErrorContainerObj;


    public void ShowErrorPanel()
    {
        if (PlayerLeftErrorContainerObj != null)
        {
            PlayerLeftErrorContainerObj.SetActive(true);
        }
        else
        {
            Debug.LogError("No reference to an PlayerLEftErrorPanelCanvas GameObject");
        }
    }

    public void OkButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        PlayerLeftErrorContainerObj.SetActive(false);
    }

}
