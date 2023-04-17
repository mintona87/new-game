using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI HealthText;
    [SerializeField] public TextMeshProUGUI HonorText;
    [SerializeField] public TextMeshProUGUI ActionText;
    public TextMeshProUGUI playerNumberText;
    public TextMeshProUGUI playerUsernameText;

    public Button playerAttackButton;
    public Button playerHealButton;
    public Button playerDefendButton;
    public Button playerChargeButton;

    public Slider hpSlider;
    PlayerManager player;

    public Image PlayerPicture;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void SetMaxHealth()
    {
        float sliderValue = GetNormalizedHP(player.GetPlayerStats().HP, player.GetPlayerStats().MaxHP);
        hpSlider.maxValue = sliderValue;
        hpSlider.value = sliderValue;
    }

    public void SetHealth(int hp)
    {
        float sliderValue = GetNormalizedHP(hp, player.GetPlayerStats().MaxHP);
        hpSlider.value = sliderValue;
    }

    public void SetPlayerPicture()
    {
        if(gameObject.name == "Player1")
        {
            PlayerPicture.sprite =  Resources.Load<Sprite>("Sprites/$decimalist");
        }
        else
        {
            PlayerPicture.sprite = Resources.Load<Sprite>("Sprites/CardanoCroc1");
        }
    }

    public float GetNormalizedHP(int hp, int maxHealth)
    {
        return (float)hp / (float)maxHealth;
    }

    public void UpdateHealthUI()
    {
        Debug.Log("target" + player.playerCombat.targetScript.HP);

        HealthText.text = $"P1 HP: {player.HP}     |     P2 HP: {player.playerCombat.targetScript.HP}";
    }

    public void UpdateHonorUI()
    {
        HonorText.text = $"P1 Honor: {player.playerHonor[0]} | P2 Honor: {player.playerHonor[1]}";
    }

   
    public void UpdateChargeButtons()
    {
        if (player.gameController.Turn % 2 == 1)
        {
            playerChargeButton.interactable = player.TurnsSinceCharge >= 6;
            player.playerCombat.targetScript.playerUI.playerChargeButton.interactable = false;

            if (player.TurnsSinceCharge >= 6)
            {
                player.ResetCharge();
            }
        }
        else
        {
            playerChargeButton.interactable = false;
            player.playerCombat.targetScript.playerUI.playerChargeButton.interactable = player.playerCombat.targetScript.TurnsSinceCharge >= 6;

            if (player.playerCombat.targetScript.TurnsSinceCharge >= 6)
            {
                player.playerCombat.targetScript.ResetCharge();
            }
        }
    }

}
