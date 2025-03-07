using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private BlockController _blockController;
    [SerializeField] private TurnBox _turnBox;
    private PlayerType[,] _board;
    private PlayerType _currentTurn;
    
    private enum GameResult { None, PlayerAWin, PlayerBWin, Draw }
    
    private GameResult _gameResult;
    private GameMode _gameMode;
    private PlayerType _1PType;
    
    private MultiplayManager _multiplayManager;
    private string _roomId;
    
    public UserInfo UserInfo { get; set; }
    public Action<MessageData> OnReceiveMessage;

    public void ConnectToServer()
    {
        _multiplayManager = new MultiplayManager(ConnectServerHandler, ReceiveMessageHandler, PlaceMakerHandler, ChangeTurnHandler);
    }

    private void ChangeTurnHandler()
    {
        UnityThread.executeInUpdate(() =>
        {
            _currentTurn = _currentTurn == PlayerType.PlayerA ? PlayerType.PlayerB : PlayerType.PlayerA;
            _turnBox.SetTurn(_currentTurn);
        });
    }
    
    private void PlaceMakerHandler(PlaceMakerData data)
    {
        UnityThread.executeInUpdate(() =>
        {
            int row = data.index / 3;
            int col = data.index % 3;
            var maker = (PlayerType)data.makerType;

            _board[row, col] = maker;
            _blockController.PlaceMaker(maker, data.index);
            
            _gameResult = CheckEndGame();
            if (_gameResult != GameResult.None)
            {
                StartCoroutine(EndGame());
                return;
            }
        });
    }
    
    private void ReceiveMessageHandler(MessageData data)
    {
        OnReceiveMessage?.Invoke(data);
    }

    private void ConnectServerHandler(Constants.MultiplayManagerState state, string id)
    {
        switch (state)
        {
            case Constants.MultiplayManagerState.CreateRoom:
            {
                UnityThread.executeInUpdate(() =>
                {
                    _roomId = id;
                    GameManager.Instance.Set1PMaker(PlayerType.PlayerA);
                    UIManager.Instance.SwitchPanel(UIType.StartPanel, UIType.LobbyPanel);
                });
                break;
            }
            case Constants.MultiplayManagerState.JoinRoom:
            {
                UnityThread.executeInUpdate(() =>
                {
                    _roomId = id;
                    GameManager.Instance.Set1PMaker(PlayerType.PlayerB);
                    UIManager.Instance.SwitchPanel(UIType.StartPanel, UIType.LobbyPanel);
                });
                break;
            }
            case Constants.MultiplayManagerState.StartGame:
            {
                UnityThread.executeInUpdate(() =>
                {
                    SceneManager.LoadScene("Game");
                });
                break;
            }
            case Constants.MultiplayManagerState.EndGame:
            {
                break;
            }
        }
    }

    public void SendChatMessage(string message)
    {
        if (!_roomId.IsUnityNull() && !_multiplayManager.IsUnityNull())
        {
            _multiplayManager?.SendMessage(_roomId, UserInfo.nickname, message);
        }
    }

    private void InitGame()
    {
        _board = new PlayerType[3, 3];

        if (_blockController == null)
        {
            _blockController = GameObject.FindObjectOfType<BlockController>();
        }

        if (_turnBox == null)
        {
            _turnBox = GameObject.FindObjectOfType<TurnBox>();
        }
        
        _blockController.OnBlockClicked += OnBlockClicked;
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
        if (!_roomId.IsUnityNull() && !_multiplayManager.IsUnityNull())
        {
            _multiplayManager?.SendStartGame(_roomId);
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Main")
        {
            StartCoroutine(NetworkManager.Instance.GetScore((userData) =>
                {
                    UserInfo = userData;
                    UIManager.Instance.ShowUI(UIType.StartPanel);
                },
                () =>
                {
                    UIManager.Instance.ShowUI(UIType.SigninPanel);
                }));
        }
        else if (scene.name == "Game")
        {
            // 초기화 작업 수행
            InitGame();
        
            _currentTurn = PlayerType.PlayerA;
            _gameResult = GameResult.None;
            _turnBox.SetTurn(PlayerType.PlayerA);

            if (_gameMode == GameMode.Solo && _currentTurn != _1PType)
            {
                GetAIMaker();
            }

            _blockController.SetAllBlockActive(true);
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);
        
        _board = new PlayerType[3, 3];
        _blockController.CleanUp();
        
        var endPanel = UIManager.Instance.GetUI<EndPanel>(UIType.EndPanel);

        string gameResultStr = "";
        if (_gameResult == GameResult.Draw)
        {
            gameResultStr = "Draw";
            ScoreData score = new()
            {
                score = Constants.WinScore
            };
            StartCoroutine(NetworkManager.Instance.AddScore(score));
        }
        else
        {
            switch (_gameMode)
            {
                case GameMode.Solo:
                {
                    if ((_gameResult == GameResult.PlayerAWin && _1PType == PlayerType.PlayerA) ||
                        (_gameResult == GameResult.PlayerBWin && _1PType == PlayerType.PlayerB))
                    {
                        gameResultStr = "Win!!";

                        ScoreData score = new()
                        {
                            score = Constants.WinScore
                        };

                        StartCoroutine(NetworkManager.Instance.AddScore(score));
                    }
                    else
                    {
                        gameResultStr = "Lose..";
                    }
                
                    break;
                }
                case GameMode.Multi:
                {
                    if (_gameResult == GameResult.PlayerAWin)
                    {
                        if (_1PType == PlayerType.PlayerA) gameResultStr = "Win!!";
                        else if(_1PType == PlayerType.PlayerB) gameResultStr = "Lose..";
                    }
                    else if (_gameResult == GameResult.PlayerBWin)
                    {
                        if (_1PType == PlayerType.PlayerA) gameResultStr = "Lose..";
                        else if(_1PType == PlayerType.PlayerB) gameResultStr = "Win!!";
                    }
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        endPanel.SetGameResult(gameResultStr);
        UIManager.Instance.ShowUI(UIType.EndPanel);
    }

    private void OnBlockClicked(int index)
    {
        if (_currentTurn != _1PType) return;
        if (_gameResult != GameResult.None) return;
        
        PlaceMaker(index);
    }

    private void PlaceMaker(int index)
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

        if (_gameMode == GameMode.Multi)
        {
            _multiplayManager.SendPlaceMaker(_roomId, _currentTurn, index);
        }
        
        SetNextTurn();
    }

    private void SetNextTurn()
    {
        _gameResult = CheckEndGame();
        if (_gameResult != GameResult.None)
        {
            StartCoroutine(EndGame());
            return;
        }
        
        _currentTurn = _currentTurn == PlayerType.PlayerA ? PlayerType.PlayerB : PlayerType.PlayerA;
        _turnBox.SetTurn(_currentTurn);

        if (_gameMode == GameMode.Solo && _currentTurn != _1PType)
        {
            GetAIMaker();
        }
        else if (_gameMode == GameMode.Multi)
        {
            _multiplayManager.SendChangeTurn(_roomId);
        }
    }

    private void GetAIMaker()
    {
        var result = AIController.FindNextMove(_board);
        PlaceMaker(result.row * 3 + result.col);
    }

    private GameResult CheckEndGame()
    {
        GameResult currentResult = GameResult.None;
        List<(int, int)> matched = new();
        
        //row
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            if(_board[i, 0] == PlayerType.None) continue;
            
            if (_board[i, 0] == _board[i, 1] && _board[i, 1] == _board[i, 2])
            {
                currentResult = _board[i, 0] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
                matched.AddRange(new []{(i, 0), (i, 1), (i, 2)});
            }
        }
        
        //col
        for (var i = 0; i < _board.GetLength(1); i++)
        {
            if(_board[0, i] == PlayerType.None) continue;
            
            if (_board[0, i] == _board[1, i] && _board[1, i] == _board[2, i])
            {
                currentResult = _board[0, i] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
                matched.AddRange(new []{(0, i), (1, i), (2, i)});
            }
        }
        
        //diag
        if (_board[0, 0] != PlayerType.None && _board[0, 0] == _board[1, 1] && _board[1, 1] == _board[2, 2])
        {
            currentResult = _board[0, 0] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
            matched.AddRange(new []{(0, 0), (1, 1), (2, 2)});
        }
        
        if (_board[0, 2] != PlayerType.None && _board[0, 2] == _board[1, 1] && _board[1, 1] == _board[2, 0])
        {
            currentResult = _board[0, 2] == PlayerType.PlayerA ? GameResult.PlayerAWin : GameResult.PlayerBWin;
            matched.AddRange(new []{(0, 2), (1, 1), (2, 0)});
        }

        if (matched.Count != 0)
        {
            _blockController.SetBlockColor(matched, currentResult == GameResult.PlayerAWin ? GameColor.PlayerABlock : GameColor.PlayerBBlock);
        }
        
        return IsBoardFull() ? GameResult.Draw : currentResult;
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

    public void ExitGame()
    {
#if UNITY_EDITOR
        // 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서 실행 중일 때
            Application.Quit();
#endif
    }
    
    private void OnApplicationQuit()
    {
        _multiplayManager.Dispose();
    }
}

public enum PlayerType { None, PlayerA, PlayerB }
public enum GameMode { None, Solo, Multi }
