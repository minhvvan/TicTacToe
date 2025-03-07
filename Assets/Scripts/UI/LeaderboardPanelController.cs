using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPanelController : MonoBehaviour, IGameUI
{
    [SerializeField] private GameObject panel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button myRankButton;
    [SerializeField] private CellController myRankCell;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        myRankButton.onClick.AddListener(MoveMyRank);
    }

    private void MoveMyRank()
    {
        var pos = scrollRect.content.transform.localPosition;
        var layout = scrollRect.content.GetComponent<VerticalLayoutGroup>();
        var cellHeight = cellPrefab.GetComponent<RectTransform>().rect.height;
        
        pos.y = (myRankCell.rank - 1) * (cellHeight + layout.spacing) + layout.padding.top;
        scrollRect.content.transform.localPosition = pos;
    }

    public void Show()
    {
        StartCoroutine(SetLeaderboard());
        gameObject.SetActive(true);
        
        var originPos = panel.transform.localPosition;
        panel.transform.DOLocalMoveY(-Screen.height, 0f);
        panel.transform.transform.DOLocalMoveY(originPos.y, 1f).SetEase(Ease.OutQuint);
    }

    public void Hide()
    {
        var originPos = panel.transform.localPosition;
        panel.transform.DOLocalMoveY(-Screen.height, 1f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            panel.transform.localPosition = originPos;
            gameObject.SetActive(false);
        });
    }

    IEnumerator SetLeaderboard()
    {
        yield return StartCoroutine(NetworkManager.Instance.GetLeaderboard((users) =>
        {
            var content = scrollRect.content.GetComponent<RectTransform>();
            var layout = scrollRect.content.GetComponent<VerticalLayoutGroup>();
            var cellHeight = cellPrefab.GetComponent<RectTransform>().rect.height;
            content.sizeDelta = new Vector2(content.sizeDelta.x, (cellHeight + layout.spacing) * users.Count + layout.padding.top);
            
            for (var i = 0; i < users.Count; i++)
            {
                var cell = Instantiate(cellPrefab, scrollRect.content);
                if (cell.TryGetComponent<CellController>(out var cellController))
                {
                    cellController.SetUsername(users[i].username);
                    cellController.SetScore(users[i].score);
                    cellController.SetRank(i+1);
                    cellController.SetColor(users[i].username.Equals(GameManager.Instance.UserInfo.username));
                }
                
                if (users[i].username.Equals(GameManager.Instance.UserInfo.username))
                {
                    myRankCell.SetUsername(users[i].username);
                    myRankCell.SetScore(users[i].score);
                    myRankCell.SetRank(i+1);
                }
            }
        }));
    }
}
