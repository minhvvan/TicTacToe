using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelController : MonoBehaviour, IGameUI
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private ChattingPanelController chattingPanelController;

    private void Start()
    {
        //TODO: guest, host 판단 필요
        // 판단에 의해 게임 start버튼 활성화 여부 결정
        gameStartButton.onClick.AddListener(() =>
        {
            GameManager.Instance.StartGame();
        });

        //채팅 이벤트
        chattingPanelController.OnEditChattedMessage += (text) =>
        {
            GameManager.Instance.SendChatMessage(text);
        };
        
        GameManager.Instance.OnReceiveMessage += OnReceiveMessage;
    }

    private void OnReceiveMessage(MessageData data)
    {
        chattingPanelController.OnReceiveMessage(data);
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
