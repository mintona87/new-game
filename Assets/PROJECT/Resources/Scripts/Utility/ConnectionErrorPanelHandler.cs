using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionErrorPanelHandler : MonoBehaviourPunCallbacks
{

    public GameObject ErrorContainerObj;
    public GameObject playfabManagerObj;
    public GameObject OnLoadingCanvasObj;
    public GameObject PlayerInfoCanvasObj;

    private float timeLostFocus;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GetDontDestroyOnLoadObjects();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            // The application just regained focus
            float timeSpentOutOfFocus = Time.realtimeSinceStartup - timeLostFocus;
            if (timeSpentOutOfFocus > 60)
            {
                // If the player was out of focus for more than 60 seconds, show an error message
                ShowErrorPanel();
            }
        }
        else
        {
            // The application just lost focus
            timeLostFocus = Time.realtimeSinceStartup;
        }
    }

    // This function is called when the local user/client disconnects
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected due to: " + cause.ToString());
        // Show the Error Panel
        ShowErrorPanel();
    }

    void ShowErrorPanel()
    {
        if (ErrorContainerObj != null)
        {
            ErrorContainerObj.SetActive(true);
        }
        else
        {
            Debug.LogError("No reference to an ErrorPanelCanvas GameObject");
        }
    }


    void DestroyDontDestroyOnLoadObjects()
    {
        Debug.Log("DestroyDontDestrorOnLoad");
        Destroy(playfabManagerObj);
        Destroy(OnLoadingCanvasObj);
        Destroy(PlayerInfoCanvasObj);
    }

    void GetDontDestroyOnLoadObjects()
    {
        playfabManagerObj = GameObject.Find("PlayfabManager");
        OnLoadingCanvasObj = GameObject.Find("OnLoadingCanvas");
        PlayerInfoCanvasObj = GameObject.Find("PlayerInfoCanvas");
    }


    public void OkButtonClicked()
    {
        DestroyDontDestroyOnLoadObjects();
        // go back to login screen
        SceneManager.LoadScene(0);
        ErrorContainerObj.SetActive(false);
    }

}
