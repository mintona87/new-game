using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerCombat : MonoBehaviour
{
    public GameObject targetObj;
    public PlayerManager targetScript;

    PlayerManager player;
    PlayerEffect playerEffect;

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

    public void OnPlayerAttackButtonClicked()
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

        UpdateTurnAndUI();
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

    public void OnPlayerHealButtonClicked()
    {
        if (player.HasLost()) return;

        int healAmount = player.Heal();
        player.ChangeHP(healAmount);
        if (player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayHealEffect());

        UpdateTurnAndUI();

        StartCoroutine(ShowActionText($"Player 1 healed for {healAmount}!", player.playerUI.ActionText));
    }

    public void OnPlayerDefendButtonClicked()
    {
        if (player.HasLost()) return;
        player.SetIsDefending(true);
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayDefendEffect());

        UpdateTurnAndUI();
    }
    public void OnPlayerChargeButtonClicked()
    {
        if (player.HasLost()) return;
        if (player.HasCharged)
        {
            // Show error message that Charge is on cooldown
            return;
        }

        int damage = player.Charge(targetScript);
        targetScript.ChangeHP(-damage);

        player.TurnsSinceCharge = 0; // Reset the counter

        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();

        StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player.playerUI.ActionText));
    }
}
