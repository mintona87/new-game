using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject walletConnectContent;
    public GameObject selectCharacterContent;
    private void Start()
    {
        walletConnectContent.SetActive(false);
        selectCharacterContent.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("BattleScreen");

    }
    public void OnClickSelectCharacter()
    {
        if(GlobalData.instance.isWalletConnected)
        {
            selectCharacterContent.SetActive(true);
        }
        else
        {
            walletConnectContent.SetActive(true);
        }
    }
}
