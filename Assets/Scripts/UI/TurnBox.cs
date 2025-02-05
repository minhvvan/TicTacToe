using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBox : MonoBehaviour
{
    [SerializeField] private Image _playerAImage;
    [SerializeField] private Image _playerBImage;
    
    [SerializeField] private ColorDictionary _colorDict;

    private void Awake()
    {
        _playerAImage.color = _colorDict._colors[GameColor.TurnOn];
        _playerBImage.color = _colorDict._colors[GameColor.TurnOff];
    }

    public void SetTurn(PlayerType type)
    {
        if (type == PlayerType.PlayerA)
        {
            _playerAImage.color = _colorDict._colors[GameColor.TurnOn];
            _playerBImage.color = _colorDict._colors[GameColor.TurnOff];
        }
        else
        {
            _playerAImage.color = _colorDict._colors[GameColor.TurnOff];
            _playerBImage.color = _colorDict._colors[GameColor.TurnOn];
        }
    }
}
