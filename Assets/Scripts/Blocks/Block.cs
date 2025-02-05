using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Block : MonoBehaviour
{
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private SpriteRenderer makerSpriteRenderer;

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
    }

    private void OnMouseUpAsButton()
    {
        if (!isActive) return;
        OnClicked?.Invoke();
    }
}
