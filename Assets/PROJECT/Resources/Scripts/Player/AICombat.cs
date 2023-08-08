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
using static UnityEngine.GraphicsBuffer;

public class AICombat : MonoBehaviour
{
    public PlayerManager playerManager;
    public PlayerEffect playerEffect;
    private int turnCounter = 0;
    private bool shouldHealNext = false;

    public void TakeTurn()
    {
        // Evaluate the situation
        EvaluateSituation();

        // Increment the turn counter
        turnCounter++;
    }

    private void EvaluateSituation()
    {
        if (shouldHealNext)
        {
            Heal();
            shouldHealNext = false;
            return;
        }

        if ((playerManager.HP <= 70 && playerManager.HP > 30) || playerManager.HP <= 30)
        {
            Defend();
            shouldHealNext = true;
            return;
        }

        if (turnCounter % 6 == 0 && playerManager.TurnsSinceCharge >= 6)
        {
            Charge();
            return;
        }

        Attack();
    }

    void Attack()
    {
        if (playerManager.HasLost()) return;
        int damage = playerManager.Attack();
        // Apply damage logic if needed
        playerManager.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlaySwordSlashEffect());
    }

    void Heal()
    {
        int healAmount = playerManager.Heal();
        playerManager.ChangeHP(healAmount);
        playerManager.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayHealEffect());
    }

    void Defend()
    {
        playerManager.SetIsDefending(PhotonNetwork.LocalPlayer, true);
        playerManager.TurnsSinceCharge++;
        StartCoroutine(playerEffect.PlayDefendEffect());
    }

    void Charge()
    {
        if (playerManager.HasCharged) return;
        int damage = playerManager.Charge();
        // Apply damage logic if needed
        playerManager.TurnsSinceCharge = 0;
        StartCoroutine(playerEffect.PlayPlayer1ChargeEffect());
    }
}
