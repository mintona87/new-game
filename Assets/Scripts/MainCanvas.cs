using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(WndManager.instance)
        {
            WndManager.instance.currentCanvas = this.GetComponent<Canvas>();
        }
    }
}
