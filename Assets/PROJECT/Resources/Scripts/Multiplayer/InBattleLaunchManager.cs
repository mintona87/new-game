using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InBattleLaunchManager : MonoBehaviourPunCallbacks
{

    GameController gameController;


    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (gameController.gameOverManager.isGameOver == false)
        {
            FindObjectOfType<PlayerLeftErrorPanelHandler>().ShowErrorPanel();
        }
    }
}
