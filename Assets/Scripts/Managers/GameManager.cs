using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController _blockController;
    private PlayerType[,] _board;
    private PlayerType _currentTurn;
    
    private enum GameResult { None, PlayerAWin, PlayerBWin, Draw }
    
    private GameResult _gameResult;
    private GameMode _gameMode;
    private PlayerType _1PType;

    private void Start()
    {
        InitGame();
        
        UIManager.Instance.ShowUI(UIType.StartPanel);
    }

    private void InitGame()
    {
        _board = new PlayerType[3, 3];
        _blockController.OnBlockClicked += OnBlockClicked;
    }

    public void StartGame()
    {
        UIManager.Instance.HideUI(UIType.StartPanel);
        _currentTurn = PlayerType.PlayerA;

        _blockController.SetAllBlockActive(true);
    }

    private void EndGame()
    {
        var endPanel = UIManager.Instance.GetUI<EndPanel>(UIType.EndPanel);

        string gameResultStr = "";
        switch (_gameMode)
        {
            case GameMode.Solo:
            {
                if ((_gameResult == GameResult.PlayerAWin && _1PType == PlayerType.PlayerA) ||
                    (_gameResult == GameResult.PlayerBWin && _1PType == PlayerType.PlayerB))
                {
                    gameResultStr = "Win!!";
                }
                else
                {
                    gameResultStr = "Lose..";
                }
                
                break;
            }
            case GameMode.Multi:
            {
                gameResultStr = _gameResult == GameResult.PlayerAWin ? "1P Win!!" : "2P Win!!";
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        endPanel.SetGameResult(gameResultStr);
        UIManager.Instance.ShowUI(UIType.EndPanel);
    }

    private void OnBlockClicked(int index)
    {
        if (_gameMode == GameMode.Solo && _currentTurn != _1PType) return;
        PlaceMaker(index);
    }

    public void PlaceMaker(int index)
    {
        int row = index / 3;
        int col = index % 3;

        if (_board[row, col] != PlayerType.None)
        {
            //TODO: 배치 불가 알림
            return;
        }
        
        _board[row, col] = _currentTurn;
        _blockController.PlaceMaker(_currentTurn, index);

        SetNextTurn();
    }

    private void SetNextTurn()
    {
        _gameResult = CheckEndGame();
        if (_gameResult != GameResult.None)
        {
            EndGame();
            return;
        }
        
        _currentTurn = _currentTurn == PlayerType.PlayerA ? PlayerType.PlayerB : PlayerType.PlayerA;
    }

    private GameResult CheckEndGame()
    {
        if (IsBoardFull()) return GameResult.Draw;
        
        //row
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            if(_board[i, 0] == PlayerType.None) continue;
            
            if (_board[i, 0] == _board[i, 1] && _board[i, 1] == _board[i, 2])
            {
                return _board[i, 0] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
            }
        }
        
        //col
        for (var i = 0; i < _board.GetLength(1); i++)
        {
            if(_board[0, i] == PlayerType.None) continue;
            
            if (_board[0, i] == _board[1, i] && _board[1, i] == _board[2, i])
            {
                return _board[0, i] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
            }
        }
        
        //diag
        if (_board[0, 0] != PlayerType.None && _board[0, 0] == _board[1, 1] && _board[1, 1] == _board[2, 2])
        {
            return _board[0, 0] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
        }
        
        if (_board[0, 2] != PlayerType.None && _board[0, 2] == _board[1, 1] && _board[1, 1] == _board[2, 0])
        {
            return _board[0, 2] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
        }
        
        return GameResult.None;
    }

    private bool IsBoardFull()
    {
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j] == PlayerType.None) return false;
            }
        }

        return true;
    }

    public void Set1PMaker(PlayerType playerType)
    {
        _1PType = playerType;
    }

    public void SetGameMode(GameMode mode)
    {
        _gameMode = mode;
    }
}

public enum PlayerType { None, PlayerA, PlayerB }
public enum GameMode { None, Solo, Multi }
