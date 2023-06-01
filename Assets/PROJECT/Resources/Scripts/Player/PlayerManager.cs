using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    public int HP = 100;
    public int MaxHP = 100;
    public bool HasCharged = false;
    public bool isDefending = false;
    public bool isStunned = false;

    int ATK;
    int DEF;
    int SPD;
    int LUCK;

   public int Gold { get; private set; } = 0;

    public int TurnsSinceCharge = 6;


    public GameController gameController;
    public PlayerUI playerUI;
    public PlayerCombat playerCombat;
    public PlayerEffect playerEffect;
    public PhotonView pv;

    public float[] playerHonor = new float[2];

    public bool canPlay;

    public int playerNumber;

    public string localPlayerNickname;

    public bool isItMyPlayer;

    
    
    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerUI = GetComponent<PlayerUI>();
        playerCombat = GetComponent<PlayerCombat>();
        playerEffect = GetComponent<PlayerEffect>();
        pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        OnLoadingGameScreen.Instance.SetLoadingScreenActive(true);

        StartCoroutine(WaitFinishLoad());
    }

    ////tmp
    //private void Update()
    //{
    //    Debug.Log("spd " + SPD + " ismine " + isItMyPlayer);
    //}
    private void Update()
    {
        if (isItMyPlayer)
        {
            Debug.Log("charge: " + FindObjectOfType<PlayerManager>().TurnsSinceCharge);

            FindObjectOfType<DebugUI>().ChargeText.text = "charge " + TurnsSinceCharge;
        }
    }

    void InitPlayer()
    {
        Debug.Log("loacl player num" + playerNumber);
        playerUI.playerNumberText.text = "Player" + (playerNumber + 1).ToString();

        gameObject.name = playerUI.playerNumberText.text;
        //playerUI.playerUsernameText.text = localPlayerNickname;
        transform.SetParent(gameController.PlayerListContainer.transform);
        gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
        Debug.Log("finish load value set");
        gameController.didGameFinishLoad = true;

        ////tmp
        //if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        //{
        //    if (isItMyPlayer)
        //    {
        //        SPD = 1;
        //    }
        //    else
        //    {
        //        SPD = 5;
        //    }
        //}
        //else
        //{
        //    if (isItMyPlayer)
        //    {
        //        SPD = 5;
        //    }
        //    else
        //    {
        //        SPD = 1;
        //    }
        //}
        InitPlayerStatOnline();

        
    }

    void InitPlayerStatOnline()
    {
        PlayfabManager playfabManager = LaunchManager.Instance.playfabManager;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "ATK", playfabManager.GetPlayerSavedData().ATK } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "DEF", playfabManager.GetPlayerSavedData().DEF } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "LUCK", playfabManager.GetPlayerSavedData().LUCK } });
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SPD", playfabManager.GetPlayerSavedData().SPD } });
    }


    IEnumerator WaitFinishLoad()
    {
        InitPlayer();

        while (gameController.PlayerListContainer.transform.childCount != 2)
        {
            yield return null;
        }

        while (gameController.didGameFinishLoad == false)
        {
            Debug.Log("is Loading");
            yield return null;
        }
        Debug.Log("finish load");


        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == 0)
        {
            canPlay = true;
        }
        else
        {
            canPlay = false;
        }

        Debug.Log("getnum " + PhotonNetwork.LocalPlayer.GetPlayerNumber() + "playernum "+ playerNumber);
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == playerNumber)
        {
            isItMyPlayer = true;
        }
        Debug.Log("isitMyPlayer " +gameObject.name+" cond "+ isItMyPlayer);

        if (isItMyPlayer)
        {
            localPlayerNickname = PhotonNetwork.LocalPlayer.CustomProperties["Nickname"].ToString();
            playerUI.playerUsernameText.text = localPlayerNickname;

        }
        else
        {
            playerUI.playerUsernameText.text = playerCombat.GetOtherPlayer().CustomProperties["Nickname"].ToString();
        }


        FindObjectOfType<ActionTextHandler>().SetActionPosition();

        playerCombat.SetDefaultTarget();
        playerUI.SetMaxHealthSlider();
        playerUI.SetPlayerPicture();
        StartCoroutine(OnSwitchTurnSettings());

        // change is mine
        playerUI.SetActiveButtons(PhotonNetwork.LocalPlayer.IsLocal);

        gameController.SetPlayerList();

        playerUI.SetActiveTargetButtons(false);

        OnLoadingGameScreen.Instance.SetLoadingScreenActive(false);
    }

    public IEnumerator OnSwitchTurnSettings()
    {
        if (isItMyPlayer)
        {
            Debug.Log("buttonactivated" + canPlay + " didchoose " + PhotonNetwork.LocalPlayer.CustomProperties["DidFinishChoosingAction"]);


            playerUI.playerAttackButton.interactable = canPlay;
            playerUI.playerHealButton.interactable = canPlay;
            playerUI.playerDefendButton.interactable = canPlay;
            playerUI.playerChargeButton.interactable = canPlay && TurnsSinceCharge >= 6;
        }
        yield return null;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
    }
    public bool UpgradeStat(ref int stat, int tier)
    {
        int cost = tier * 100;

        if (Gold >= cost)
        {
            Gold -= cost;
            stat += 1;
            return true;
        }

        return false;
    }

    public bool UpgradeATK(int tier)
    {
        return UpgradeStat(ref ATK, tier);
    }

    public bool UpgradeDEF(int tier)
    {
        return UpgradeStat(ref DEF, tier);
    }

    public bool UpgradeSPD(int tier)
    {
        return UpgradeStat(ref SPD, tier);
    }

    public bool UpgradeLUCK(int tier)
    {
        return UpgradeStat(ref LUCK, tier);
    }
   
    public void ResetCharge()
    {
        HasCharged = false;
    }

    public void ChangeHP(int amount)
    {
        pv.RPC("ChangeHPRPC", RpcTarget.AllBuffered, amount);
    }

    [PunRPC]
    void ChangeHPRPC(int amount)
    {
        HP += amount;
        if (HP < 0)
        {
            HP = 0;
        }
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
        playerUI.SetHealthSlider(HP);

        if (HasLost())
        {
            // Set "Win" for a player different than the local player
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    if (!playerCombat.targetScript.HasLost())
                    {
                        player.CustomProperties["WonLost"] = "Won";
                        PhotonNetwork.LocalPlayer.CustomProperties["WonLost"] = "Lost";
                    }
                    else
                    {
                        player.CustomProperties["WonLost"] = "Lost";
                        PhotonNetwork.LocalPlayer.CustomProperties["WonLost"] = "Won";
                        OnPlayerWin();
                    }
                }
            }
            gameController.gameOverManager.DisplayPlayersGameOverObj();
        }
    }


    public int Attack()
    {
        int damage = Random.Range(10, 15) + ATK;

        if (IsCriticalHit())
        {
            damage = Mathf.RoundToInt(damage * 1.75f); // 1.75x damage for critical hits
        }

        if (isDefending)
        {
            damage = 0;
        }

        return damage;
    }

    public int Heal()
    {
        int healing = Random.Range(10, 15);
        if (IsCriticalHeal())
        {
            healing *= 2; // 2x healing for critical heals
        }
        return healing;
    }

    public void SetIsDefending(bool condition)
    {
        pv.RPC("SetIsDefendingRPC", RpcTarget.AllBuffered,condition);
    }
    [PunRPC]
    void SetIsDefendingRPC(bool condition)
    {
        isDefending = condition;
    }

    public int Charge(PlayerManager opponent)
    {
        int damage = 0;
        if (!HasCharged)
        {
            // Generate a random number between 0 and 1
            float random = UnityEngine.Random.Range(0f, 1f);

            // 70% chance of stun
            if (random <= 0.7)
            {
                Debug.Log("shouldSetOtherPlayerStun"+ playerCombat.GetOtherPlayer().GetPlayerNumber());
                Debug.Log("onchangestun2");
                playerCombat.GetOtherPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayerStun", "stun" } });
            }
            else
            {
                playerCombat.GetOtherPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayerStun", "notStun" } });
            }

            damage = Random.Range(20, 30);
            if (IsCriticalHit())
            {
                damage = Mathf.RoundToInt(damage * 1.75f); // 1.75x damage for critical hits
            }
            if (isDefending)
            {
                damage = 0;
            }
            HasCharged = true;
        }
        return damage;
    }
    public void OnPlayerWin()
    {
        Debug.Log("OnWin");
        PlayfabManager playfabManager = FindObjectOfType<PlayfabManager>();
        playfabManager.GetPlayerSavedData().Honor += 10;
        int honor = playfabManager.GetPlayerSavedData().Honor;
        playfabManager.SavePlayerSavedData(playfabManager.GetPlayerSavedData());
        Debug.Log("OnWin" + honor);
        playfabManager.SendLeaderboard(honor);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Honor", honor } });
    }

    public bool Dodge()
    {
        float baseDodgeChance = 10f; // 10% base dodge chance
        float dodgeChance = baseDodgeChance + (SPD * 2.5f); // 2.5% dodge chance per SPD point + base dodge chance
        float randomValue = Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= dodgeChance;
    }

    public bool IsCriticalHit()
    {
        float baseCritChance = 10f; // 10% base crit chance
        float criticalChance = baseCritChance + (LUCK * 2.5f); // 2.5% critical chance per LUCK point
        float randomValue = Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= criticalChance;
    }

    public bool IsCriticalHeal()
    {
        float baseCritChance = 10f; // 10% base crit chance
        float criticalChance = baseCritChance + (LUCK * 2.5f); // 2.5% critical chance per LUCK point
        float randomValue = Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= criticalChance;
    }

    public bool HasLost()
    {
        return HP <= 0;
    }
}
