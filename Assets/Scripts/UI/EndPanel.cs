using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndPanel : MonoBehaviour, IGameUI
{
    [SerializeField] private TMP_Text _textGameResult;
    
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
