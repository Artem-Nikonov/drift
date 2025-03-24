using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MultiplayerController : MonoBehaviour
{

    public void Start()
    {
        //Debug.LogError("Start MultiplayerController");
    }
    public static event Action<string, bool> OnEnterLobbyNotify;

    public static event Action<CarTransformInfo> OnCarTransformReciecved;

    public static event Action<string> OnPlayerHasCompletedRace;


    public static event Action OnStartGame;

    [DllImport("__Internal")]
    public static extern void enterLobby(string lobbyId);

    [DllImport("__Internal")]
    public static extern void sendCarTransform(string lobbyId, string carTransformJSON);

    [DllImport("__Internal")]
    public static extern void sendGameResult(string lobbyId, string gameResultJSON);


    public void EnterLobbyNotify(string userId) => OnEnterLobbyNotify?.Invoke(userId, false);

    public void GetSelfId(string userId) => OnEnterLobbyNotify?.Invoke(userId, true);

    public void StartGame() => OnStartGame?.Invoke();

    public void PlayerHasCompletedRace(string userId) => OnPlayerHasCompletedRace?.Invoke(userId);

    public void CarTransformReciecved(string carTransformJSON)
    {
        var carPositionInfo = JsonUtility.FromJson<CarTransformInfo>(carTransformJSON);
        OnCarTransformReciecved?.Invoke(carPositionInfo);
    }

}

[Serializable]
public class CarTransformInfo
{
    public string connectionId;
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


