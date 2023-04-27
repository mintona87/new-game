using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordSlashEffectController : MonoBehaviour
{
    public void DisableSwordSlashEffect()
    {
        GetComponent<Image>().enabled = false;
    }
}
