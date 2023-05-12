using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ActionTextHandler : MonoBehaviour
{
    PlayerManager playerManager;

    private void Awake()
    {
        playerManager = transform.parent.GetComponent<PlayerManager>();
    }


    public void SetActionPosition()
    {
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            if (playerManager.isItMyPlayer)
            {
                transform.localPosition = new Vector3(450.0f, -1.5f, 0.0f);
            }
            else
            {
                transform.localPosition = new Vector3(-280.0f, -1.5f, 0.0f);

            }
            gameObject.SetActive(false);
        }
        else
        {
            if (playerManager.isItMyPlayer)
            {
                transform.localPosition = new Vector3(-280.0f, -1.5f, 0.0f);
            }
            else
            {
                transform.localPosition = new Vector3(450.0f, -1.5f, 0.0f);
            }
            gameObject.SetActive(false);
        }
    }


}
