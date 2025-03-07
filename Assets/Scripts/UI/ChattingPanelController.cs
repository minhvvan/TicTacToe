using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChattingPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform messageTextParent;

    public Action<string> OnEditChattedMessage;
    
    public void OnEndEdit(string text)
    {
        var messageTextObject = Instantiate(messagePrefab, messageTextParent);
        messageTextObject.GetComponent<TMP_Text>().text = text;
        messageInputField.text = "";

        OnEditChattedMessage?.Invoke(text);
    }

    public void OnReceiveMessage(MessageData data)
    {
        UnityThread.executeInUpdate(() =>
        {
            var messageTextObject = Instantiate(messagePrefab, messageTextParent);
            messageTextObject.GetComponent<TMP_Text>().text = data.message;
        });
    }
}
