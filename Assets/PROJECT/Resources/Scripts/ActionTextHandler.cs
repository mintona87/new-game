using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class ActionTextHandler : MonoBehaviour
{
    public PlayerManager playerManager;


    public IEnumerator SetActionPosition()
    {
        while(playerManager == null)
        {
            Debug.Log("action text problem");
            yield return null;
        }

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
        }
        gameObject.SetActive(false);
    }


}
