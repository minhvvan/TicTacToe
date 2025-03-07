using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UIType, IGameUI> _UICache = new();
    [SerializeField] private SerializedDictionary<UIType, RectTransform> _uiPrefabs = new();
    public Canvas canvas;

    void ConfirmCallback()
    {
        if (_UICache.TryGetValue(UIType.PopupConfirm, out var popup)) popup.Hide();
    }

    void CancelCallback()
    {
        if (_UICache.TryGetValue(UIType.PopupConfirm, out var popup)) popup.Hide();
    }

    public void ShowUI(UIType type)
    {
        if (_UICache.TryGetValue(type, out var gameUI) && !gameUI.IsUnityNull())
        {
            gameUI.Show();
            return;
        }
        
        var ui = Instantiate(_uiPrefabs[type], canvas.transform).GetComponent<IGameUI>();
        _UICache[type] = ui;
            
        ui.Show();
    }
    
    public void HideUI(UIType type)
    {
        if (_UICache.TryGetValue(type, out var gameUI))
        {
            gameUI.Hide();
        }
    }

    public T GetUI<T>(UIType type) where T : class, IGameUI
    {
        if (_UICache.TryGetValue(type, out var gameUI) &&  !gameUI.IsUnityNull())
        {
            return gameUI as T;
        }
        
        var ui = Instantiate(_uiPrefabs[type], canvas.transform).GetComponent<IGameUI>();
        return (_UICache[type] = ui) as T;
    }

    public void SwitchPanel(UIType hidePanel, UIType showPanel)
    {
        HideUI(hidePanel);
    
        // 약간의 딜레이 후 새 패널을 보여줌
        DOVirtual.DelayedCall(0.1f, () =>
        {
            ShowUI(showPanel);
        });
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }
}

[Serializable]
public enum UIType
{
    None,
    PopupConfirm,
    PopupSelectMaker,
    StartPanel,
    EndPanel,
    SigninPanel,
    SignupPanel,
    LeaderboardPanel,
    LobbyPanel,
    Max
}