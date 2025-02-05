using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockedButton : Button
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        eventData.Use();
    }

    public override void OnPointerUp(PointerEventData eventData) 
    {
        base.OnPointerUp(eventData);
        eventData.Use();
    }
    
}
