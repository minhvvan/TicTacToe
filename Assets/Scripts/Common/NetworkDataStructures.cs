using System;
using System.Collections.Generic;
using Newtonsoft.Json;

// NetworkDataStructures.cs

[Serializable]
public struct SigninData
{
    public string username;
    public string password;
}

[Serializable]
public struct SignupData
{
    public string username;
    public string password;
    public string nickname;
}

[Serializable]
public struct SigninResult
{
    public int result;
}

[Serializable]
public struct UserInfo
{
    public string id;
    public string username;
    public string nickname;
    public int score; // 'result'에서 'score'로 변경 - 서버 응답과 일치하도록
}

[Serializable]
public struct ScoreData
{
    public int score;
}

[Serializable]
public struct LeaderboardResult
{
    public List<UserInfo> leaderboard;
}

public class RoomData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class UserData
{
    [JsonProperty("userId")]
    public string userId { get; set; }
}

public class MessageData
{
    [JsonProperty("nickName")]
    public string nickName { get; set; }
    
    [JsonProperty("message")]
    public string message { get; set; }
}

public class PlaceMakerData
{
    [JsonProperty("makerType")]
    public int makerType { get; set; }    
    
    [JsonProperty("index")]
    public int index { get; set; }
}