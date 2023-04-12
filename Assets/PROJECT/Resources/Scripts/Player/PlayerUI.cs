using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider hpSlider;
    Player player;

    void Start()
    {
        player = GetComponent<Player>();
        SetMaxHealth();
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

    public float GetNormalizedHP(int hp, int maxHealth)
    {
        return (float)hp / (float)maxHealth;
    }
}
