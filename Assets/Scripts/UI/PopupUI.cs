using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour, IGameUI
{
    [SerializeField] private Button _buttonConfirm;
    [SerializeField] private Button _buttonCancel;
    [SerializeField] private TMP_Text _textMessage;
    [SerializeField] private TMP_Text _textButtonText;

    public delegate void OnClickCallback();

    private OnClickCallback _onConfirmClicked;
    private OnClickCallback _onCancelClicked;

    void Awake()
    {
        _buttonConfirm.onClick.AddListener(OnConfirmClicked);
        _buttonCancel.onClick.AddListener(OnCancelClicked);
    }

    public void SetConfirmCallback(OnClickCallback callback)
    {
        _onConfirmClicked = callback;
    }

    public void SetCancelCallback(OnClickCallback callback)
    {
        _onCancelClicked = callback;
    }

    public void SetMessageText(string text)
    {
        _textMessage.text = text;
    }
    
    public void SetButtonText(string text)
    {
        _textButtonText.text = text;
    }

    private void OnConfirmClicked()
    {
        _onConfirmClicked();
    }
    
    private void OnCancelClicked()
    {
        _onCancelClicked();
    }

    public void Show()
    {
        transform.DOKill();
        gameObject.SetActive(true);
    
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);
    }
    
    public void Hide()
    {
        transform.DOKill();
    
        transform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}

public interface IGameUI
{
    public void Show();
    public void Hide();
}
