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
    [SerializeField] private Button _buttonLeaderboard;

    private void Start()
    {
        _button1P.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(UIType.PopupSelectMaker);
            GameManager.Instance.SetGameMode(GameMode.Solo);
        });
        
        _button2P.onClick.AddListener(() =>
        {
            //TODO: 서버 연결
            GameManager.Instance.SetGameMode(GameMode.Multi);
            GameManager.Instance.ConnectToServer();
        });
        
        _buttonExit.onClick.AddListener(() =>
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
            popup.SetButtonText("확인");
            popup.SetMessageText("게임을 종료하시겠습니까?");
            popup.SetConfirmCallback(GameManager.Instance.ExitGame);
        
            UIManager.Instance.ShowUI(UIType.PopupConfirm);
        });

        _buttonLeaderboard.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(UIType.LeaderboardPanel);
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
