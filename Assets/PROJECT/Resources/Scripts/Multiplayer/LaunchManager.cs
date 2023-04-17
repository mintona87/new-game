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

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public static LaunchManager Instance;
    PlayfabManager playfabManager;

    [SerializeField] GameObject startGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList;

    public string roomType;// set in ChooseModeHandler.cs
    public bool isConnectedOnMaster;

    public GameObject insideRoomUi;
    public GameObject RoomLoadingScreen;
    public GameObject PlayMultiButton;
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
        if (PhotonNetwork.LocalPlayer.CustomProperties["Rank"] == null)
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
        RoomLoadingScreen.SetActive(true);

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

    public void CreateRoom()
    {
        //if (string.IsNullOrEmpty(roomNameInputField.text))
        //{
        //	//roomNameInputField.text = "testRoom2"; //tmp for debug
        //	return;
        //}
        //int maxPlayer = 2;
        //bool isVisible = true;
        //object customPropKey = "null key";
        //object customPropValue = "null value";
        //PhotonNetwork.CreateRoom(roomNameInputField.text, GetRoomOptions(maxPlayer, isVisible, customPropKey, customPropValue));

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

        Debug.Log("custom rank " + PhotonNetwork.LocalPlayer.CustomProperties["Rank"].ToString());

        if (Math.Abs(int.Parse(getCustomPropValue) - int.Parse(PhotonNetwork.LocalPlayer.CustomProperties["Rank"].ToString())) <= 5)
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
            object customPropValue = PhotonNetwork.LocalPlayer.CustomProperties["Rank"].ToString();

            PhotonNetwork.CreateRoom(rndRoomName.ToString(), GetRoomOptions(maxPlayer, isVisible, customPropKey, customPropValue));
        }
    }

    void SetPlayerCustomProperties()
    {
        PhotonNetwork.LocalPlayer.CustomProperties.Add("Rank", playfabManager.localPlayerHonor);
    }


    public void ModifyPlayerCustomRank(int rank)
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Rank"] = rank;
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

        roomOption.MaxPlayers = 7;
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


    public override void OnJoinedRoom()
    {

        Debug.Log("joinedroom");
        

        //Player[] players = PhotonNetwork.PlayerList;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            insideRoomUi.SetActive(true);
            RoomLoadingScreen.SetActive(false);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButton.SetActive(true);
            }
        }

        //foreach (Transform child in playerListContent)
        //{
        //	Destroy(child.gameObject);
        //}

        //Debug.Log("player count" + players.Count());

        //for (int i = 0; i < players.Count(); i++)
        //{
        //	GameObject playerListItemObj = Instantiate(PlayerListItemPrefab, playerListContent);
        //	playerListItemObj.GetComponent<PlayerListItemHandler>().SetUp(players[i]);
        //}
        //startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        LoadingText.text = "Searching for other players...";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("playerentered");

        //Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItemHandler>().SetUp(newPlayer);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            insideRoomUi.SetActive(true);
            RoomLoadingScreen.SetActive(false);
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                PlayMultiButton.SetActive(true);
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
                PlayMultiButton.SetActive(true);
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
