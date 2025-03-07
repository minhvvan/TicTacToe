using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private Button _buttonExit;

    private void Awake()
    {
        _buttonExit.onClick.AddListener(OnClickExit);
    }

    private void OnClickExit()
    {
        var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
        popup.SetButtonText("확인");
        popup.SetMessageText("게임을 종료하시겠습니까?");
        popup.SetConfirmCallback(GameManager.Instance.ExitGame);
        
        UIManager.Instance.ShowUI(UIType.PopupConfirm);
    }
}
