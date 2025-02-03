using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Dictionary<UIType, IPopupUI> _UICache = new();
    public RectTransform _popupUIPrefab;
    public Canvas canvas;
    
    void ConfirmCallback()
    {
        Debug.Log("Clicked Confirm");
        if (_UICache.TryGetValue(UIType.Popup_Confirm, out var popup)) popup.Hide();
    }

    void CancelCallback()
    {
        Debug.Log("Cancel Confirm");
        if (_UICache.TryGetValue(UIType.Popup_Confirm, out var popup)) popup.Hide();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (_UICache.TryGetValue(UIType.Popup_Confirm, out var popup))
            {
                popup.Show();
                return;
            }

            var confirmPopup = Instantiate(_popupUIPrefab, canvas.transform).GetComponent<PopupUI>();
            confirmPopup.SetConfirmCallback(ConfirmCallback);
            confirmPopup.SetCancelCallback(CancelCallback);
            confirmPopup.SetMessageText("게임을 종료하시겠습니까?");
            confirmPopup.SetButtonText("확인");
            
            _UICache[UIType.Popup_Confirm] = confirmPopup;
            
            confirmPopup.Show();
        }
    }
}

[Serializable]
public enum UIType
{
    None,
    Popup_Confirm,
    Max
}