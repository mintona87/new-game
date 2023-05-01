using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    public string emailInput;
    public string passwordInput;
    public string nickname;
    public string accountID;


    public static PlayfabManager Instance;

    public bool IsitOnPlayerDebugMode;

    public Text playfabNameText;
    
    public int localPlayerHonor;

    public TMP_InputField LoginUserNameInput;
    public TMP_InputField LoginPasswordInput;

    public TMP_InputField RegisterEmailInput;
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;

    //DebugUI debugUI;
    PlayerSavedData playerSavedData;

    private void Awake()
    {
        LoginPasswordInput.inputType = TMP_InputField.InputType.Password;
        RegisterPasswordInput.inputType = TMP_InputField.InputType.Password;
        
        //debugUI = FindObjectOfType<DebugUI>();
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        OnLoadingGameScreen.Instance.SetLoadingScreenActive(false);
    }

    public PlayerSavedData GetPlayerSavedData()
    {
        return playerSavedData;
    }


    // Registering
    public void RegisterButtonPressed()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = RegisterEmailInput.text,
            Password = RegisterPasswordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, RegisterButtonPressedError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        // SHOW NICKNAME FORM
        Debug.Log("SHOW NICKNAME FORM");

        GetLoadedPlayerDatas();//load and create data if no data

        nickname = RegisterUsernameInput.text;
        //debugUI.NickNameText.text = nickname;

        SubmitNameButton();

        SendLeaderboard(1);

        LoginSettings();
    }

    #region Send Leaderboard initial stats

    // Send the base leaderboard when registering
    //UPDATE LEADERBOARD 
    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "Honor_Leaderboard", // 
                    Value = score
                    //Value = Random.Range(10,100) <- ⭐️ Use this to test out random send data
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, SendLeaderboardError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard sent!");

        GetStatistics();
    }
    void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            if(eachStat.StatisticName == "Honor_Leaderboard")
            {
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                localPlayerHonor = eachStat.Value;
            }
        }
    }

   

    #endregion

    // Logging in
    public void LoginButtonPressed()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = LoginUserNameInput.text,
            Password = LoginPasswordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Logged in!");

        // CHECK IF THE ACCOUNT HAS DISPLAY NAME, PROMPT FORM IF NOT
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        nickname = result.InfoResultPayload.PlayerProfile.DisplayName;
        //debugUI.NickNameText.text = nickname;

        GetLoadedPlayerDatas();
        
        if (name == null)
        {
            Debug.Log("no name, show set name form ...");
            SubmitNameButton();
        }
        else
        {
            LoginSettings();
        }
    }

    void LoginSettings()
    {
        Debug.Log("logged in w username " + name);

        PlayerPrefs.SetString("username", name);

        SceneManager.LoadScene("MainMenu");

        GetStatistics();//leaderboard stuffs
    }

    // SUBMITTING NICKNAME
    void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nickname
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, SubmitNameButtonError);
        PlayerPrefs.SetString("username", name);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
        SceneManager.LoadScene("MainMenu");
    }

    // Forgot password
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput,
            TitleId = PlayFabSettings.staticSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnForgotPasswordSuccess, OnResetPasswordError);
    }
    void OnForgotPasswordSuccess(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Sent password recovery link!");
    }
    #region Error
    void OnLoginError(PlayFabError error)
    {
        Debug.Log("onloginError " + error.ErrorMessage);
    }
    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    void OnResetPasswordError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    void OnSavePlayerDataError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }

    void GetLoadedPlayerDatasError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    void SubmitNameButtonError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    void SendLeaderboardError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }
    void RegisterButtonPressedError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }

    #endregion

    // SENDING JSON DATA
    public void SavePlayerSavedData(PlayerSavedData data)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> {
                {"PlayerSavedData", JsonConvert.SerializeObject(data) },
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnSavePlayerDataError);
    }

    public void SaveAll(PlayerSavedData data)
    {
        SavePlayerSavedData(data);
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successful JSON data send!");
    }

    // RECEIVING JSON DATA
    public void GetLoadedPlayerDatas()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharacterDataReceived, GetLoadedPlayerDatasError);
    }
    void OnCharacterDataReceived(GetUserDataResult result)
    {
        Debug.Log("Received data");
        if(result.Data != null && result.Data.ContainsKey("PlayerSavedData"))
        {
            PlayerSavedData loadedStats = JsonConvert.DeserializeObject<PlayerSavedData>(result.Data["PlayerSavedData"].Value);
            playerSavedData = loadedStats;
            Debug.Log("honor " + playerSavedData.Honor);
        }
        else
        {
            Debug.Log("inside no chara key");
            playerSavedData = new PlayerSavedData(
                1,//ATT
                1,//DEF
                1,//SPD
                1,//LUCK
                0,//Gold
                1//Honor
                  );
            SavePlayerSavedData(playerSavedData);
        }
    }
}
