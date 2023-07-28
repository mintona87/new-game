using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

#if UNITY_WEBGL
    using System.Runtime.InteropServices;
#endif

public class DepositSwamplandsManager : MonoBehaviour
{
    //jslib with one function - open page that open _blank (new tab)
#if UNITY_WEBGL
    [DllImport("__Internal")] private static extern void OpenNewPage(string str);
    [DllImport("__Internal")] private static extern void FocusTabChangeListen();
#endif
    [Header("URLs")] // link to swamplands page for client to sign in.
    [SerializeField] private string _swamplandsClientAuthURL;

    [Header("UIs"), Space(20)]
    [SerializeField] private TMP_Text _currentBalanceGold;
    [SerializeField] private Button _swamplandsClientAuthBtn; // btn to call open page function
    [SerializeField] private TMP_Text _connectBtnText;
    [SerializeField] private TMP_InputField _depositAmountInput;
    [SerializeField] private Button _swamplandsDepositBtn; // btn to call cloud script "makeDepositRequest" function on playFab -> page in admin panel: Automation/Revisions(Legacy) 

    [SerializeField] private Button _getC4BalanceBtn;
    [SerializeField] private TMP_Text _resultMessageText;

    private string _playerPlayFabID = "";
    private bool _isUserConnected = false;

    private IEnumerator Start()
    {
        _swamplandsClientAuthBtn.onClick.AddListener(SwamplandsAuth);
        _swamplandsDepositBtn.onClick.AddListener(OnDepositBtnClick);
        _getC4BalanceBtn.onClick.AddListener(OnGetGoldBalanceBtnClick);

        _playerPlayFabID = GlobalData.instance?.playfabId;
        _resultMessageText.text = "";

        //check if user already connected
#if UNITY_WEBGL
        FocusTabChangeListen();
#endif
        OnGetGoldBalanceBtnClick();
        yield return new WaitForSeconds(2);

        GetSwampConnectionStatus();
    }
    private void SwamplandsAuth()
    {
#if UNITY_WEBGL
        if (!string.IsNullOrWhiteSpace(_playerPlayFabID))
        {
            if (!_isUserConnected)
            {
                _resultMessageText.text = "Please wait...";
                OpenNewPage(_swamplandsClientAuthURL + _playerPlayFabID);
            }
            else
                CallDisconnectOnPlayFab();
        }
        else
            _resultMessageText.text = "Player ID error.";
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
            _resultMessageText.text = "Wrong deposit amount.";
    }

    public void OnWindowActivate()
    {
        Debug.Log("RUN CHECK IN 3sec");
        if(gameObject.activeSelf)
            StartCoroutine(WindowActivateIE());
    }
    IEnumerator WindowActivateIE()
    {
        _resultMessageText.text = "Please wait...";
        yield return new WaitForSeconds(1.5f);
        GetSwampConnectionStatus();
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

    private void CallDisconnectOnPlayFab()
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "disconnectSwamplads",
            FunctionParameter = new {},
            GeneratePlayStreamEvent = true
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnDisconnectOnPlayFabCompleted, OnPlayFabError);
        _resultMessageText.text = "Please wait...";
        _swamplandsDepositBtn.interactable = false;
    }

    public void OnDisconnectOnPlayFabCompleted(ExecuteCloudScriptResult result)
    {
        _isUserConnected = false;
        _resultMessageText.text = "Disconnected from swamplands.";
        _connectBtnText.text = _isUserConnected ? "Disconnect from Swamplands" : "Connect to Swamplands";
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
                _currentBalanceGold.text = $"Gold balance: {responseData.responseContent.newBalance}";
                _resultMessageText.text = responseData.responseContent.msg;
                //_swamplandsClientAuthBtn.interactable = false;
            }
            else if(responseData.responseContent.code == 1)
            {
                _resultMessageText.text = "Please wait...";
                SwamplandsAuth();
            }
            else
            {
                //fail deposit  
                _resultMessageText.text = responseData.responseContent.msg;
                //check if is token expired issue
                GetSwampConnectionStatus();
                //_swamplandsClientAuthBtn.interactable = true;
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
        _swamplandsDepositBtn.interactable = false;
        _resultMessageText.text = "Please wait...";
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

        _isUserConnected = responseData.responseContent.status;
        _swamplandsDepositBtn.interactable = _isUserConnected;
        Debug.Log("CHECK: " + _isUserConnected);
        _resultMessageText.text = _isUserConnected ? responseData.responseContent.msg : "Please connect to swamplands first.";
        _connectBtnText.text = _isUserConnected ? "Disconnect from Swamplands" : "Connect to Swamplands";
    }

    private void OnGetGoldBalanceBtnClick()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharacterDataReceived, OnPlayFabError);
    }

    private void OnCharacterDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("PlayerSavedData"))
        {
            PlayerSavedData playerSavedData = JsonConvert.DeserializeObject<PlayerSavedData>(result.Data["PlayerSavedData"].Value);
            int goldBalance = playerSavedData.Gold;
            _resultMessageText.text = $"Your gold balance: {goldBalance} gold";
            _currentBalanceGold.text = $"Gold balance: {goldBalance}";
        }
        else
        {
            string errorText = "Empty";
            int goldBalance = 0;
            _currentBalanceGold.text = $"Gold balance: {goldBalance}";
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
    public int code; // 1 - not connected; // 0 - connected
    public bool status; // success deposit true/false
    public string msg; // error/success msg
    public int newBalance; // current balance after deposit OR previous balance if status = false.
}