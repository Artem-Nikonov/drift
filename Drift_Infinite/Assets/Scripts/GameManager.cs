using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RaitingController raitingController;
    private float lobbyLifeTineInSeconds = 1800f;

    [Header("RaitingTable")]
    [SerializeField] private Image lobbyTopButtonImage;
    [SerializeField] private Image gameTopButtonImage;

    [SerializeField] private TextMeshProUGUI signalRTestOutput;
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

    private List<PlayerInfo> topPlayers = new();
    private List<PlayerInfo> lobbyPlayers = new();

    public static string gameName => "drift-infinite";

    public static string LobbyId =>
#if !UNITY_EDITOR
         $"{/*AllGamesServer.Instance.startData?.chatId ?? ""*/ "12345"}_{gameName}";
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeLobbyInfo()
    {
        AllGamesServer.Instance.GetLobby(gameName, /*AllGamesServer.Instance.startData?.chatId*/ "12345", lobby =>
        {
            LobbyGuid = lobby.guid;
            lobbyPlayers = lobby.players;
            lobbyLifeTineInSeconds = lobby.lifeTime;
            Timer = lobby.RemainingTimeSpan;
            StartCoroutine(TimerUpdater());
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
            ShowGameTop();
        },
        () =>
        {

            Debug.Log("Не удалось получить информацию об игре");
        });
    }

    public void ShowLobbyTop()
    {
        lobbyTopButtonImage.color = Color.white;
        gameTopButtonImage.color = new Color32(255, 255, 255, 100);
        raitingController.ShowPlayers(lobbyPlayers);
        MultiplayerController.OnEnterLobbyNotify += (msg) =>
        {
            
        };
        MultiplayerController.enterLobby(LobbyId);
    }

    public void ShowGameTop()
    {
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

    public void StartSendCarTransform(Transform carTransform)
    {
        StartCoroutine(SendCarTransform(carTransform));

    }

    public void StopSendCarTransform()
    {
        StopAllCoroutines();
    }

    private IEnumerator SendCarTransform(Transform carTransform)
    {
        while (carTransform != null)
        {
            var data = new CarTransformInfo
            {
                connectionId = "777",
                position = new Position
                {
                    x = carTransform.position.x,
                    y = carTransform.position.y,
                    z = carTransform.position.z,
                },
                rotation = new Rotation
                {
                    x = carTransform.rotation.x,
                    y = carTransform.rotation.y,
                    z = carTransform.rotation.z,
                    w = carTransform.rotation.w
                }
            };
            MultiplayerController.sendCarTransform(LobbyId, JsonUtility.ToJson(data));

            yield return new WaitForSeconds(0.2f);
        }
    }
}
