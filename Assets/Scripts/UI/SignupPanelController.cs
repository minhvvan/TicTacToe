using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SignupPanelController : MonoBehaviour, IGameUI
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;

    
    private void Awake()
    {
        confirmButton.onClick.AddListener(OnClickConfirmButton);
        confirmButton.onClick.AddListener(OnClickCancelButton);
    }
    
    private void OnClickConfirmButton()
    {
        var username = usernameInputField.text;
        var nickname = nicknameInputField.text;
        var password = passwordInputField.text;
        var confirmPassword = confirmPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
            popup.SetMessageText("Please, enter all fields");
            popup.SetButtonText("확인");
            popup.SetConfirmCallback(() => { popup.Hide();});
            popup.Show();
            
            CleanInputFields();
            return;
        }

        if (!password.Equals(confirmPassword))
        {
            var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
            popup.SetMessageText("passwords do not match");
            popup.SetButtonText("확인");
            popup.SetConfirmCallback(() => { popup.Hide();});
            popup.Show();

            passwordInputField.text = "";
            confirmPasswordInputField.text = "";
            return;
        }

        SignupData signupData = new SignupData();
        signupData.username = username;
        signupData.nickname = nickname;
        signupData.password = password;
        
        StartCoroutine(NetworkManager.Instance.Signup(signupData, Hide, FailureSignup));
    }

    private void FailureSignup()
    {
        var popup = UIManager.Instance.GetUI<PopupUI>(UIType.PopupConfirm);
        popup.SetMessageText("회원 가입에 실패하였습니다.");
        popup.SetButtonText("확인");
        popup.SetConfirmCallback(() => { popup.Hide();});
        popup.Show();

        CleanInputFields();
    }

    private void CleanInputFields()
    {
        usernameInputField.text = "";
        nicknameInputField.text = "";
        passwordInputField.text = "";
        confirmPasswordInputField.text = "";
    }

    private void OnClickCancelButton()
    {
        CleanInputFields();
        Hide();
        UIManager.Instance.ShowUI(UIType.SigninPanel);
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
