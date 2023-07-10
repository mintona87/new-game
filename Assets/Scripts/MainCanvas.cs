using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
   
    void Start()
    {
        if(WndManager.instance)
        {
            WndManager.instance.currentCanvas = this.GetComponent<Canvas>();
        }
    }

    

}
