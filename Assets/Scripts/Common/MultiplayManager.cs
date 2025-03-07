using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

public class MultiplayManager: IDisposable
{
    private SocketIOUnity _socket;
    
    private event Action<Constants.MultiplayManagerState, string> _onMulttplayStateChanged;
    private event Action<MessageData> _onReceivedMessage;
    private event Action<PlaceMakerData> _onPlacedMaker;
    private event Action _onChangeTurn;

    public MultiplayManager(Action<Constants.MultiplayManagerState, string> sessionCallback, Action<MessageData> messageCallback, Action<PlaceMakerData> placeMakerCallback, Action changeTurnCallback)
    {
        _onMulttplayStateChanged = sessionCallback;
        _onReceivedMessage = messageCallback;
        _onPlacedMaker = placeMakerCallback;
        _onChangeTurn = changeTurnCallback;

        var uri = new Uri(Constants.GameServerURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        
        _socket.On("createRoom", CreateRoom);
        _socket.On("joinRoom", JoinRoom);
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
        _onMulttplayStateChanged?.Invoke(Constants.MultiplayManagerState.CreateRoom, data.roomId);
    }
    
    private void JoinRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        Debug.Log($"Join Room: {data.roomId}");
        _onMulttplayStateChanged?.Invoke(Constants.MultiplayManagerState.JoinRoom, data.roomId);
    }
    
    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<UserData>();
        _onMulttplayStateChanged?.Invoke(Constants.MultiplayManagerState.StartGame, data.userId);
    }
    
    private void GameEnded(SocketIOResponse response)
    {
        var data = response.GetValue<UserData>();
        _onMulttplayStateChanged?.Invoke(Constants.MultiplayManagerState.EndGame, data.userId);
    }
    
    private void ReceiveMessage(SocketIOResponse response)
    {
        Debug.Log("ReceiveMessage");
        var data = response.GetValue<MessageData>();
        _onReceivedMessage?.Invoke(data);
    }

    public void SendMessage(string roomId, string nickName, string message)
    {
        _socket.Emit("sendMessage", new{ roomId, nickName, message });
    }

    public void SendStartGame(string roomId)
    {
        _socket.Emit("startGame", roomId);
    }

    public void Dispose()
    {
        if (_socket != null)
        {
            _socket.Disconnect();
            _socket.Dispose();
            _socket = null;
        }
    }

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
}
