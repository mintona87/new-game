using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Drawing;
using Photon.Realtime;
using System;
using System.Runtime.InteropServices;

public class PlayerCombat : MonoBehaviourPunCallbacks
{
    public GameObject targetObj;
    public PlayerManager targetScript;

    PlayerManager playerManager;
    PlayerEffect playerEffect;

    public bool isPlayingAction;
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerEffect = playerManager.playerEffect;
    }

    public void SetDefaultTarget()
    {
        // To do : does not make sens 
        if(PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            playerManager.gameController.MyPlayerObj = playerManager.gameController.playerList[0].gameObject;
            targetObj = playerManager.gameController.playerList[1].gameObject;

            playerManager.gameController.MyPlayerManager = playerManager.gameController.MyPlayerObj.GetComponent<PlayerManager>();
        }
        else
        {
            playerManager.gameController.MyPlayerObj = playerManager.gameController.playerList[1].gameObject;
            targetObj = playerManager.gameController.playerList[0].gameObject;

            playerManager.gameController.MyPlayerManager = playerManager.gameController.MyPlayerObj.GetComponent<PlayerManager>();
        }

        Debug.Log("playernumber " + PhotonNetwork.LocalPlayer.GetPlayerNumber());

        targetScript = targetObj.GetComponent<PlayerManager>();
    }

    public enum Actions 
    {
        NoAction = 0,
        Attack = 1,
        Defend = 2,
        Charge = 3,
        Heal = 4,
    }

    public Actions choosenAction;
    
    void UpdateTurnAndUI()
    {
        playerManager.playerUI.UpdateChargeButtons();

        Debug.Log("istargetstuned " + playerManager.playerCombat.targetScript.isStunned);
        if (playerManager.playerCombat.targetScript.isStunned)
        {
            playerManager.playerCombat.targetScript.gameController.HandlePlayerTurn();
        }
        else
        {
            playerManager.gameController.HandlePlayerTurn();
        }
    }


    //gets a player different than local player
    public Player GetOtherPlayer()
    {
        Player otherPlayer = null;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                otherPlayer = player;
                break;
            }
        }
        return otherPlayer;
    }


    private void Update()
    {

        

        Debug.Log("stuncustomprop1" + PhotonNetwork.LocalPlayer.CustomProperties["isPlayerStun"]+" num "+ PhotonNetwork.LocalPlayer.GetPlayerNumber() +"manastun "+playerManager.isStunned);
        if (isPlayingAction)
        {
            DisableButtonUI();
        }
    }

    //Set a random number only on the master, so both players share the same number
    private void SetSharedRandomNumber()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int rnd = UnityEngine.Random.Range(1, 100);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SharedRandomNumber", rnd } });
        }
    }


    // called when both players finish to selecte an action. This part handle the order in which players are playing the actions.
    IEnumerator CheckIfShouldPlayAction()
    {
        const float waitTime = 3.0f;
        isPlayingAction = true;
        targetScript.playerCombat.isPlayingAction = true;
        
        // wait if no actions are selected (Unless there is a buf it should never be called)
        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            while (choosenAction == Actions.NoAction)
            {
                yield return null;
            }
        }

        //Reset DidFinishChoosingAction for all players
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.CustomProperties["DidFinishChoosingAction"] =  false;
        }

        int sharedRandomNumber = (int)PhotonNetwork.MasterClient.CustomProperties["SharedRandomNumber"];
        Debug.Log("SharedRandomNumber: " + sharedRandomNumber);

        // set the variable that indicates that the player is not choosing action anymore but he is playing the action that he choose
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayingAction", true } });

       
        
        int localSpd = Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["SPD"]);
        int otherSpd = Convert.ToInt32(GetOtherPlayer().CustomProperties["SPD"]);

        Debug.Log("localspd " + localSpd + "other " + otherSpd);

        if (localSpd > otherSpd)
        {
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                CheckStunAndChooseAction();
            }
        }
        else if(localSpd == otherSpd)
        {
            //choose who plays first randomely
            if (sharedRandomNumber > 50)
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
                {
                    CheckStunAndChooseAction();
                }
                else
                {
                    yield return new WaitForSeconds(waitTime);
                    CheckStunAndChooseAction();
                }
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 1)
                {
                    CheckStunAndChooseAction();
                }
                else
                {
                    yield return new WaitForSeconds(waitTime);
                    CheckStunAndChooseAction();
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                CheckStunAndChooseAction();
            }
        }

        // reset variables for the next turn
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayingAction", false } });
        isPlayingAction = false;
        targetScript.playerCombat.isPlayingAction = false;
        PhotonNetwork.MasterClient.CustomProperties["SharedRandomNumber"] = 0;
        
        SetButtonUIAccordingToCanPlay();

        //player.OnSwitchTurnSettings();
        choosenAction = Actions.NoAction;
        yield return null;
    }

    void CheckStunAndChooseAction()
    {
        if (!playerManager.isStunned)
        {
            ChooseAction();
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayerStun", "notStun" } });
        }
    }

    public void DisableButtonUI()
    {
        foreach (PlayerManager playerManager in FindObjectsOfType<PlayerManager>())
        {
            playerManager.playerUI.playerAttackButton.interactable = false;
            playerManager.playerUI.playerHealButton.interactable = false;
            playerManager.playerUI.playerDefendButton.interactable = false;
            playerManager.playerUI.playerChargeButton.interactable = false;
        }
    }
    public void SetButtonUIAccordingToCanPlay()
    {
        foreach (PlayerManager playerManager in FindObjectsOfType<PlayerManager>())
        {
            playerManager.playerUI.playerAttackButton.interactable = playerManager.canPlay;
            playerManager.playerUI.playerHealButton.interactable = playerManager.canPlay;
            playerManager.playerUI.playerDefendButton.interactable = playerManager.canPlay;
            playerManager.playerUI.playerChargeButton.interactable = playerManager.canPlay && playerManager.TurnsSinceCharge >= 6;
        }
    }
    void ChooseAction()
    {
        switch (choosenAction) 
        {
            case Actions.Attack:
                Attack();
                break;
            case Actions.Defend:
                Defend();
                break;
            case Actions.Charge:
                Charge();
                break;
            case Actions.Heal:
                Heal();
                break;
            case Actions.NoAction:
                Debug.LogError("No action was selected");
                break;
        }
    }


    #region custom prop updated callback

    // called after "PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { .... "is called
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Callback called");
        
        // Check if the "DidFinishChoosingAction" property has been updated
        if (changedProps.ContainsKey("DidFinishChoosingAction"))
        {
            int countTrue = 0;
            // Iterate through all players in the room
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // Check if the player has the custom property "DidFinishChoosingAction"
                if (player.CustomProperties.ContainsKey("DidFinishChoosingAction"))
                {
                    // Get the updated value
                    bool didFinishChoosingAction = (bool)player.CustomProperties["DidFinishChoosingAction"];

                    // Perform any required action based on the value
                    Debug.Log($"callbackdisfinish {player.NickName} has DidFinishChoosingAction set to: {didFinishChoosingAction}" + " trucount "+ countTrue);
                    if (didFinishChoosingAction)
                    {
                        countTrue++;
                        Debug.Log("countrue " + countTrue);
                    }
                }
            }
            if (countTrue == 2)
            {
                SetSharedRandomNumber();
                StartCoroutine(WaitForSharedRandomNumberAndStartCheck());
            }
            if (countTrue == 1)
            {
                UpdateTurnAndUI();
            }
        }
        else if (changedProps.ContainsKey("isPlayingAction"))
        {
            int countPlayerPlayingAction = 0;

            // Iterate through all players in the room
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // Check if the player has the custom property "DidFinishChoosingAction"
                if (player.CustomProperties.ContainsKey("isPlayingAction"))
                {
                    // Get the updated value
                    bool isPlayingAction = (bool)player.CustomProperties["isPlayingAction"];

                    // Perform any required action based on the value
                    Debug.Log($"callbackPlayingaction {player.NickName} has DidFinishChoosingAction set to: {isPlayingAction}" + " actioncount " + countPlayerPlayingAction);
                    if (!isPlayingAction)
                    {
                        countPlayerPlayingAction++;
                        Debug.Log("countPlayerPlayingAction " + countPlayerPlayingAction);
                    }
                }
            }

            if (countPlayerPlayingAction == 2)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    // do something when both player finish to play action
                }
            }

        }
        else if (changedProps.ContainsKey("isPlayerStun"))
        {
            // Iterate through all players in the room
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // Check if the player has the custom property "isPlayerStun"
                if (player.CustomProperties.ContainsKey("isPlayerStun"))
                {
                    bool isPlayerStun;

                    // Get the updated value
                    if (player.CustomProperties["isPlayerStun"].ToString() == "stun") 
                    {
                        isPlayerStun = true;
                    }
                    else
                    {
                        isPlayerStun = false;
                    }
                 
                    Debug.Log("stunplayerid " + player.GetPlayerNumber() + " stun " + isPlayerStun);
                    if (player.GetPlayerNumber() == PhotonNetwork.LocalPlayer.GetPlayerNumber())
                    {
                        playerManager.isStunned = isPlayerStun;
                    }
                }
            }
        }
    }

    IEnumerator WaitForSharedRandomNumberAndStartCheck()
    {
        Debug.Log("Waiting for SharedRandomNumber");
        while (Convert.ToInt32(PhotonNetwork.MasterClient.CustomProperties["SharedRandomNumber"]) == 0)
        {
            yield return null;
        }

        Debug.Log("ShouldStartCheck");
        StartCoroutine(CheckIfShouldPlayAction());
    }
    #endregion

    #region Attack
    public void OnPlayerAttackButtonClicked()
    {
        if (playerManager.HasLost()) return;
        choosenAction = Actions.Attack;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
    }

    void Attack()
    {
        if (playerManager.HasLost()) return;

        int damage = playerManager.Attack();

        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();
        // to do set target
        if (!targetScript.Dodge()) // Check if Player 2 dodged the attack
        {
            targetScript.ChangeHP(-damage);
            playerManager.pv.RPC("ShowActionRPC", RpcTarget.AllBuffered, $"Player 1 dealt {damage} damage!");
        }
        else
        {
            playerManager.pv.RPC("ShowActionRPC", RpcTarget.AllBuffered, "Player 1 missed the attack!");
        }

        playerManager.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        //UpdateTurnAndUI();
    }
    #endregion

  
    #region Heal
    public void OnPlayerHealButtonClicked()
    {
        if (playerManager.HasLost()) return;
        choosenAction = Actions.Heal;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();

    }

    void Heal()
    {
        int healAmount = playerManager.Heal();
        playerManager.ChangeHP(healAmount);
        if (playerManager.HP > playerManager.MaxHP)
        {
            playerManager.HP = playerManager.MaxHP;
        }
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayHealEffect());

        //UpdateTurnAndUI();

        StartCoroutine(ShowActionText($"Player 1 healed for {healAmount}!", playerManager.playerUI.ActionText));
    }

    #endregion

    #region Defend
    public void OnPlayerDefendButtonClicked()
    {
        if (playerManager.HasLost()) return;
        choosenAction = Actions.Defend;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
    }

    void Defend()
    {
        playerManager.SetIsDefending(true);
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayDefendEffect());

        //UpdateTurnAndUI();
    }
    #endregion

    #region Charge
    public void OnPlayerChargeButtonClicked()
    {
        if (playerManager.HasLost()) return;
        choosenAction = Actions.Charge;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
        //player.playerUI.UpdateHealthUI();
        //player.playerUI.UpdateChargeButtons();
    }

    void Charge()
    {
        if (playerManager.HasCharged)
        {
            // Show error message that Charge is on cooldown
            return;
        }

        int damage = playerManager.Charge(targetScript);
        targetScript.ChangeHP(-damage);

        playerManager.TurnsSinceCharge = 0; // Reset the counter

        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        //player.playerUI.UpdateHealthUI();
        playerManager.playerUI.UpdateChargeButtons();

        StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", playerManager.playerUI.ActionText));
    }
    #endregion

    [PunRPC]
    void ShowActionRPC(string text)
    {
        StartCoroutine(ShowActionText(text, playerManager.playerUI.ActionText));
    }


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
}
