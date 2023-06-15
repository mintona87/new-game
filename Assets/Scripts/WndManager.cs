using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DLGType
{
    CHAT_DLG
}
public class WndManager : MonoBehaviour
{
    public static WndManager instance;
    public GameObject m_chatDialog;
    public Canvas currentCanvas;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

    public void OpenDialog(DLGType dlgType)
    {
        GameObject digObj;
        if(currentCanvas != null)
        {
            switch(dlgType)
            {
                case DLGType.CHAT_DLG:
                    digObj = Instantiate(m_chatDialog, currentCanvas.transform);
                    digObj.transform.SetAsLastSibling();
                    break;
            }
            
        }
    }
}
