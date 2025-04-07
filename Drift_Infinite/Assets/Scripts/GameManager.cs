using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject CancelGameButton;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI PlayersCountText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RaitingController raitingController;
    [SerializeField] private ConnectionsViewController connectionsViewController;
    [SerializeField] private LevelManager LevelManager;
    private float lobbyLifeTineInSeconds = 1800f;

    [Header("RaitingTable")]
    [SerializeField] private Image lobbyTopButtonImage;
    [SerializeField] private Image gameTopButtonImage;

    [Header("Other")]
    [SerializeField] UserProfile userProfile;

    public static GameManager Instance;

    private TimeSpan timer;
    private TimeSpan Timer
    {
        set
        {
            timerText.text = value.ToString();
            timer = value;
        }
        get => timer;
    }

    private List<PlayerInfo> topPlayers = new() { new PlayerInfo() { userName = "1", score = 5}, new PlayerInfo() { userName = "2", score = 125 } };

    private List<DrifterInfo> connections = new();

    public static List<int> GameRewards { get; private set; } = null;

    public List<DrifterInfo> Connections =>
#if UNITY_EDITOR
         new() { new DrifterInfo {userId = 1, userName = "User1", carColor = 0 }, new DrifterInfo { userId = 2, userName = "User2", carColor = 1 }, new DrifterInfo { userId = 2, userName = "User2", carColor = 2 }, new DrifterInfo { userId = 2, userName = "User2", carColor = 3 }, new DrifterInfo { userId = 2, userName = "User2", carColor = 4 } };
#else
        connections.OrderBy(c => c.userId).ToList();
#endif

    public long SelfId { get; private set; } =
#if UNITY_EDITOR
        2;
#else
        0;
#endif
    public int MaxPlayersCount { get; set; }
    public int SelectedLevel { get; private set; }

    public static string gameName => "drift-infinite";

    public static string LobbyId =>
#if !UNITY_EDITOR
         $"{AllGamesServer.Instance.startData?.chatId}_{gameName}";
#else
        "12345_drift-infinite";
#endif
    public static string LobbyGuid = "";
    // Start is called before the first frame update

    public void Awake()
    {
        Instance = Instance != null ? Instance : this;
    }

    void Start()
    {
        InitializeLobbyInfo();
        InitializeGameInfo();
        GetUserBalance();
        startButton.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerController.OnEnterLobbyNotify -= EnterLobbyNotify;
        MultiplayerController.OnGetSelfInfo -= GetSelfInfo;
        MultiplayerController.OnLeaveLobbyNotify -= LeaveLobbyNotify;
        MultiplayerController.OnSessionFull -= SessionFullHandler;
        MultiplayerController.OnQueueCompleted -= QueueCompletedHandler;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame() => MultiplayerController.startGame(LobbyId);

    public void InitializeLobbyInfo()
    {
        AllGamesServer.Instance.GetLobby(gameName, AllGamesServer.Instance.startData?.chatId, lobby =>
        {
            SelectedLevel = lobby.selectedLevel;
            MaxPlayersCount = lobby.maxPlayersCount;
            LobbyGuid = lobby.guid;
            connections = lobby.usersConnections;

            MultiplayerController.OnEnterLobbyNotify += EnterLobbyNotify;
            MultiplayerController.OnGetSelfInfo += GetSelfInfo;
            MultiplayerController.OnLeaveLobbyNotify += LeaveLobbyNotify;
            MultiplayerController.OnSessionFull += SessionFullHandler;
            MultiplayerController.OnQueueCompleted += QueueCompletedHandler;
            MultiplayerController.enterLobby(LobbyId, LevelManager.SelectedCarColor);
            ShowLobbyPlayers();
        },
        () =>
        {
            Debug.Log("Не удалось получить информацию о лобби");
        });
        
    }

    public void InitializeGameInfo()
    {
        AllGamesServer.Instance.GetTopPlayersInfo(gameName, top =>
        {
            topPlayers = top.players;
        },
        () =>
        {
            Debug.Log("Не удалось получить информацию об игре");
        });

        if (GameRewards != null) return;

        AllGamesServer.Instance.GetGameRewards(gameName, gameRewards =>
        {
            GameRewards = gameRewards.reward;
        },
        () =>
        {
            Debug.Log("Не удалось получить информацию о наградах");
        });
    }

    private void GetUserBalance()
    {
        AllGamesServer.Instance.GetBalance(userBalance =>
        {
            userProfile.SetBalance(userBalance.balance);
        },
        () =>
        {
            Debug.Log("Не удалось получить информацию об игре");
        });
    }

    public void ShowLobbyPlayers()
    {
        raitingController.SetActive(false);
        connectionsViewController.SetActive(true);

        lobbyTopButtonImage.color = Color.white;
        gameTopButtonImage.color = new Color32(255, 255, 255, 100);
        connectionsViewController.ShowPlayers(connections, true);
    }

    

    public void ShowGameTop()
    {

        connectionsViewController.SetActive(false);
        raitingController.SetActive(true);

        gameTopButtonImage.color = Color.white;
        lobbyTopButtonImage.color = new Color32(255, 255, 255, 100);
        raitingController.ShowPlayers(topPlayers);
    }



    private IEnumerator TimerUpdater()
    {
        while (Timer > TimeSpan.Zero)
        {
            Timer = timer.Add(TimeSpan.FromSeconds(-1));

            if (Timer <= TimeSpan.FromMinutes(2) && startButton.IsActive())
            {
                startButton.interactable = false;
                startButtonText.color = new Color32(255, 255, 255, 50);
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void EnterLobbyNotify(DrifterInfo drifterInfo) => AddConnection(drifterInfo);

    private void GetSelfInfo(SelfInfo selfInfo)
    {
        if (selfInfo.isLobbyAdmin)
            startButton.SetActive(true);

        SelfId = AllGamesServer.Instance.startData.userId;
        var drifterInfo = new DrifterInfo { userId = SelfId, userName = selfInfo.userName, carColor = LevelManager.SelectedCarColor };
        AddConnection(drifterInfo);
    }

    private void AddConnection(DrifterInfo drifterInfo)
    {
        if (connections.FirstOrDefault(c => c.userId == drifterInfo.userId) != null) return;
        connections.Add(drifterInfo);
        connectionsViewController.AddPlayer(drifterInfo);
        PlayersCountText.text = $"Joined players {connections.Count}/{MaxPlayersCount}";
    }

    private void LeaveLobbyNotify(long id)
    {
        var connection = connections.FirstOrDefault(c => c.userId == id);
        if (connection == null) return;
        connections.Remove(connection);
        connectionsViewController.RemovePlayer(id);
        PlayersCountText.text = $"Joined players {connections.Count}/{MaxPlayersCount}";
    }

    private void SessionFullHandler() => PlayersCountText.text = "Game has already started! Awaiting the next round...";

    private void QueueCompletedHandler()
    {
        AllGamesServer.Instance.GetLobby(gameName, AllGamesServer.Instance.startData?.chatId, lobby =>
        {
            SelectedLevel = lobby.selectedLevel;
            MaxPlayersCount = lobby.maxPlayersCount;
            LobbyGuid = lobby.guid;
            connections = lobby.usersConnections;

            MultiplayerController.enterLobby(LobbyId, LevelManager.SelectedCarColor);
            ShowLobbyPlayers();
        },
        () =>
        {
            Debug.Log("Не удалось получить информацию о лобби");
        });
    }

    public void LeaveLobby(bool alreaddyInGame)
    {
        MultiplayerController.leaveLobby(LobbyId);

        if (alreaddyInGame)
        {
            LevelManager.StopQueue();
            startButton.SetActive(false);
        }

        var connection = connections.FirstOrDefault(c => c.userId == SelfId);
        if (connection == null) return;

        connections.Remove(connection);
        connectionsViewController.RemovePlayer(SelfId);
        CancelGameButton.SetActive(false);
        PlayersCountText.text = $"You are disconnected";
    }

}

