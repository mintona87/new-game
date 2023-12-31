using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI gameOverText;

    public AudioSource backgroundMusic;

    public int player1Honor = 0;
    public int player2Honor = 0;
    [SerializeField] private TextMeshProUGUI honorText;
    private float[] playerHonor = new float[2];

    public int Turn = 0;



    public AudioManager audioManager;
    public List<PlayerManager> playerList = new List<PlayerManager>();

    public GameObject playerPrefab;
    public GameObject PlayerListContainer;
    public bool didGameFinishLoad;

    public PhotonView pv;

    public GameObject MyPlayerObj;
    public PlayerManager MyPlayerManager;

    public GameOverManager gameOverManager;

    public Shaker cameraShaker;

    public GameObject InventoryPanelObj;
    public GameObject InventoryContentObj;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        gameOverManager = FindObjectOfType<GameOverManager>();
        cameraShaker = FindObjectOfType<Shaker>();
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        StartCoroutine(SpawnPlayerPrefab());
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
                GameObject playerObj = PhotonNetwork.Instantiate("Prefabs/" + playerPrefab.name, Vector3.zero, Quaternion.identity);
                PlayerManager playerManager = playerObj.GetComponent<PlayerManager>();
                playerManager.playerNumber = player.GetPlayerNumber();
                playerManager.localPlayerNickname = player.CustomProperties["Nickname"].ToString();
                playerList.Add(playerManager);
                pv.RPC("SynchronizePlayerNumber", RpcTarget.OthersBuffered, player.GetPlayerNumber(), playerObj.GetPhotonView().ViewID);
            }
        }
    }

    [PunRPC]
    void SynchronizePlayerNumber(int playerNumber, int viewID)
    {
        GameObject playerObj = PhotonView.Find(viewID).gameObject;
        PlayerManager playerManager = playerObj.GetComponent<PlayerManager>();
        playerManager.playerNumber = playerNumber;
        playerList.Add(playerManager);
    }


    public void SetPlayerList()
    {
        for (int i = 0; i < PlayerListContainer.transform.childCount; i++)
        {
            playerList.Add(PlayerListContainer.transform.GetChild(i).GetComponent<PlayerManager>());
        }
    }

    public void HandlePlayerTurn()
    {
        
        SwitchPlayerTurn();
    }

    public void SwitchPlayerTurn()
    {
        pv.RPC("IncreasePlayerTurn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void IncreasePlayerTurn()
    {
        Turn++;

        Debug.Log("playerturn " + Turn + "calc " + Turn % 2 + " local player  " + PhotonNetwork.LocalPlayer.GetPlayerNumber());
       
        if (Turn % 2 == 0)
        {
            playerList[1].canPlay = true;
            playerList[0].canPlay = false;

            // If player 1 is an AI, take its turn here
            if (playerList[0].isAI)
            {
                playerList[0].GetComponent<AICombat>().TakeTurn();
            }
        }
        else
        {
            playerList[0].canPlay = true;
            playerList[1].canPlay = false;

            // If player 2 is an AI, take its turn here
            if (playerList[1].isAI)
            {
                playerList[1].GetComponent<AICombat>().TakeTurn();
            }
        }

        foreach (PlayerManager playerManager in playerList)
        {
            StartCoroutine(playerManager.OnSwitchTurnSettings());
        }
    }

    private void ShowGameOverMessage(int winnerPlayerNumber)
    {
        gameOverText.text = $"Player {winnerPlayerNumber} Wins!";
        gameOverText.gameObject.SetActive(true);
    }

    private void UpdateHonorAfterBattle(int winner, int loser)
    {
        playerHonor[winner - 1] += 1;
        playerHonor[loser - 1] -= 0.25f;
    }

    public void QuitButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("onleftroom");
        PhotonNetwork.LoadLevel("MainMenu");
        PhotonNetwork.LocalPlayer.CustomProperties["DidFinishChoosingAction"] = false;
        PhotonNetwork.LocalPlayer.CustomProperties["isPlayerStun"] = "notStun";
        PhotonNetwork.LocalPlayer.CustomProperties["isPlayingAction"] = false;
    }
}



