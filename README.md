# TicTacToe

<br>

## 📝 프로젝트 소개

이 프로젝트는 Unity로 개발된 멀티플레이 틱택토 게임입니다. 실시간 온라인 대전과 AI와의 대결을 지원하는 기능이 포함되어 있으며, 사용자 계정 시스템과 리더보드를 통해 게임 기록을 관리할 수 있습니다.

<br>

## 🛠️ 주요 기능 및 구현 사항

### 1. 회원가입 및 로그인 시스템

- Node.js와 Express를 활용한 로그인 서버 구현
- Unity 클라이언트에서는 UnityWebRequest를 사용해 HTTP 통신 기반의 회원가입 및 로그인 기능 구현
- 서버로부터 받은 쿠키를 파싱하여 PlayerPrefs에 저장함으로써 자동 로그인 기능도 지원

```csharp
// 로그인 처리 예시
public IEnumerator Signin(SigninData signinData, Action successCallback, Action failureCallback)
{
    string jsonString = JsonUtility.ToJson(signinData);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);

    using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/signin", UnityWebRequest.kHttpVerbPOST))
    {
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();

        Debug.Log($"Response Code: {www.responseCode}");
        Debug.Log($"Response Body: {www.downloadHandler.text}");
        
        // 모든 응답 헤더 로깅
        var allHeaders = www.GetResponseHeaders();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            failureCallback?.Invoke();
        }
        else
        {
            string cookieHeader = null;
            foreach (var header in allHeaders)
            {
                if (header.Key.ToLower() == "set-cookie")
                {
                    cookieHeader = header.Value;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(cookieHeader))
            {
                string[] cookieParts = cookieHeader.Split(';');
                string sessionCookie = cookieParts[0]; // 세션 ID
                
                PlayerPrefs.SetString("sid", sessionCookie);
                PlayerPrefs.Save(); // 명시적 저장
            }
            else
            {
                Debug.LogWarning("No cookie found in response headers.");
            }
            
            var result = JsonUtility.FromJson<SigninResult>(www.downloadHandler.text);
            
            if (result.result == 2)
            {
                successCallback?.Invoke();
            }
            else
            {
                failureCallback?.Invoke();
            }
        }
    }
}
```

<br>

### 2. 리더보드 시스템

- 서버로부터 유저 정보를 받아와 점수 기준으로 내림차순 정렬하여 표시
- Unity의 스크롤뷰를 이용하여 모든 사용자의 등수, 이름, 점수를 표시하는 UI 구현
- 상단에 자신의 점수를 표시하여 사용자가 편리하게 확인할 수 있도록 설계

```csharp
// 리더보드 데이터 로드 함수
public IEnumerator GetLeaderboard(Action<List<UserInfo>> callback = null)
{
    using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "users/leaderboard", UnityWebRequest.kHttpVerbGET))
    {
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            yield break;
        }
        
        var result = www.downloadHandler.text;
        Debug.Log("Leaderboard response: " + result);
    
        try {
            // JSON 배열을 객체로 감싸기
            string wrappedJson = "{\"leaderboard\":" + result + "}";
            var wrapper = JsonUtility.FromJson<LeaderboardResult>(wrappedJson);
            callback?.Invoke(wrapper.leaderboard);
        }
        catch (Exception e) {
            Debug.LogError("Error parsing leaderboard: " + e.Message);
            callback?.Invoke(null);
        }
    }
}
```

<br>

### 3. 실시간 턴제 게임 플레이

#### 로비 시스템

- Socket.IO를 활용한 실시간 로비 시스템 구현
- 기존 세션 확인 및 자동 재연결 기능 지원
- 대기 중인 상대방과 자동 매칭 또는 새로운 게임방 생성

```csharp
// 소켓 연결 및 로비 접속
public MultiplayerManager(Action<Constants.MultiplayerManagerState, RoomData> roomDataCallback,
        Action<Constants.MultiplayerManagerState, UserData> userDataCallback, 
        Action<MessageData> messageCallback, 
        Action<PlaceMakerData> placeMakerCallback, 
        Action changeTurnCallback)
{
    _onReceivedRoomData = roomDataCallback;
    _onReceivedUserData = userDataCallback;
    _onReceivedMessage = messageCallback;
    _onPlacedMaker = placeMakerCallback;
    _onChangeTurn = changeTurnCallback;

    var uri = new Uri(Constants.GameServerURL);
    _socket = new SocketIOUnity(uri, new SocketIOOptions
    {
        Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
        Query = new Dictionary<string, string>
        {
            { "nickName", GameManager.Instance.UserInfo.nickname }
        }
    });
    
    _socket.On("createRoom", CreateRoom);
    _socket.On("joinRoom", JoinRoom);
    _socket.On("userJoined", UserJoined);
    _socket.On("startGame", StartGame);
    _socket.On("gameEnded", GameEnded);
    _socket.On("receiveMessage", ReceiveMessage);
    _socket.On("placeMaker", PlaceMaker);
    _socket.On("changeTurn", ChangeTurn);
    
    _socket.Connect();
}

private void CreateRoom(SocketIOResponse response)
{
    var data = response.GetValue<RoomData>();
    Debug.Log($"Create Room: {data.roomId}");
    _onReceivedRoomData?.Invoke(Constants.MultiplayerManagerState.CreateRoom, data);
}

private void JoinRoom(SocketIOResponse response)
{
    var data = response.GetValue<RoomData>();
    Debug.Log($"Join Room: {data.roomId}");
    _onReceivedRoomData?.Invoke(Constants.MultiplayerManagerState.JoinRoom, data);
}

private void UserJoined(SocketIOResponse response)
{
    var data = response.GetValue<UserData>();
    Debug.Log($"User Joined: {data.userId}");
    _onReceivedUserData?.Invoke(Constants.MultiplayerManagerState.UserJoined, data);
}
```

<br>

#### 인게임 시스템

- 실시간 게임 상태 동기화 및 턴 관리
- 게임 보드 UI 상호작용 및 승패 판정 로직 구현
- 게임 종료 시 결과 처리 및 점수 업데이트

```csharp
// 게임 보드 셀 클릭 이벤트 처리
public void SendPlaceMaker(string roomId, PlayerType currentTurn, int index)
{
    var makerType = (int)currentTurn;
    _socket.Emit("placeMaker", new{ roomId, makerType, index });
}

public void SendChangeTurn(string roomId)
{
    _socket.Emit("changeTurn", new{ roomId });
}

private void PlaceMaker(SocketIOResponse response)
{
    var data = response.GetValue<PlaceMakerData>();
    Debug.Log($"Maker: {data.makerType}, Index: {data.index}");
    _onPlacedMaker?.Invoke(data);
}

private void ChangeTurn(SocketIOResponse response)
{
    _onChangeTurn?.Invoke();
}
```

<br>

### 4. AI 대전 모드

- MiniMax 알고리즘을 활용한 최적의 AI 이동 전략 구현
- 난이도 설정에 따른 AI 알고리즘 깊이 조절
- 싱글 플레이어 모드에서 다양한 AI 난이도 지원

```csharp
// AI 이동 계산 함수
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
```

<br>

## 👥 개발 정보

- 개발 인원: 1명
- 개발 기간: 2025.02.03 ~ 2025.03.07
- 사용 엔진: Unity
- GitHub: [minhvvan/TicTacToe](https://github.com/minhvvan/TicTacToe)

<br>

## 📚 기술 스택

- **클라이언트**
  - Unity (게임 엔진)
  - C# (프로그래밍 언어)
  - Socket.IO Client (실시간 통신)

- **서버**
  - Node.js (백엔드 플랫폼)
  - Express (웹 프레임워크)
  - Socket.IO (실시간 통신 라이브러리)
  - MongoDB (데이터베이스)

<br>

## 🙏 감사의 말

- Socket.IO - https://socket.io/
- Node.js - https://nodejs.org/
- Unity - https://unity.com/
