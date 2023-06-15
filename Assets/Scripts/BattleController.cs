using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnClickChatBtn()
    {
        WndManager.instance.OpenDialog(DLGType.CHAT_DLG);
    }
}
