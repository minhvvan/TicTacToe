using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour, IGameUI
{
    [SerializeField] private TMP_Text _textGameResult;
    [SerializeField] private Button _buttonMenu;
    [SerializeField] private Button _buttonExit;

    private void Awake()
    {
        _buttonMenu.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main");
        });

        _buttonExit.onClick.AddListener(() =>
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
            popup.SetButtonText("확인");
            popup.SetMessageText("게임을 종료하시겠습니까?");
            popup.SetConfirmCallback(GameManager.Instance.ExitGame);
        
            UIManager.Instance.ShowUI(UIType.PopupConfirm);
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
    
    public void SetGameResult(string str)
    {
        _textGameResult.text = str;
    }
}
