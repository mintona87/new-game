using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI player1ActionText;
    [SerializeField] private TextMeshProUGUI player2ActionText;
    
    public AudioSource backgroundMusic;

    public int player1Honor = 0;
    public int player2Honor = 0;
    [SerializeField] private TextMeshProUGUI honorText;
    private float[] playerHonor = new float[2];



    public int Turn = 0;

    
    private int player1TurnsSinceCharge = 6;
    private int player2TurnsSinceCharge = 6;

    private bool isGameOver = false;

    public AudioManager audioManager;
    public List<PlayerManager> playerList = new List<PlayerManager>();

    public GameObject playerPrefab;
    public GameObject PlayerListContainer;
    public bool didGameFinishLoad;

    public PhotonView pv;

    public GameObject MyPlayerObj;

    public GameOverManager gameOverManager;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        gameOverManager = FindObjectOfType<GameOverManager>();
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
        for(int i = 0; i < PlayerListContainer.transform.childCount; i++)
        {
            playerList.Add(PlayerListContainer.transform.GetChild(i).GetComponent<PlayerManager>());
        }
    }
    
    //Added this method to check if both players have selected their actions and then execute them
    public void CheckForActionExecution()
    {
    if (player1.actionSelected && player2.actionSelected)
    {
        StartCoroutine(player1.ExecuteActions(player2));
    }
    }
    // ...

// Coroutine to show action text and fade it away
    public IEnumerator ShowActionText(string text, TextMeshProUGUI textComponent)
    {
        float duration = 1.5f; // How long the text should stay visible
        float fadeDuration = 0.5f; // How long the fade in/out should take

        // Set the text and alpha value to 0
        textComponent.text = text;
        textComponent.canvasRenderer.SetAlpha(0f);

        // Fade in
        textComponent.CrossFadeAlpha(1f, fadeDuration, false);

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Fade out
        textComponent.CrossFadeAlpha(0f, fadeDuration, false);
    }





    public void SwitchPlayerTurn()
    {
        //if (Turn % 2 == 0)
        //{
        //    player1AttackButton.interactable = false;
        //    player1HealButton.interactable = false;
        //    player1DefendButton.interactable = false;
        //    player1ChargeButton.interactable = false;

        //    player2AttackButton.interactable = true;
        //    player2HealButton.interactable = true;
        //    player2DefendButton.interactable = true;
        //    player2ChargeButton.interactable = player2TurnsSinceCharge >= 6;

        //     if (player2Script.isStunned)
        //      {
        //        Turn++;
        //        // Skip opponent's turn and allow player 1 to take another action
        //        player2Script.isStunned = false;
        //        player1AttackButton.interactable = true;
        //        player1HealButton.interactable = true;
        //        player1DefendButton.interactable = true;
        //        player1ChargeButton.interactable = player1TurnsSinceCharge >= 6;

        //        player2AttackButton.interactable = false;
        //        player2HealButton.interactable = false;
        //        player2DefendButton.interactable = false;
        //        player2ChargeButton.interactable = false;

        //        StartCoroutine(ShowActionText("Player 2 is stunned!", player2ActionText));
        //        StartCoroutine(ShowActionText(" ", player1ActionText));
        //    }
        //}
        //else
        //{
        //    player1AttackButton.interactable = true;
        //    player1HealButton.interactable = true;
        //    player1DefendButton.interactable = true;
        //    player1ChargeButton.interactable = player1TurnsSinceCharge >= 6;

        //    player2AttackButton.interactable = false;
        //    player2HealButton.interactable = false;
        //    player2DefendButton.interactable = false;
        //    player2ChargeButton.interactable = false;

        //    if (player1Script.isStunned)
        //    {
        //        Turn++;
        //        // Skip opponent's turn and allow player 2 to take another action
        //        player1Script.isStunned = false;
        //        player1AttackButton.interactable = false;
        //        player1HealButton.interactable = false;
        //        player1DefendButton.interactable = false;
        //        player1ChargeButton.interactable = false;

        //        player2AttackButton.interactable = true;
        //        player2HealButton.interactable = true;
        //        player2DefendButton.interactable = true;
        //        player2ChargeButton.interactable = player2TurnsSinceCharge >= 6;

        //        StartCoroutine(ShowActionText("Player 1 is stunned!", player1ActionText));
        //        StartCoroutine(ShowActionText(" ", player2ActionText));
        //    }
        //}

        pv.RPC("IncreasePlayerTurn", RpcTarget.AllBuffered);


        Debug.Log("playerlistCount" + playerList.Count);

       
    }

    [PunRPC]
    void IncreasePlayerTurn()
    {
        Turn++;
        if (Turn % 2 == 0)
        {

            playerList[0].canPlay = true;
            playerList[1].canPlay = false;
        }
        else
        {
            playerList[1].canPlay = true;
            playerList[0].canPlay = false;
        }

        foreach (PlayerManager player in playerList)
        {
            player.OnSwitchTurnSettings();
        }
    } 



    void Update()
    {
        //if (!isGameOver)
        //{
        //    if (player1Script.HasLost())
        //    {
        //        UpdateHonorAfterBattle(winner: 2, loser: 1);
        //        ShowGameOverMessage(2);
        //        isGameOver = true;
        //    }
        //    else if (player2Script.HasLost())
        //    {
        //        UpdateHonorAfterBattle(winner: 1, loser: 2);
        //        ShowGameOverMessage(1);
        //        isGameOver = true;
        //    }
        //}
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

}



