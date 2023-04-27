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
    PlayfabManager playfabManager;

    [SerializeField] GameObject startGameButton;

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

    bool isJoinMatchClicked;

    void Awake()
    {
        Instance = this;
        isConnectedOnMaster = false;
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
        //while (characterBox.GetStats().country == null)
        //{
        //	//Debug.Log("inside");
        //	yield return null;
        //}
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = characterBox.GetStats().country;

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

    private void FixedUpdate()
    {
        //Debug.Log("country " +PhotonNetwork.CloudRegion);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            OnLoadingScreen.Instance.SetLoadingScreenActive(false);
        }
        else
        {
            OnLoadingScreen.Instance.SetLoadingScreenActive(true);
        }
        //Debug.Log("isitConnectedAndready " + PhotonNetwork.IsConnectedAndReady);
        //Debug.Log("inlobby " + PhotonNetwork.InLobby);
    }
    public void JoinMatchClicked()
    {
        isJoinMatchClicked = true;
        //RoomLoadingScreen.SetActive(true);

        UpdateLobby();
        LoadingText.text = "Searching room...";

        Debug.Log("join match clicked");
    }

    public void PlayMultiButtonClicked()
    {
        Debug.Log("PlayButtonPressed");
    }

    public void OnPlayButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("LoadBattleScreen", RpcTarget.All);
        }
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

        Debug.Log("custom rank " + PhotonNetwork.LocalPlayer.CustomProperties["Honor"].ToString());

        if (Math.Abs(int.Parse(getCustomPropValue) - int.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Honor"].ToString())) <= 500000)
        {
            shouldMatch = true;
        }
        else
        {
            shouldMatch = false;
        }

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
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Honor", playfabManager.localPlayerHonor);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Nickname", playfabManager.nickname);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("WonLost", "null");
    }

    public void ModifyPlayerCustomHonor(int honor)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Honor"] = honor;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
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
                Debug.Log("Called1" + getPlayer.CustomProperties["Nickname"].ToString());

                GameObject playerRoomObj = Instantiate(PlayerRoomObjPrefab, Vector3.zero, Quaternion.identity);

                playerRoomObj.transform.SetParent(PlayerRoomObjContainerObj.transform);

                int playerHonor = Convert.ToInt32(getPlayer.CustomProperties["Honor"]);
                playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
                (
                    getPlayer.GetPlayerNumber() + 1,
                    getPlayer.CustomProperties["Nickname"].ToString(),
                    playerHonor,
                    "matchmaking"
                );
            }
        }
        else
        {
            GameObject playerRoomObj = Instantiate(PlayerRoomObjPrefab, Vector3.zero, Quaternion.identity);

            playerRoomObj.transform.SetParent(PlayerRoomObjContainerObj.transform);
            int playerHonor = Convert.ToInt32(player.CustomProperties["Honor"]);

            playerRoomObj.GetComponent<PlayerRoomObjHandler>().SetUpPlayerInfo
                (player.GetPlayerNumber() + 1,
                player.CustomProperties["Nickname"].ToString(),
                playerHonor,
                "matchmaking"
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

            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButtonObj.SetActive(true);
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Hashtable customProperties = new Hashtable();

        //int NonSpectatorPlayerCount = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["NonSpectatorPlayerCount"].ToString());
        //NonSpectatorPlayerCount -= 1;
        //customProperties["NonSpectatorPlayerCount"] = NonSpectatorPlayerCount;
        //PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButtonObj.SetActive(true);
            }
        }

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("listUpdate called");

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
        roomType = "";
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
