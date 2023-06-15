using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatDialog : MonoBehaviour
{
    public TMP_Text msgText;
    public TMP_Text statusText;
    public TMP_InputField messageInput;
    // Start is called before the first frame update
    void Start()
    {
        msgText.text = ChatManager.instance.chatMessage;
        ChatManager.instance.OnChatUpdated += OnChatUpdated;
    }

    private void OnEnable()
    {
        statusText.text = ChatManager.instance.statusStr;
    }
    public void OnClickSendMessage()
    {
        ChatManager.instance.SendMessage(messageInput.text);
    }

    public void OnChatUpdated()
    {
        msgText.text = ChatManager.instance.chatMessage;
    }

    public void OnClickClose()
    {
        Destroy(gameObject);
    }
}
