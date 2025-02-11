using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour, IGameUI
{
    [SerializeField] private Button _button1P;
    [SerializeField] private Button _button2P;
    [SerializeField] private Button _buttonExit;

    private void Start()
    {
        _button1P.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(UIType.Popup_Select_Maker);
            GameManager.Instance.SetGameMode(GameMode.Solo);
        });
        
        _button2P.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
            GameManager.Instance.SetGameMode(GameMode.Multi);
        });
        
        _buttonExit.onClick.AddListener(() =>
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.Popup_Confirm);
            popup.SetButtonText("확인");
            popup.SetMessageText("게임을 종료하시겠습니까?");
            popup.SetConfirmCallback(GameManager.Instance.ExitGame);
        
            UIManager.Instance.ShowUI(UIType.Popup_Confirm);
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(Screen.width, 0);
        rect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.InQuart);
    }

    public void Hide()
    {
        RectTransform rect = GetComponent<RectTransform>();
    
        // 화면 왼쪽으로 이동
        rect.DOAnchorPos(new Vector2(-Screen.width, 0), 0.3f)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => 
            {
                gameObject.SetActive(false);
            });
    }
}
