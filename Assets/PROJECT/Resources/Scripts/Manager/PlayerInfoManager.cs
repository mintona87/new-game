using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Net.NetworkInformation;

public class PlayerInfoManager : MonoBehaviour
{
    public TextMeshProUGUI LatencyText;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            LatencyText.text = PhotonNetwork.GetPing().ToString() + " ms";
        }
    }


}
