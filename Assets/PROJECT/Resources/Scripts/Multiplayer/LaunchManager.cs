using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using Photon.Pun.UtilityScripts;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public static LaunchManager Instance;
    public PlayfabManager playfabManager;

    [SerializeField] GameObject PlayMultiButton;

    private Dictionary<string, RoomInfo> cachedRoomList;


    public string roomType;// set in ChooseModeHandler.cs
    public bool isConnectedOnMaster;

    public GameObject insideRoomUiObj;
    public GameObject RoomLoadingScreen;
    public GameObject PlayMultiButtonObj;
    public GameObject PlayerRoomObjContainerObj;
    public GameObject PlayerRoomObjPrefab;
    public TextMeshProUGUI PlayerCountText;

    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI LookingForOpponentText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI RoomHonorText;

    bool isJoinMatchClicked;


    // increase honor every minute when searching for other player
    bool shouldStartSearchHonorTimer;
    bool isItOnRematch;
    float timeBeforeIncreaseHonorOtherPlayer;
    int otherPlayerhonorDifference;
    //
    void Awake()
    {
        Instance = this;
        
        timeBeforeIncreaseHonorOtherPlayer = 5.0f;
        otherPlayerhonorDifference = 100;
        RoomHonorText.text = "Honor of the room : " + otherPlayerhonorDifference.ToString();
        isConnectedOnMaster = false;
        shouldStartSearchHonorTimer = false;
        roomType = "";
        playfabManager = FindObjectOfType<PlayfabManager>();
        cachedRoomList = new Dictionary<string, RoomInfo>();
    }
    void Start()
    {
        StartCoroutine(WaitForPlayfabDataToLoad());
    }

    IEnumerator WaitForPlayfabDataToLoad()
    {
        while (playfabManager.localPlayerHonor == -1)
        {
            Debug.Log("inside honor did not load");
            yield return null;
        }
       // PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "jp";//tmp

        PhotonNetwork.ConnectUsingSettings();
        yield return null;
        //Debug.Log("Connecting to Master country2 " + characterBox.GetStats().country);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        isConnectedOnMaster = true;
        if (PhotonNetwork.LocalPlayer.CustomProperties["Honor"] == null)
        {
            SetPlayerCustomProperties();
        }
        SetLocalPlayerNickName();

    }

    void SetLocalPlayerNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = playfabManager.nickname;
    }


    #region Update

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            OnLoadingScreen.Instance.SetLoadingScreenActive(false);
            //Debug.Log("devregion " +PhotonNetwork.CloudRegion);
        }
        else
        {
            OnLoadingScreen.Instance.SetLoadingScreenActive(true);
        }

        Debug.Log("isitConnectedAndready " + PhotonNetwork.IsConnectedAndReady);
        Debug.Log("inlobby " + PhotonNetwork.InLobby);
    }

    private void Update()
    {
        Debug.Log("otherplayerhonordif" + otherPlayerhonorDifference + " time "+ timeBeforeIncreaseHonorOtherPlayer);
        //Debug.Log("getnfthonor " + GlobalData.instance.GetNFTHonor(playfabManager.getSelectedNFTData.unit) + " unit " + playfabManager.getSelectedNFTData.unit);
        if (shouldStartSearchHonorTimer)
        {
            if (timeBeforeIncreaseHonorOtherPlayer > 0)
            {
                timeBeforeIncreaseHonorOtherPlayer -= Time.deltaTime;
                TimerText.text = Mathf.Round(timeBeforeIncreaseHonorOtherPlayer).ToString()+"s" ;
            }
            else
            {
                Debug.Log("Timer has finished. Resetting...");
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.LeaveRoom();
                }
                isItOnRematch = true;
                timeBeforeIncreaseHonorOtherPlayer = 15;
            }
        }
    }

    #endregion

    public void JoinMatchClicked()
    {
        isJoinMatchClicked = true;
        shouldStartSearchHonorTimer = true;
        UpdateLobby();
        LoadingText.text = "Searching room...";

        Debug.Log("join match clicked");
    }

    public void OnPlayButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                photonView.RPC("LoadBattleScreen", RpcTarget.All);
            }
        }
    }

    public void LeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    void LoadBattleScreen()
    {
        // Replace "GameScene" with the name of your actual game scene
        PhotonNetwork.LoadLevel("BattleScreen");
    }

    
    public void UpdateLobby()
    {
        // used to call OnRoomListUpdate() callback when open rank menu
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    void JoinRoomWithSameRankOrCreate(string roomName, string getCustomPropValue, bool isOpen, bool isItFirstRoom)
    {
        bool shouldMatch = false;
        Debug.Log("getCustomPropValue " + getCustomPropValue);
        if (getCustomPropValue == "")
        {
            Debug.Log("isnull");
            getCustomPropValue = "0";// to avoid int.Parse error
        }

        int twoPlayersHonorSubstraction = Math.Abs(int.Parse(getCustomPropValue) - int.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Honor"].ToString()));

        Debug.Log("custom rank " + PhotonNetwork.LocalPlayer.CustomProperties["Honor"].ToString()+" calcresult "+ twoPlayersHonorSubstraction);

        Debug.Log("honor diffrences " + twoPlayersHonorSubstraction + " otherdifference "+ otherPlayerhonorDifference);

        if (FindObjectOfType<DebugUI>().ToggleMatchWithEveryone.isOn)
        {
            otherPlayerhonorDifference = 50000;
        }

        if (twoPlayersHonorSubstraction <= otherPlayerhonorDifference)
        {
            shouldMatch = true;
        }
        else
        {
            shouldMatch = false;
        }

        Debug.Log("shouldMatch " + shouldMatch + "isOpen"+ isOpen+ " isItFirstRoom "+ isItFirstRoom);

        if (shouldMatch == true && isOpen == true && isItFirstRoom == false)
        {
            Debug.Log("inside join room");
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.Log("inside create room ");
            int rndRoommNumberName = UnityEngine.Random.RandomRange(0, 999999);
            string rndRoomName = "Room : " + rndRoommNumberName.ToString();
            int maxPlayer = 2;
            bool isVisible = true;
            object customPropKey = "RoomRank";
            object customPropValue = PhotonNetwork.LocalPlayer.CustomProperties["Honor"].ToString();

            PhotonNetwork.CreateRoom(rndRoomName.ToString(), GetRoomOptions(maxPlayer, isVisible, customPropKey, customPropValue));
        }
    }

    void SetPlayerCustomProperties()
    {
        
        int honor = 0;
#if UNITY_WEBGL
        //Debug.Log("getnfthonor " + GlobalData.instance.GetNFTHonor(playfabManager.getSelectedNFTData.unit) + " unit " + playfabManager.getSelectedNFTData.unit);
        //honor = GlobalData.instance.GetNFTHonor(playfabManager.getSelectedNFTData.unit);
#else
        honor = playfabManager.localPlayerHonor;
#endif

        
        PhotonNetwork.LocalPlayer.CustomProperties.Add("SpriteData", "");
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Honor", honor);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Nickname", playfabManager.nickname);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("WonLost", "null");
        PhotonNetwork.LocalPlayer.CustomProperties.Add("DidFinishChoosingAction",false );
        PhotonNetwork.LocalPlayer.CustomProperties.Add("SPD", 0);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("ATK", 0);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("DEF", 0);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("LUCK", 0);

        PhotonNetwork.LocalPlayer.CustomProperties.Add("SharedRandomNumber", 0 );
        PhotonNetwork.LocalPlayer.CustomProperties.Add("isPlayerStun", "notStun");
        PhotonNetwork.LocalPlayer.CustomProperties.Add("isPlayingAction", false);
    }

    public void ModifyPlayerCustomHonor(int honor)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Honor"] = honor;
    }

    public void ModifyPlayerCustomImageURL(string URL)
    {
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable { { "SpriteData", URL } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }

    public int GetCustomHonor()
    {
        return (int)PhotonNetwork.LocalPlayer.CustomProperties["Honor"];
    }

    public void JoinRoom(RoomInfo info)// for customize and training room 
    {
        Debug.Log("inside rrom typeranked " + roomType);
        if (info.PlayerCount < 2)
        {
            PhotonNetwork.JoinRoom(info.Name);
        }
        else
        {
            Debug.Log("The room is full");
        }
    }


    #region Photon Callback
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created.");
    }

    RoomOptions GetRoomOptions(int maxplayer, bool isVisible, object customPropKey, object customPropValue)
    {
        RoomOptions roomOption = new RoomOptions();

        roomOption.MaxPlayers = 2;
        roomOption.CustomRoomProperties = new Hashtable();
        roomOption.IsVisible = isVisible;
        roomOption.CustomRoomProperties.Add(customPropKey, customPropValue);
        roomOption.CustomRoomProperties.Add("RoomType", roomType);
        roomOption.CustomRoomPropertiesForLobby = new string[]
        { customPropKey.ToString(), customPropValue.ToString(),
            "RoomType", roomType,
        };
        return roomOption;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }


    IEnumerator WaitToFinishLoadPlayerInfo(Player player, bool isItJoining)
    {
        while(player.GetPlayerNumber() == -1)
        {
            Debug.Log("Called2"+ player.GetPlayerNumber());
        yield return null;
        }
        if (isItJoining)
        {
            // Create a sorted list of players by player number
            List<Player> sortedPlayers = PhotonNetwork.CurrentRoom.Players.Values.OrderBy(p => p.GetPlayerNumber()).ToList();

            foreach (Player getPlayer in sortedPlayers)
            {
                Debug.Log("Called1" + PhotonNetwork.LocalPlayer.CustomProperties["Nickname"].ToString());

                GameObject playerRoomObj = PhotonNetwork.Instantiate("Prefabs/PlayerRoomObj"/*PlayerRoomObjPrefab*/, Vector3.zero, Quaternion.identity);

                playerRoomObj.transform.SetParent(PlayerRoomObjContainerObj.transform);

                int playerHonor = Convert.ToInt32(PhotonNetwork.LocalPlayer.CustomProperties["Honor"]);

                playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
                (
                    getPlayer.GetPlayerNumber() + 1,
                    getPlayer.CustomProperties["Nickname"].ToString(),
                    playerHonor,
                    "",
                    "matchmaking",
                    getPlayer.CustomProperties["SpriteData"].ToString()
                );
            }           
        }
        else
        {
            GameObject playerRoomObj = PhotonNetwork.Instantiate("Prefabs/PlayerRoomObj"/*PlayerRoomObjPrefab*/, Vector3.zero, Quaternion.identity);

            playerRoomObj.transform.SetParent(PlayerRoomObjContainerObj.transform);
            int playerHonor = Convert.ToInt32(player.CustomProperties["Honor"]);

            playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
                (player.GetPlayerNumber() + 1,
                player.CustomProperties["Nickname"].ToString(),
                playerHonor,
                "",
                "matchmaking",
                player.CustomProperties["SpriteData"].ToString()
                ) ;
        }
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(WaitToFinishLoadPlayerInfo(PhotonNetwork.LocalPlayer, true));

        PlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        Debug.Log("joinedroom");

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        
        insideRoomUiObj.SetActive(true);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButtonObj.SetActive(true);
            }
            shouldStartSearchHonorTimer = false;
        }
        LoadingText.text = "Searching for other players...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("playerentered");

        StartCoroutine(WaitToFinishLoadPlayerInfo(newPlayer, false));

        PlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/"+ PhotonNetwork.CurrentRoom.MaxPlayers;


        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            insideRoomUiObj.SetActive(true);
        
            RoomLoadingScreen.SetActive(false);

            shouldStartSearchHonorTimer = false;

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButtonObj.SetActive(true);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Remove the playerRoomObj when the player leaves the room
        int otherPlayerNumber = otherPlayer.GetPlayerNumber();

        foreach (Transform child in PlayerRoomObjContainerObj.transform)
        {
                Destroy(PlayerRoomObjContainerObj.transform.GetChild(otherPlayerNumber).gameObject);
                break;
        }
        // Update player count text
        PlayerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButtonObj.SetActive(true);
            }
            shouldStartSearchHonorTimer = false;
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    IEnumerator WaitForMasterSwitch()
    {
        while (PhotonNetwork.IsMasterClient == false)
        {
            yield return null;
        }
        PlayMultiButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartCoroutine(WaitForMasterSwitch());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("listUpdate called " + roomList.Count);

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                if (cachedRoomList.Count != 0)
                {
                    //update cachedRoom list
                    if (cachedRoomList.ContainsKey(room.Name))
                    {
                        cachedRoomList[room.Name] = room;
                    }
                    //add the new room to the cached room list
                    else
                    {
                        cachedRoomList.Add(room.Name, room);
                    }
                }
            }
        }


        //foreach (RoomInfo room in cachedRoomList.Values)
        //{
        //    Debug.Log("instantiate called");
        //    GameObject roomListEntryGameobject = Instantiate(roomListItemPrefab);
        //    Debug.Log("joinroomListSpectatormodeRoomlist " + PhotonNetwork.LocalPlayer.CustomProperties["Spectator"].ToString() + " local " + localIsItInSpectatorMode);

        //    roomListEntryGameobject.transform.SetParent(roomListItemContainer.transform);
        //    roomListEntryGameobject.transform.localScale = Vector3.one;
        //    roomListEntryGameobject.transform.localPosition = Vector3.one;

        //    roomListEntryGameobject.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = room.Name;
        //    roomListEntryGameobject.transform.Find("RoomTypeText").GetComponent<TextMeshProUGUI>().text = room.CustomProperties["RoomType"].ToString();
        //    roomListEntryGameobject.transform.Find("NumOfPlayerInText").GetComponent<TextMeshProUGUI>().text =
        //        room.CustomProperties["NonSpectatorPlayerCount"].ToString() + " / " + 2;

        //    roomListEntryGameobject.transform.Find("JoinButton").GetComponent<Button>().onClick.AddListener(() => JoinRoom(room));

        //    roomListGameobjects.Add(room.Name, roomListEntryGameobject);
        //}

        //if (roomType == "Ranked")
        //{

        //}
        if (isJoinMatchClicked)
        {
            SearchForSameRankedRoom(roomList);
            isJoinMatchClicked = false;
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("onleftroom");
        foreach (Transform PlayerRoomObj in PlayerRoomObjContainerObj.transform)
        {
            Destroy(PlayerRoomObj.gameObject);
        }
        insideRoomUiObj.SetActive(false);
        
        shouldStartSearchHonorTimer = false;
        roomType = "";

        if (isItOnRematch)
        {
            otherPlayerhonorDifference += 50;
            RoomHonorText.text = "Honor of the room : " + otherPlayerhonorDifference.ToString();
            JoinMatchClicked();
            isJoinMatchClicked = true;
            isItOnRematch = false;
        }
        else
        {
            otherPlayerhonorDifference = 100;
        }
    }
    #endregion


    void SearchForSameRankedRoom(List<RoomInfo> roomList)
    {
        Debug.Log("roomtype1 ");
        if (roomList.Count == 0)
        {
            Debug.Log("roomtype2 ");
            JoinRoomWithSameRankOrCreate("", "", true,true);//create room
        }
        else
        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].CustomProperties["RoomRank"] != null) // check it is not custom room or training room
                {
                    //Debug.Log("listroomcutomprop " + roomList[i].CustomProperties["IsOpenToNonSpectatorPlayer"].ToString());
                    //if ((bool)roomList[i].CustomProperties["IsOpenToNonSpectatorPlayer"] == true)
                    //{
                        Debug.Log("roomtype3 " + roomList[i].CustomProperties["RoomRank"].ToString());
                        JoinRoomWithSameRankOrCreate(roomList[i].Name, roomList[i].CustomProperties["RoomRank"].ToString(), roomList[i].IsOpen,false);//join room 
                        break;
                    //}
                    //else
                    //{
                    //    Debug.Log("roomtype4 ");
                    //    JoinRoomWithSameRankOrCreate("", "", true);//create room
                    //    break;
                    //}
                }
                else
                {
                    JoinRoomWithSameRankOrCreate("", "", true,true);//create room
                    break;
                }
            }
        }
    }


    


}