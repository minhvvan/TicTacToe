using System;
using System.Collections;
using System.Collections.Generic;
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
#if UNITY_EDITOR
            // 에디터에서 실행 중일 때
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 실행 중일 때
            Application.Quit();
#endif
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
