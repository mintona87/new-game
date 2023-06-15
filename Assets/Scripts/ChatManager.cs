using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    string m_chat = "";
    [HideInInspector]
    public string chatMessage
    {
        get
        {
            return m_chat; 
        }
        set
        {
            m_chat = value;
            OnChatUpdated?.Invoke();
        }
    }
    public string statusStr = "";
    private string[] channelsToSubscribe;
    private ChatClient chatClient;
    ChatAppSettings chatAppSettings;
    public delegate void ChatUpadte();
    public ChatUpadte OnChatUpdated;

    public static ChatManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        chatAppSettings = GetChatAppSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
    }

    public void SendMessage(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage("general chat", message);
        }
    }

    public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)

    {

    }

    public void OnDisconnected()
    {
        //Debug.LogError("OnDisconnected");
        statusStr = "disconnected";
        Connect();
    }

    public void OnConnected ()
     {
        Debug.Log("OnConnected");
        //chatClient.Subscribe(new string[] { "public channel" });
        SetPublicChainnel();
        statusStr = "connected";
    }


    public void OnChatStateChange (ChatState state)
     {

    }


    public void OnGetMessages (string channelName, string[] senders, object[] messages)
    {
        for(int i = 0; i < senders.Length; i++)
        {
            chatMessage += string.Format("<color=green>{0}</color>: {1}<br>", senders[i], messages[i]);
        }
}


    public void OnPrivateMessage(string sender, object message, string channelName)

    {

    }


    public void OnSubscribed(string[] channels, bool[] results)
    {
        channelsToSubscribe = channels;
        //Debug.LogError("channel subscribed: " + channels[0]);
    }


    public void OnUnsubscribed (string[] channels)
     {
        //Debug.LogError("OnUnsubscribed: " + channels[0]);
    }


    public void OnStatusUpdate (string user, int status, bool gotMessage, object message)
     {

    }


    public void OnUserSubscribed(string channel, string user)

    {
        //Debug.LogError("OnUserSubscribed: " + channel);
    }


    public void OnUserUnsubscribed(string channel, string user)
    {
        //Debug.LogError("OnUserUnsubscribed: " + channel);
    }

    public void Connect()
    {
        chatClient = new ChatClient(this);
#if !UNITY_WEBGL
        chatClient.UseBackgroundWorkerForSending = true;
#endif
        chatClient.AuthValues = new AuthenticationValues(GlobalData.instance.userName);
        chatClient.ConnectUsingSettings(GetChatAppSettings());
        Debug.Log("connect to chat server");
    }

    public void SetPublicChainnel()
    {
        chatClient.Subscribe(new string[] { "general chat" });
    }

    ChatAppSettings GetChatAppSettings()
    {
        return new ChatAppSettings
        {
            AppIdChat = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            AppVersion = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion,
            FixedRegion = PhotonNetwork.PhotonServerSettings.AppSettings.IsBestRegion ? null : PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion,
            NetworkLogging = PhotonNetwork.PhotonServerSettings.AppSettings.NetworkLogging,
            Protocol = PhotonNetwork.PhotonServerSettings.AppSettings.Protocol,
            EnableProtocolFallback = PhotonNetwork.PhotonServerSettings.AppSettings.EnableProtocolFallback,
            Server = PhotonNetwork.PhotonServerSettings.AppSettings.IsDefaultNameServer ? null : PhotonNetwork.PhotonServerSettings.AppSettings.Server,
            Port = (ushort)PhotonNetwork.PhotonServerSettings.AppSettings.Port,
            ProxyServer = PhotonNetwork.PhotonServerSettings.AppSettings.ProxyServer
            // values not copied from AppSettings class: AuthMode
            // values not needed from AppSettings class: EnableLobbyStatistics 
        };
    }
    
    
}
