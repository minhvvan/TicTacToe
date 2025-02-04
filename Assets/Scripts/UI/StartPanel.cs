using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour, IGameUI
{
    [SerializeField] private Button _button1P;
    [SerializeField] private Button _button2P;

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
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
