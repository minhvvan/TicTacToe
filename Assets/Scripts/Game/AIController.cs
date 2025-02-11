using UnityEngine;

public static class AIController
{
    private static PlayerType startType;
    
    public static (int row, int col) FindNextMove(PlayerType[,] board)
    {
        // 현재 턴 결정
        int oCnt = 0, xCnt = 0;
        foreach (var cell in board)
        {
            if (cell == PlayerType.PlayerA) oCnt++;
            else if (cell == PlayerType.PlayerB) xCnt++;
        }
        startType = oCnt == xCnt ? PlayerType.PlayerA : PlayerType.PlayerB;

        int bestScore = int.MinValue;  // maxValue를 가장 작은 값으로 초기화
        (int row, int col) bestMove = (-1, -1);  // 초기값 변경

        // 가능한 모든 수를 탐색
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i,j] != PlayerType.None) continue;
                
                board[i,j] = startType;
                int score = Minimax(board, false);
                board[i,j] = PlayerType.None;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (i, j);
                }
            }
        }

        return bestMove;
    }

    private static int Minimax(PlayerType[,] board, bool isMaximizing)
    {
        int score = GetScore(board);
        if (score != 0) return score;  // 게임이 끝난 경우
        if (CheckFull(board)) return 0;  // 무승부

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i,j] != PlayerType.None) continue;
                    
                    board[i,j] = startType;
                    bestScore = Mathf.Max(bestScore, Minimax(board, false));
                    board[i,j] = PlayerType.None;
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i,j] != PlayerType.None) continue;
                    
                    board[i,j] = (startType == PlayerType.PlayerA) ? PlayerType.PlayerB : PlayerType.PlayerA;
                    bestScore = Mathf.Min(bestScore, Minimax(board, true));
                    board[i,j] = PlayerType.None;
                }
            }
            return bestScore;
        }
    }

    private static int GetScore(PlayerType[,] board)
    {
        // 승리 체크
        PlayerType winner = CheckWinner(board);
        if (winner == startType) return 1;
        if (winner != PlayerType.None) return -1;
        return 0;
    }

    private static PlayerType CheckWinner(PlayerType[,] board)
    {
        // 가로 검사
        for (int i = 0; i < 3; i++)
        {
            if (board[i,0] != PlayerType.None && 
                board[i,0] == board[i,1] && 
                board[i,1] == board[i,2])
            {
                return board[i,0];
            }
        }

        // 세로 검사
        for (int j = 0; j < 3; j++)
        {
            if (board[0,j] != PlayerType.None && 
                board[0,j] == board[1,j] && 
                board[1,j] == board[2,j])
            {
                return board[0,j];
            }
        }

        // 대각선 검사
        if (board[0,0] != PlayerType.None && 
            board[0,0] == board[1,1] && 
            board[1,1] == board[2,2])
        {
            return board[0,0];
        }

        if (board[0,2] != PlayerType.None && 
            board[0,2] == board[1,1] && 
            board[1,1] == board[2,0])
        {
            return board[0,2];
        }

        return PlayerType.None;
    }

    private static bool CheckFull(PlayerType[,] board)
    {
        foreach (var cell in board)
        {
            if (cell == PlayerType.None) return false;
        }
        return true;
    }
}