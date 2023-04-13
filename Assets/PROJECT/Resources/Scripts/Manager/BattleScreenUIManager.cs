using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleScreenUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI player1ActionText;
    [SerializeField] private TextMeshProUGUI player2ActionText;

    [SerializeField] private TextMeshProUGUI honorText;


    public Button player1AttackButton;
    public Button player1HealButton;
    public Button player1DefendButton;
    public Button player1ChargeButton;

    public Button player2AttackButton;
    public Button player2HealButton;
    public Button player2DefendButton;
    public Button player2ChargeButton;
}
