using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectMakerUI : PopupUI
{
    [SerializeField] private Button _buttonO;
    [SerializeField] private Button _buttonX;

    void Awake()
    {
        _buttonO.onClick.AddListener(() =>
        {
            UIManager.Instance.HideUI(UIType.PopupSelectMaker);
            GameManager.Instance.Set1PMaker(PlayerType.PlayerA);
            GameManager.Instance.StartGame();
        });
        
        _buttonX.onClick.AddListener(() =>
        {
            GameManager.Instance.Set1PMaker(PlayerType.PlayerB);
            UIManager.Instance.HideUI(UIType.PopupSelectMaker);
            GameManager.Instance.StartGame();
        });
    }
}
