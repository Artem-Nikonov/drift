using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class MultiplayerController : MonoBehaviour
{
    public static event Action<string> OnEnterLobbyNotify;

    public static event Action<CarTransformInfo> OnCarTransformReciecved;

    [DllImport("__Internal")]
    public static extern void enterLobby(string lobbyId);

    [DllImport("__Internal")]
    public static extern void sendCarTransform(string lobbyId, string carTransformJSON);


    public void EnterLobbyNotify(string message)
    {
        OnEnterLobbyNotify?.Invoke(message);
    }

    public void CarPositionReciecved(string carTransformJSON)
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


