using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private SpriteRenderer makerSpriteRenderer;
    private SpriteRenderer bgSpriteRenderer;
    [SerializeField] private ColorDictionary _colorDict;

    public Action OnClicked;
    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set => isActive = value;
    }

    private void Awake()
    {
        isActive = false;
        bgSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMaker(PlayerType type)
    {
        if (type == PlayerType.None)
        {
            CleanUp();
            return;
        }
        
        makerSpriteRenderer.sprite = type == PlayerType.PlayerA ? oSprite : xSprite;
    }

    public void CleanUp()
    {
        OnClicked = null;
        makerSpriteRenderer.sprite = null;
        bgSpriteRenderer.color = _colorDict._colors[GameColor.DefaultBlock];
    }

    private void OnMouseUpAsButton()
    {
        if (!isActive) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        OnClicked?.Invoke();
    }

    public void SetColor(GameColor color)
    {
        bgSpriteRenderer.DOColor(_colorDict._colors[color], .3f).SetEase(Ease.OutCubic);
    }
}
