using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour, IGameUI
{
    [SerializeField] private RectTransform _popup;
    [SerializeField] private Button _buttonConfirm;
    [SerializeField] private Button _buttonCancel;
    [SerializeField] private TMP_Text _textMessage;
    [SerializeField] private TMP_Text _textButtonText;

    public delegate void OnClickCallback();
    private OnClickCallback _onConfirmClicked;

    void Awake()
    {
        _buttonConfirm.onClick.AddListener(OnConfirmClicked);
        _buttonCancel.onClick.AddListener(OnCancelClicked);
    }

    public void SetConfirmCallback(OnClickCallback callback)
    {
        _onConfirmClicked = callback;
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
        UIManager.Instance.HideUI(UIType.PopupConfirm);
    }

    public void Show()
    {
        _popup.transform.DOKill();
        gameObject.SetActive(true);
    
        _popup.transform.localScale = Vector3.zero;
        _popup.transform.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack);
    }
    
    public void Hide()
    {
        _popup.transform.DOKill();
    
        _popup.transform.DOScale(Vector3.zero, 0.2f)
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
