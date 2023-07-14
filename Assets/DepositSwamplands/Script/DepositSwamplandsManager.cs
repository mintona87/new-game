using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using System;

#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#endif

public class DepositSwamplandsManager : MonoBehaviour
{
    //jslib with one function - open page that open _blank (new tab)
#if UNITY_WEBGL
    [DllImport("__Internal")] private static extern void OpenNewPage(string str);
#endif
    [Header("URLs")] // link to swamplands page for client to sign in.
    [SerializeField] private string _swamplandsClientAuthURL;

    [Header("UIs"), Space(20)]
    [SerializeField] private TMP_Text _currentBalanceGold;
    [SerializeField] private Button _swamplandsClientAuthBtn; // btn to call open page function
    [SerializeField] private TMP_InputField _depositAmountInput;
    [SerializeField] private Button _swamplandsDepositBtn; // btn to call cloud script "makeDepositRequest" function on playFab -> page in admin panel: Automation/Revisions(Legacy) 

    [SerializeField] private Button _getC4BalanceBtn;
    [SerializeField] private TMP_Text _resultMessageText;

    private string _playerPlayFabID = string.Empty;

    private void Start()
    {
        _swamplandsClientAuthBtn.onClick.AddListener(OnClickSwampLandsAuth);
        _swamplandsDepositBtn.onClick.AddListener(OnDepositBtnClick);
        _getC4BalanceBtn.onClick.AddListener(OnGetGoldBalanceBtnClick);

        _playerPlayFabID = GlobalData.instance?.playfabId;
        _resultMessageText.text = $"Your PlayFabID: {_playerPlayFabID}";
        Debug.Log($"Your PlayFabID: {_playerPlayFabID}");

        //check if user already connected
        GetSwampConnectionStatus();
    }
    private void OnClickSwampLandsAuth()
    {
    #if UNITY_WEBGL
        if (!string.IsNullOrWhiteSpace(_playerPlayFabID))
        {
            OpenNewPage(_swamplandsClientAuthURL + _playerPlayFabID);
        }
        else
            _resultMessageText.text = "GlobalData playfabId is empty";
    #endif
    }

    #region Swamplands C4 deposit
    private void OnDepositBtnClick()
    {
        bool isAmountValid = int.TryParse(_depositAmountInput.text, out int depositAmount) && depositAmount > 0;
        if (isAmountValid)
        {
            CallDepositOnPlayFab(depositAmount);
        }
        else
            _resultMessageText.text = "Wrong deposit amount";
    }

    private void CallDepositOnPlayFab(int c4Amount)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "makeDepositRequest",
            FunctionParameter = new { depositAmount = c4Amount },
            GeneratePlayStreamEvent = true 
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnMakeDepositRequestCompleted, OnPlayFabError);
        _resultMessageText.text = "Please wait...";
        _swamplandsDepositBtn.interactable = false;
    }

    private void OnMakeDepositRequestCompleted(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null /*&& result.FunctionResult is Dictionary<string,object> responceData*/)
        {
            string resultJson = result.FunctionResult.ToString();
            DepositResponse responseData = JsonUtility.FromJson<DepositResponse>(resultJson);
            if (responseData.responseContent.status == true)
            {
                //success deposit
                _currentBalanceGold.text = $"Balance: {responseData.responseContent.newBalance}";
                _resultMessageText.text = responseData.responseContent.msg;
                _swamplandsClientAuthBtn.interactable = false;
            }
            else
            {
                //fail deposit  
                _resultMessageText.text = responseData.responseContent.msg;
                //check if is token expired issue
                _swamplandsClientAuthBtn.interactable = true;
            }
        }
        else
        {
            _resultMessageText.text = "ServerError";
            Debug.LogError("ServerError");
        }
        if(result.Error != null)
        {
            string errorText = result.Error.Message;
            Debug.LogError(result.Error.StackTrace);
            _resultMessageText.text = errorText;
            Debug.LogWarning(errorText);
        }
        _swamplandsDepositBtn.interactable = true;
    }
    #endregion

    #region Get player gold balance

    private void GetSwampConnectionStatus()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "isPlayerSwampConnectionActive",
            FunctionParameter = new {},
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnGetSwampConnectionStatus, OnPlayFabError);
    }
    private void OnGetSwampConnectionStatus(ExecuteCloudScriptResult result)
    {
        string resultJson = result.FunctionResult.ToString();
        DepositResponse responseData = JsonUtility.FromJson<DepositResponse>(resultJson);

        _swamplandsClientAuthBtn.interactable = !responseData.responseContent.status;
        _resultMessageText.text = responseData.responseContent.msg;
    }

    private void OnGetGoldBalanceBtnClick()
    {
        var request = new GetPlayerCombinedInfoRequest();
        request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams() { GetUserVirtualCurrency = true };
        PlayFabClientAPI.GetPlayerCombinedInfo(request, OnGetGetGoldBalanceSuccess, OnPlayFabError);
    }

    private void OnGetGetGoldBalanceSuccess(GetPlayerCombinedInfoResult result)
    {
        if (result.InfoResultPayload != null && result.InfoResultPayload.UserVirtualCurrency.ContainsKey("GD"))
        {
            int goldBalance = result.InfoResultPayload.UserVirtualCurrency["GD"];
            _resultMessageText.text = $"Your balance: {goldBalance}"; 
            _currentBalanceGold.text = $"Balance: {goldBalance}";
        }
        else
        {
            string errorText = "No internal data for C4Balance found.";
            _resultMessageText.text = errorText;
            Debug.LogWarning(errorText);
        }
    }
    #endregion

    private void OnPlayFabError(PlayFab.PlayFabError error)
    {
        string errorReport = error.GenerateErrorReport();
        _resultMessageText.text = errorReport;
        Debug.LogError("playFab error: " + errorReport);
        _swamplandsDepositBtn.interactable = true;
    }
}

//Object that will be return as responce from playFab on success/fail callback

[Serializable]
public class DepositResponse
{
    public DepositResponseContent responseContent;
}

[Serializable]
public class DepositResponseContent
{
    public bool status; // success deposit true/false
    public string msg; // error/success msg
    public int newBalance; // current balance after deposit OR previous balance if status = false.
}