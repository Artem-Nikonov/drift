using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;

[Serializable]
public class Lobby
{
    public long chatId;
    public string gameName;
    public string guid;
    public List<PlayerInfo> players;
    public int selectedLevel;
    public List<DrifterInfo> usersConnections;
    public int maxPlayersCount;
    //public int remainingTime;
    //public int lifeTime;
    public string Id => $"{chatId}_{gameName}";
    //public TimeSpan RemainingTimeSpan => TimeSpan.FromSeconds(remainingTime);
}

[Serializable]
public class GameTop
{
    public List<PlayerInfo> players;
}


[Serializable]
public class GameRewards
{
    public List<int> reward;
}
[Serializable]
public class UserBalance
{
    public int balance;
}

[Serializable]
public class PlayerInfo
{
    public long userId;
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