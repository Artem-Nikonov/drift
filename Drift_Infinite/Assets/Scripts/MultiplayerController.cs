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
    public static event Action<DrifterInfo> OnEnterLobbyNotify;

    public static event Action<SelfInfo> OnGetSelfInfo;

    public static event Action<long> OnLeaveLobbyNotify;

    public static event Action<CarTransformInfo> OnCarTransformReciecved;

    public static event Action<long> OnPlayerHasCompletedRace;

    public static event Action OnStartGame;

    public static event Action<GameTop> OnGameOver;

    public static event Action OnSessionFull;

    public static event Action OnQueueCompleted;


    [DllImport("__Internal")]
    public static extern void connectToHub();

    [DllImport("__Internal")]
    public static extern void enterLobby(string lobbyId, int carColor);

    [DllImport("__Internal")]
    public static extern void startGame(string lobbyId);

    [DllImport("__Internal")]
    public static extern void leaveLobby(string lobbyId);

    [DllImport("__Internal")]
    public static extern void sendCarTransform(string lobbyId, string carTransformJSON);

    [DllImport("__Internal")]
    public static extern void sendGameResult(string lobbyId, string gameResultJSON);


    public void EnterLobbyNotify(string drifterInfoJSON)
    {
        var enemyInfo = JsonUtility.FromJson<DrifterInfo>(drifterInfoJSON);
        OnEnterLobbyNotify?.Invoke(enemyInfo);
    }

    public void GetSelfInfo(string selfInfoJSON)
    {
        var selfInfo = JsonUtility.FromJson<SelfInfo>(selfInfoJSON);
        OnGetSelfInfo?.Invoke(selfInfo);
    }

    public void LeaveLobbyNotify(string userId)
    {
        if(long.TryParse(userId, out var id))
        {
            OnLeaveLobbyNotify?.Invoke(id);
        }
    }

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

    public void StartGame() => OnStartGame?.Invoke();

    public void SessionFull() => OnSessionFull?.Invoke();

    public void QueueCompleted() => OnQueueCompleted?.Invoke();

}


[Serializable]
public class SelfInfo
{
    public string userName;
    public bool isLobbyAdmin;
}

[Serializable]
public class DrifterInfo
{
    public long userId;
    public string userName;
    public int carColor;
}

[Serializable]
public class CarTransformInfo
{
    public long userId;
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


