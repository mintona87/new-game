using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUI : MonoBehaviour
{
    [SerializeField] GameObject uiContainer;

    PlayfabManager playfabManager;

    public Text NickNameText, RankText;

    public Toggle ToggleMatchWithEveryone;

    public TextMeshProUGUI ChargeText;


    void Start()
    {
        DontDestroyOnLoad(gameObject);
        playfabManager = FindObjectOfType<PlayfabManager>();
    }
    
    public void SetPlayer2ButtonPressed()
    {
        playfabManager.LoginUserNameInput.text = "tt@gl.com";
        playfabManager.LoginPasswordInput.text = "123456";
    }
    public void SetPlayer3ButtonPressed()
    {
        playfabManager.LoginUserNameInput.text = "rr@gh.com";
        playfabManager.LoginPasswordInput.text = "123456";
    }
}
