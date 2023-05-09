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

    PlayerManager player;
    PlayerEffect playerEffect;

    public bool isPlayingAction;
    
    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        playerEffect = player.playerEffect;
    }

    public void SetDefaultTarget()
    {
        // To do : does not make sens 
        if(PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            player.gameController.MyPlayerObj = player.gameController.playerList[0].gameObject;
            targetObj = player.gameController.playerList[1].gameObject;

            player.gameController.MyPlayerManager = player.gameController.MyPlayerObj.GetComponent<PlayerManager>();
        }
        else
        {
            player.gameController.MyPlayerObj = player.gameController.playerList[1].gameObject;
            targetObj = player.gameController.playerList[0].gameObject;

            player.gameController.MyPlayerManager = player.gameController.MyPlayerObj.GetComponent<PlayerManager>();
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
        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();

        Debug.Log("istargetstuned " + player.playerCombat.targetScript.isStunned);
        if (player.playerCombat.targetScript.isStunned)
        {
            player.playerCombat.targetScript.gameController.HandlePlayerTurn(true);
        }
        else
        {
            player.gameController.HandlePlayerTurn(false);
        }
    }

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
        Debug.Log("isplayingaction " + isPlayingAction + " choosenAction "+ choosenAction);
        if (isPlayingAction)
        {
            DisableButtonUI();
        }
    }

    private void SetSharedRandomNumber()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int rnd = UnityEngine.Random.Range(1, 100);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SharedRandomNumber", rnd } });
        }
    }

    IEnumerator CheckIfShouldPlayAction()
    {

        isPlayingAction = true;
        targetScript.playerCombat.isPlayingAction = true;

        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            while (choosenAction == Actions.NoAction)
            {
                yield return null;
            }
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.CustomProperties["DidFinishChoosingAction"] =  false;
        }

        int sharedRandomNumber = (int)PhotonNetwork.MasterClient.CustomProperties["SharedRandomNumber"];
        Debug.Log("SharedRandomNumber: " + sharedRandomNumber);

        //tmp
        if (player.isStunned)
        {
            Debug.Log("StunedBfrAction");
        }


        Debug.Log("localspd " + Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["SPD"]) + "other "+ Convert.ToInt32(GetOtherPlayer().CustomProperties["SPD"]));
        if (Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["SPD"]) > Convert.ToInt32(GetOtherPlayer().CustomProperties["SPD"]))
        {
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                if (!player.isStunned)
                {
                    ChooseAction();
                }
            }
        }
        else if(Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["SPD"]) == Convert.ToInt32(GetOtherPlayer().CustomProperties["SPD"]))
        {

            if (sharedRandomNumber > 50)
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
                {
                    if (!player.isStunned)
                    {
                        ChooseAction();
                    }
                }
                else
                {
                    yield return new WaitForSeconds(3.0f);
                    if (!player.isStunned)
                    {
                        ChooseAction();
                    }
                }
            }
            else
            {
                if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 1)
                {
                    if (!player.isStunned)
                    {
                        ChooseAction();
                    }
                }
                else
                {
                    yield return new WaitForSeconds(3.0f);
                    if (!player.isStunned)
                    {
                        ChooseAction();
                    }
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                if (!player.isStunned)
                {
                    ChooseAction();
                }
            }
        }
        Debug.Log("CameAtTheEnd");
        isPlayingAction = false;
        targetScript.playerCombat.isPlayingAction = false;
        PhotonNetwork.MasterClient.CustomProperties["SharedRandomNumber"] = 0;
        SetButtonUIAccordingToCanPlay();

        //player.OnSwitchTurnSettings();
        choosenAction = Actions.NoAction;
        
        yield return null;
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
            playerManager.playerUI.playerChargeButton.interactable = playerManager.canPlay;
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


    #region Attack
    public void OnPlayerAttackButtonClicked()
    {
        if (player.HasLost()) return;
        choosenAction = Actions.Attack;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
    }

    void Attack()
    {
        if (player.HasLost()) return;

        int damage = player.Attack();

        PlayerManager targetScript = targetObj.GetComponent<PlayerManager>();
        // to do set target
        if (!targetScript.Dodge()) // Check if Player 2 dodged the attack
        {
            targetScript.ChangeHP(-damage);
            StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player.playerUI.ActionText));
        }
        else
        {
            StartCoroutine(ShowActionText("Player 1 missed the attack!", player.playerUI.ActionText));
        }

        player.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        //UpdateTurnAndUI();
    }
    #endregion

  
    #region Heal
    public void OnPlayerHealButtonClicked()
    {
        if (player.HasLost()) return;
        choosenAction = Actions.Heal;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();

    }

    void Heal()
    {
        int healAmount = player.Heal();
        player.ChangeHP(healAmount);
        if (player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayHealEffect());

        //UpdateTurnAndUI();

        StartCoroutine(ShowActionText($"Player 1 healed for {healAmount}!", player.playerUI.ActionText));
    }

    #endregion

    #region Defend
    public void OnPlayerDefendButtonClicked()
    {
        if (player.HasLost()) return;
        choosenAction = Actions.Defend;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
    }

    void Defend()
    {
        player.SetIsDefending(true);
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayDefendEffect());

        //UpdateTurnAndUI();
    }
    #endregion

    #region Charge
    public void OnPlayerChargeButtonClicked()
    {
        if (player.HasLost()) return;
        choosenAction = Actions.Charge;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DidFinishChoosingAction", true } });
        UpdateTurnAndUI();
        //player.playerUI.UpdateHealthUI();
        //player.playerUI.UpdateChargeButtons();
    }

    void Charge()
    {
        if (player.HasCharged)
        {
            // Show error message that Charge is on cooldown
            return;
        }

        int damage = player.Charge(targetScript);
        targetScript.ChangeHP(-damage);

        player.TurnsSinceCharge = 0; // Reset the counter

        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        //player.playerUI.UpdateHealthUI();
        //player.playerUI.UpdateChargeButtons();

        StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player.playerUI.ActionText));
    }
    #endregion

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
