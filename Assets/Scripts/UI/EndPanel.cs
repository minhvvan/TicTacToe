using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
            UIManager.Instance.ShowUI(UIType.StartPanel);
            UIManager.Instance.HideUI(UIType.EndPanel);
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

    public void SetGameResult(string str)
    {
        _textGameResult.text = str;
    }
}
