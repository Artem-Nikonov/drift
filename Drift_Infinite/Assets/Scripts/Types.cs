using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class Lobby
{
    public long chatId;
    public string gameName;
    public string guid;
    public List<PlayerInfo> players;
    public int remainingTime;
    public int lifeTime;
    public string Id => $"{chatId}_{gameName}";
    public TimeSpan RemainingTimeSpan => TimeSpan.FromSeconds(remainingTime);
}

[Serializable]
public class GameTop
{
    public List<PlayerInfo> players;
}

[Serializable]
public class PlayerInfo
{
    public string userName;
    public int score;
}

[Serializable]
public struct GameResult
{
    public int score;
    public string messageId;
    public string key;
}