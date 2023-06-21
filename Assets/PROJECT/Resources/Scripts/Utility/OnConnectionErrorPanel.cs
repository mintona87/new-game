using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnConnectionErrorPanel : MonoBehaviourPunCallbacks
{
    public static OnConnectionErrorPanel Instance;

    public bool onlyOnceDisableLoadPanel;

    CanvasGroup canvasGroup;

    void Start()
    {
        Instance = this;
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();

        SetErrorPanelScreenActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void SetErrorPanelScreenActive(bool condition)
    {
        ActiveAnimation(condition);
    }

    void ActiveAnimation(bool condition)
    {
        if (condition)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.gameObject.SetActive(condition);
            onlyOnceDisableLoadPanel = false;
        }
        else
        {
            if (!onlyOnceDisableLoadPanel)
            {

                StartCoroutine(DisableModal(condition));
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // Call your function to display an error screen here
        Debug.Log("disconect cause " +cause.ToString());
        //SetErrorPanelScreenActive(true);
        ReconnectToServer();
    }


    void ReconnectToServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Attempting to reconnect...");
            PhotonNetwork.ConnectUsingSettings(); // Connects to Photon master servers using the settings provided via the PhotonServerSettings file.
            if(LaunchManager.Instance.disconnectedTime >= 30)
            {
                LaunchManager.Instance.disconnectedTime = 0;
                SetErrorPanelScreenActive(true);
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully reconnected to the server!");
        // Here you could also handle what should happen upon reconnection. For example, joining a room or a lobby.
    }

    void DestroyAllDontDestroyOnLoad()
    {
        Destroy(GameObject.Find("PlayfabManager"));
        Destroy(GameObject.Find("OnLoadingCanvas"));
        Destroy(GameObject.Find("DebugCanvas"));
        Destroy(GameObject.Find("PlayerDataContainer"));
        Destroy(GameObject.Find("GlobalData"));
        Destroy(GameObject.Find("DownloadManager"));
    }


    public void TryAgainClicked()
    {
        FindObjectOfType<PlayfabManager>().SelectedNftImageURL = "";
        DestroyAllDontDestroyOnLoad();
        SceneManager.LoadScene(0);
        SetErrorPanelScreenActive(false);
        Destroy(gameObject);
    }


    IEnumerator DisableModal(bool condition)
    {
        yield return new WaitForSeconds(1f);
        canvasGroup.gameObject.SetActive(condition);
        onlyOnceDisableLoadPanel = true;
    }
}
