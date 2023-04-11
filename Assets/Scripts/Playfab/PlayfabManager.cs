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

    //public CharacterBox characterBoxes;//set in the inspector

    public bool IsitOnPlayerDebugMode;

    public Text playfabNameText;
    
    public int localPlayerELOScore;

    public TMP_InputField LoginUserNameInput;
    public TMP_InputField LoginPasswordInput;

    public TMP_InputField RegisterEmailInput;
    public TMP_InputField RegisterUsernameInput;
    public TMP_InputField RegisterPasswordInput;

    //LocalSaveSystemManager localSaveSystemManager;

    //DebugUI debugUI;


    private void Awake()
    {
        LoginPasswordInput.inputType = TMP_InputField.InputType.Password;
        RegisterPasswordInput.inputType = TMP_InputField.InputType.Password;
        //debugUI = FindObjectOfType<DebugUI>();
        //localSaveSystemManager = FindObjectOfType<LocalSaveSystemManager>();


    }
    private void Start()
    {
        if (IsitOnPlayerDebugMode)
        {
            LoginUserNameInput.text = "25@x.com";
            LoginPasswordInput.text = "123456";
        }
        DontDestroyOnLoad(gameObject);
    }

    // Registering
    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = RegisterEmailInput.text,
            Password = RegisterPasswordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        // SHOW NICKNAME FORM
        Debug.Log("SHOW NICKNAME FORM");
        
        GetLoadedCharacterDatas();//load and create data if no data

        nickname = RegisterUsernameInput.text;
        //debugUI.NickNameText.text = nickname;

        SubmitNameButton();

        SendLeaderboard(1200);

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
                    StatisticName = "ELO", // <- CHANGE YOUR LEADERBOARD NAME HERE!
                    Value = score
                    //Value = Random.Range(10,100) <- ⭐️ Use this to test out random send data
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
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
            if(eachStat.StatisticName == "ELO")
            {
                //Launcher.Instance.rankELOFromPlayfab = eachStat.Value;
                Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
                localPlayerELOScore = eachStat.Value;
                //debugUI.playfabLocalPlayerELOText.text = "ELO: " + localPlayerELOScore;
            }
        }
    }

   

    #endregion

    // Logging in
    public void LoginButton()
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
    void OnLoginError(PlayFabError error)
    {
        Debug.Log("onloginError " +error.ErrorMessage);
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

        GetLoadedCharacterDatas();
        
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

        SceneManager.LoadScene("Menu");

        GetStatistics();//leaderboard stuffs
    }

    // SUBMITTING NICKNAME
    void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nickname
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        PlayerPrefs.SetString("username", name);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
        SceneManager.LoadScene("Menu");
        // leaderboardWindow.SetActive(true);
    }

    // Forgot password
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput,
            TitleId = PlayFabSettings.staticSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnForgotPasswordSuccess, OnError);
    }
    void OnForgotPasswordSuccess(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Sent password recovery link!");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error: " + error.ErrorMessage);
    }

    // SENDING JSON DATA
    public void SaveCharacters()
    {
        //Debug.Log("losses box saved " + characterBoxes.GetStats().losses +"  "+ characterBoxes.GetStats().wins);
        var request = new UpdateUserDataRequest
        {
            //Data = new Dictionary<string, string> {
            //    {"Characters", JsonConvert.SerializeObject(characterBoxes.GetStats()) },
            //}
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void SaveAll()
    {
        SaveCharacters();
        SaveCharacterGraphicStats();
    }
    public void SaveCharacterGraphicStats()
    {
        var request = new UpdateUserDataRequest
        {
            //Data = new Dictionary<string, string> {
            //    {"CharacterGraphicStats", JsonConvert.SerializeObject(FindObjectOfType<CharacterGraphicBox>().GetGraphicStats())}
            //}
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }



    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successful JSON data send!");
    }



    // RECEIVING JSON DATA
    public void GetLoadedCharacterDatas()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharacterDataReceived, OnError);
    }
    void OnCharacterDataReceived(GetUserDataResult result)
    {
        Debug.Log("Received data");
        if(result.Data != null && result.Data.ContainsKey("Characters"))
        {
            //CharacterStats loadedStats = JsonConvert.DeserializeObject<CharacterStats>(result.Data["Characters"].Value);
            //Debug.Log("countrtystat" +loadedStats.country);
            //characterBoxes.SetStats(loadedStats);
            //characterBoxes.SetUi(loadedStats);
        }
        else
        {
            Debug.Log("inside no chara key");
            //characterStats = new CharacterStats(
            //    "TestCreateDataName",
            //    "SphericalHat",
            //    0.0f,//level
            //    100,//health
            //    0,//losses
            //    0,//wins
            //    FindObjectOfType<ChooseCountryHandler>().selectedCountry);
            //characterBoxes.SetStats(characterStats);
            //SaveCharacters();
            //localSaveSystemManager.SetAndSaveCharacterStats();
        }
    }

    #region Graphic Datas
    public void GetLoadedCharacterGraphicDatas()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharacterGraphicDataReceived, OnError);
    }
    void OnCharacterGraphicDataReceived(GetUserDataResult result)
    {
       
        if (result.Data != null && result.Data.ContainsKey("CharacterGraphicStats"))
        {
            Debug.Log("Received graphic data");
            //CharacterGraphicStats loadedGraphicStats = JsonConvert.DeserializeObject<CharacterGraphicStats>(result.Data["CharacterGraphicStats"].Value);

            //FindObjectOfType<CharacterGraphicBox>().SetGraphicStats(loadedGraphicStats);
        }
        else
        {
            Debug.Log("inside no graphic key");

            //FindObjectOfType<CharacterGraphicBox>().SetGraphicStats(characterGraphicStats);
            SaveCharacterGraphicStats();
            //localSaveSystemManager.SetAndSaveCharacterGraphicStats();
        }
    }
    #endregion
}
