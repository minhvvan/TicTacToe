using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct SigninData
{
    public string username;
    public string password;
}

public struct SigninResult
{
    public int result;
}

public class SigninPanelController : MonoBehaviour, IGameUI
{
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] Button signinButton;
    [SerializeField] Button signupButton;
    
    private void Awake()
    {
        signinButton.onClick.AddListener(OnClickSigninButton);
        signupButton.onClick.AddListener(OnClickSignupButton);
    }

    private void OnClickSigninButton()
    {
        var username = usernameInputField.text;
        var password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.Popup_Confirm);
            popup.SetMessageText("Username과 Password를 모두 입력해주세요.");
            popup.SetButtonText("확인");
            popup.SetConfirmCallback(() => { popup.Hide();});
            popup.Show();
            return;
        }

        StartCoroutine(NetworkManager.Instance.Signin(new SigninData() { username = username, password = password }, ShowStartPanel,
            () =>
            {
                var popup = UIManager.Instance.GetUI<PopupUI>(UIType.Popup_Confirm);
                popup.SetMessageText("로그인에 실패하였습니다.");
                popup.SetButtonText("확인");
                popup.SetConfirmCallback(() => { popup.Hide();});
                popup.Show();
            }));
    }

    private void ShowStartPanel()
    {
        Hide();
        UIManager.Instance.ShowUI(UIType.StartPanel);
    }

    private void OnClickSignupButton()
    {
        Hide();
        UIManager.Instance.ShowUI(UIType.SignupPanel);
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