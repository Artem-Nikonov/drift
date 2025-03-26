using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MultiplayerController : MonoBehaviour
{

    public void Start()
    {
    }
    public static event Action<long, bool> OnEnterLobbyNotify;

    public static event Action<long> OnLeaveLobbyNotify;

    public static event Action<CarTransformInfo> OnCarTransformReciecved;

    public static event Action<long> OnPlayerHasCompletedRace;

    public static event Action OnStartGame;

    public static event Action<GameTop> OnGameOver;

    public static event Action OnSessionFull;

    [DllImport("__Internal")]
    public static extern void connectToHub();

    [DllImport("__Internal")]
    public static extern void enterLobby(string lobbyId);

    [DllImport("__Internal")]
    public static extern void sendCarTransform(string lobbyId, string carTransformJSON);

    [DllImport("__Internal")]
    public static extern void sendGameResult(string lobbyId, string gameResultJSON);


    public void EnterLobbyNotify(string userId)
    {
        if(long.TryParse(userId, out var id))
        {
            OnEnterLobbyNotify?.Invoke(id, false);
        }
    }

    public void GetSelfId(string userId)
    {
        if(long.TryParse(userId, out var id))
        {
            OnEnterLobbyNotify?.Invoke(id, true);
        }
    }

    public void LeaveLobbyNotify(string userId)
    {
        if(long.TryParse(userId, out var id))
        {
            OnLeaveLobbyNotify?.Invoke(id);
        }
    }

    public void StartGame() => OnStartGame?.Invoke();

    public void PlayerHasCompletedRace(string userId)
    {
        if(long.TryParse(userId, out var id))
        {
            OnPlayerHasCompletedRace?.Invoke(id);
        }

    }

    public void CarTransformReciecved(string carTransformJSON)
    {
        var carPositionInfo = JsonUtility.FromJson<CarTransformInfo>(carTransformJSON);
        OnCarTransformReciecved?.Invoke(carPositionInfo);
    }

    public void GameOver(string playersJSON)
    {
        var top = JsonUtility.FromJson<GameTop>(playersJSON);
        OnGameOver?.Invoke(top);
    }

    public void SessionFull() => OnSessionFull?.Invoke();

}

[Serializable]
public class CarTransformInfo
{
    public long connectionId;
    public Position position;
    public Rotation rotation;
}

[Serializable]
public struct Position
{
    public float x;
    public float y;
    public float z;
}


[Serializable]
public struct Rotation
{
    public float x;
    public float y;
    public float z;
    public float w;
}


