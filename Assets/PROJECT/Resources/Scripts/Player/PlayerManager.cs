using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;

public class PlayerManager : MonoBehaviourPunCallbacks
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
    public ActionTextHandler actionTextHandler;
    public PlayfabManager playfabManager;


    public float[] playerHonor = new float[2];

    public bool canPlay;

    public int playerNumber;

    public string localPlayerNickname;

    public bool isItMyPlayer;

    public int isDefendingTurnIndex;


    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        playerUI = GetComponent<PlayerUI>();
        playerCombat = GetComponent<PlayerCombat>();
        playerEffect = GetComponent<PlayerEffect>();
        pv = GetComponent<PhotonView>();
        playfabManager = FindObjectOfType<PlayfabManager>();
    }
    private void Start()
    {
        OnLoadingGameScreen.Instance.SetLoadingScreenActive(true);

        StartCoroutine(WaitFinishLoad());
    }

    //////tmp
    private void Update()
    {
        if (isItMyPlayer)
        {
            Debug.Log("ATK" +ATK);
            //Debug.Log("charge: " + FindObjectOfType<PlayerManager>().TurnsSinceCharge);
            //Debug.Log("isdefending " + (bool)PhotonNetwork.LocalPlayer.CustomProperties["isDefending"] + " index " + isDefendingTurnIndex);
        }
    }

    void InitPlayer()
    {
        Debug.Log("loacl player num" + playerNumber);
        //playerUI.playerNumberText.text = "Player" + (playerNumber + 1).ToString();

        gameObject.name = "Player" + (playerNumber + 1).ToString()/*playerUI.playerNumberText.text*/;
        //playerUI.playerUsernameText.text = localPlayerNickname;
        transform.SetParent(gameController.PlayerListContainer.transform);
        gameObject.transform.localScale = new Vector2(1.0f, 1.0f);
        Debug.Log("finish load value set");
        gameController.didGameFinishLoad = true;

        InitPlayerStatOnline();
    }

    void InitPlayerStatOnline()
    {
        Debug.Log("SavedATT " + playfabManager.GetPlayerSavedData().ATK);
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
    {
        { "ATK", playfabManager.GetPlayerSavedData().ATK },
        { "DEF", playfabManager.GetPlayerSavedData().DEF },
        { "LUCK", playfabManager.GetPlayerSavedData().LUCK },
        { "SPD", playfabManager.GetPlayerSavedData().SPD },
        { "isDefending", isDefending }
    };

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }


    #region custom prop updated callback

    // called after "PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { .... "is called
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Callback called playermanager");

        if (targetPlayer.IsLocal)
        {
            // Check if the "DidFinishChoosingAction" property has been updated
            if (changedProps.ContainsKey("ATK"))
            {
                Debug.Log("customATT " + (int)targetPlayer.CustomProperties["ATK"]);
                ATK = (int)targetPlayer.CustomProperties["ATK"];
                DEF = (int)targetPlayer.CustomProperties["DEF"];
                LUCK = (int)targetPlayer.CustomProperties["LUCK"];
                SPD = (int)targetPlayer.CustomProperties["SPD"];
            }
        }
    }

    #endregion


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

        Debug.Log("getnum " + PhotonNetwork.LocalPlayer.GetPlayerNumber() + "playernum " + playerNumber);
        if (PhotonNetwork.LocalPlayer.GetPlayerNumber() == playerNumber)
        {
            isItMyPlayer = true;
        }
        Debug.Log("isitMyPlayer " + gameObject.name + " cond " + isItMyPlayer);

        if (isItMyPlayer)
        {
            localPlayerNickname = PhotonNetwork.LocalPlayer.CustomProperties["Nickname"].ToString();
            playerUI.playerUsernameText.text = localPlayerNickname;
            playerUI.PlayerPicture.raycastTarget = true; 
        }
        else
        {
            playerUI.playerUsernameText.text = playerCombat.GetOtherPlayer().CustomProperties["Nickname"].ToString();
            playerUI.PlayerPicture.raycastTarget = false;
        }


        StartCoroutine(actionTextHandler.SetActionPosition());

        playerCombat.SetDefaultTarget();
        playerUI.SetMaxHealthSlider();
        if (isItMyPlayer)
        {
            playerUI.SetPlayerPicture(PhotonNetwork.LocalPlayer.CustomProperties["SpriteData"].ToString(), PhotonNetwork.LocalPlayer.CustomProperties["DefaultSpriteName"].ToString());
        }
        else
        {
            playerUI.SetPlayerPicture(playerCombat.GetOtherPlayer().CustomProperties["SpriteData"].ToString(), playerCombat.GetOtherPlayer().CustomProperties["DefaultSpriteName"].ToString());
        }
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

            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "isGameOver", true}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

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
            StartCoroutine(InitGameOver());
        }
    }


    public int Attack()
    {
        int damage = UnityEngine.Random.Range(10, 15) + ATK;

        if (IsCriticalHit())
        {
            damage = Mathf.RoundToInt(damage * 1.75f); // 1.75x damage for critical hits
        }

        if ((bool)playerCombat.GetOtherPlayer().CustomProperties["isDefending"])
        {
            damage = 0;
        }

        return damage;
    }

    public int Heal()
    {
        int healing = UnityEngine.Random.Range(10, 15);
        if (IsCriticalHeal())
        {
            healing *= 2; // 2x healing for critical heals
        }
        return healing;
    }

    public void SetIsDefending(Player player, bool condition)
    {
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isDefending", condition } });
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
                Debug.Log("shouldSetOtherPlayerStun" + playerCombat.GetOtherPlayer().GetPlayerNumber());
                Debug.Log("onchangestun2");
                playerCombat.GetOtherPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayerStun", "stun" } });
            }
            else
            {
                playerCombat.GetOtherPlayer().SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "isPlayerStun", "notStun" } });
            }

            damage = UnityEngine.Random.Range(20, 30);
            if (IsCriticalHit())
            {
                damage = Mathf.RoundToInt(damage * 1.75f); // 1.75x damage for critical hits
            }
            if ((bool)playerCombat.GetOtherPlayer().CustomProperties["isDefending"])
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
        string unit = playfabManager.getSelectedNFTData.unit;
        int honor = GlobalData.instance.GetNFTHonor(unit) + 10;
        GlobalData.instance.SaveNFTHonor(unit, honor);
        playfabManager.SavePlayerSavedData(playfabManager.GetPlayerSavedData());
        playfabManager.SendLeaderboard(honor);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Honor", honor } });
    }

    public bool Dodge()
    {
        float baseDodgeChance = 10f; // 10% base dodge chance
        float dodgeChance = baseDodgeChance + (SPD * 2.5f); // 2.5% dodge chance per SPD point + base dodge chance
        float randomValue = UnityEngine.Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= dodgeChance;
    }

    public bool IsCriticalHit()
    {
        float baseCritChance = 10f; // 10% base crit chance
        float criticalChance = baseCritChance + (LUCK * 2.5f); // 2.5% critical chance per LUCK point
        float randomValue = UnityEngine.Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= criticalChance;
    }

    public bool IsCriticalHeal()
    {
        float baseCritChance = 10f; // 10% base crit chance
        float criticalChance = baseCritChance + (LUCK * 2.5f); // 2.5% critical chance per LUCK point
        float randomValue = UnityEngine.Random.Range(0f, 100f); // Random value between 0 and 100
        return randomValue <= criticalChance;
    }

    public bool HasLost()
    {
        return HP <= 0;
    }
    IEnumerator InitGameOver()
    {
        while (!gameController.gameOverManager.isGameOver)
        {
            Debug.Log("is playing action");
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);

        gameController.gameOverManager.DisplayPlayersGameOverObj();
    }
}
