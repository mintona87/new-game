using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    public GameObject targetObj;
    public PlayerManager targetScript;

    PlayerManager player;
    PlayerEffect playerEffect;


    void Start()
    {
        player = GetComponent<PlayerManager>();
        playerEffect = player.playerEffect;
        
    }

    public void SetDefaultTarget()
    {
        if (gameObject.name == "Player1")
        {
            targetObj = GameObject.Find("Player2");
        }
        else
        {
            targetObj = GameObject.Find("Player1");
        }
        targetScript = targetObj.GetComponent<PlayerManager>();
    }

    void Update()
    {
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

        player.gameController.Turn++;
        player.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();
        player.gameController.SwitchPlayerTurn();
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
        player.gameController.Turn++;
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayHealEffect());

        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();
        player.gameController.SwitchPlayerTurn();

        StartCoroutine(ShowActionText($"Player 1 healed for {healAmount}!", player.playerUI.ActionText));


    }

    public void OnPlayerDefendButtonClicked()
    {
        if (player.HasLost()) return;
        player.isDefending = true;
        player.gameController.Turn++;
        targetScript.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayDefendEffect());

        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();
        player.gameController.SwitchPlayerTurn();
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

        player.gameController.Turn++;
        player.TurnsSinceCharge = 0; // Reset the counter

        StartCoroutine(playerEffect.PlaySwordSlashEffect());

        player.playerUI.UpdateHealthUI();
        player.playerUI.UpdateChargeButtons();
        player.gameController.SwitchPlayerTurn();

        StartCoroutine(ShowActionText($"Player 1 dealt {damage} damage!", player.playerUI.ActionText));
    }

  

}
