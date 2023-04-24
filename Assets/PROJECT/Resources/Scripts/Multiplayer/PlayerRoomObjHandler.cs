using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerRoomObjHandler : MonoBehaviour
{


    public Image PlayerImage;
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI PlayerHonorText;
    public TextMeshProUGUI WonLostText;

    bool isItGameOverObj;

    void Start()
    {
        if (gameObject.name.Contains("GameOver"))
        {
            WonLostText = transform.Find("WonLostText").GetComponent<TextMeshProUGUI>();
            isItGameOverObj = true;
        }

    }

    public void SetUpPlayerInfo(int playerNumber, string nickName, string playerHonor)
    {
        PlayerNumberText.text = "Player " + playerNumber.ToString();
        PlayerNameText.text = nickName;
        PlayerHonorText.text = playerHonor;
        if (isItGameOverObj)
        {
        }
        //to do set the image

    }
}
