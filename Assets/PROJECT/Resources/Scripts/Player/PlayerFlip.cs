using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFlip : MonoBehaviour, IPointerClickHandler
{


    public void OnPointerClick(PointerEventData eventData)
    {
        // Flips the object
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        Debug.Log("PlayerImage was clicked");
    }
}
