using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    [Header("Level Prefabs")]
    public List<GameObject> levelPrefabs; // List of level prefabs

    [Header("Skybox Colors")]
    public List<Color> skyboxColors; // List of skybox colors for each level

    private int selectedLevelIndex = -1; // Stores selected level index

    public Animator animator; // Reference to animator

    [Header("UI")] 
    [SerializeField] private GameObject results;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject upperMenu;
    public TextMeshProUGUI finalScore;
    
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimedButton;
    [SerializeField] private Button startButton;

    [SerializeField] private Image timerMenuSlider; 
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private RaitingController raitingController;
    public float gameTimeInSeconds => 1800f;
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
    public int maxScore = 0; 

    public static LevelManager Instance { get; private set; }
    public static string gameName => "drift-infinite";

    public static string LobbyId =>
#if !UNITY_EDITOR
         $"{AllGamesServer.Instance.startData?.chatId ?? ""}_{gameName}";
#else
        "12345_drift-infinite";
#endif

    [Header("Car Colors")]
    public List<Color> carColors = new List<Color> { Color.green, Color.yellow, Color.red, Color.blue, new Color(1.0f, 0.5f, 0.0f), Color.magenta }; // Orange & Pink

    private GameObject spawnedCar; // Reference to the spawned car

    private void Awake()
    {
        Instance ??= this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        InitializeLobbyInfo();
    }

    public void SelectRandomLevel()
    {
        if (levelPrefabs.Count == 0)
        {
            Debug.LogWarning("LevelManager: No level prefabs assigned!");
            return;
        }

        // Select a random level but do not activate it yet
        selectedLevelIndex = UnityEngine.Random.Range(0, levelPrefabs.Count);

        // Set the correct animation bool based on selected level
        if (animator != null)
        {
            animator.SetBool("Level1", selectedLevelIndex == 0);
            animator.SetBool("Level2", selectedLevelIndex == 1);
            animator.SetBool("Level3", selectedLevelIndex == 2);
        }
        else
        {
            Debug.LogWarning("LevelManager: Animator not assigned!");
        }

        Debug.Log("Selected Level: " + levelPrefabs[selectedLevelIndex].name);
    }

    public void ActivateLevel()
    {
        if (selectedLevelIndex == -1) return; // No level selected

        // Disable all levels
        foreach (GameObject level in levelPrefabs)
        {
            level.SetActive(false);
        }

        // Activate selected level
        GameObject selectedLevel = levelPrefabs[selectedLevelIndex];
        selectedLevel.SetActive(true);

        // Change the skybox color
        if (selectedLevelIndex < skyboxColors.Count)
        {
            RenderSettings.skybox.SetColor("_Tint", skyboxColors[selectedLevelIndex]);
        }

        if (RenderSettings.skybox.HasProperty("_Exposure"))
        {
            RenderSettings.skybox.SetFloat("_Exposure", 0.5f);
        }
        else
        {
            Debug.LogWarning("No skybox color assigned for level index: " + selectedLevelIndex);
        }

        // Spawn the car in the level
        SpawnCar(selectedLevel);

        Debug.Log("Activated Level: " + selectedLevel.name);
    }

    private void SpawnCar(GameObject level)
    {
        Level levelData = level.GetComponent<Level>();
        if (levelData == null)
        {
            Debug.LogError("LevelManager: LevelData component is missing on " + level.name);
            return;
        }

        // Get a random spawn point
        Transform spawnPoint = levelData.GetRandomSpawnPoint();
        if (spawnPoint == null) return;

        // Spawn the car
        spawnedCar = Instantiate(levelData.carPrefab, spawnPoint.position, spawnPoint.rotation);

        // Assign a random color
        Renderer carRenderer = spawnedCar.GetComponentInChildren<Renderer>(); // Assuming the car has a Renderer
        if (carRenderer != null)
        {
            carRenderer.material.color = carColors[UnityEngine.Random.Range(0, carColors.Count)];
        }
        else
        {
            Debug.LogWarning("Spawned car does not have a Renderer!");
        }

        // Find and assign CinemachineVirtualCamera
        CinemachineVirtualCamera cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (cinemachineCam != null)
        {
            cinemachineCam.Follow = spawnedCar.transform;
            cinemachineCam.LookAt = spawnedCar.transform;
        }
        else
        {
            Debug.LogWarning("CinemachineVirtualCamera not found in the scene!");
        }
    }

    public void FinishRace()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void CloseResults()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void InitializeLobbyInfo()
    {
        AllGamesServer.Instance.GetLobby(gameName, AllGamesServer.Instance.startData?.chatId, lobby =>
        {
            lobbyPlayers = lobby.players;
            //Debug.Log(timer.ToString());
            Timer = lobby.RemainingTimeSpan;
            StartCoroutine(TimerUpdater());
            //InitializeMenuTimer();
            //Debug.Log(JsonSerializer.Serialize(lobby));
            string json = JsonUtility.ToJson(lobby);
            Debug.Log(json);
            raitingController.ShowLobbyPlayers(lobbyPlayers);

        },
        () =>
        {
            //playButton.gameObject.SetActive(false);
            //lobbyInfo.text = "Не удалось получить информацию о лобби";
            Debug.Log("Не удалось получить информацию о лобби");
        });
    }

    public void InitializeGameInfo()
    {
        AllGamesServer.Instance.GetTopPlayersInfo(gameName, top =>
        {
            topPlayers = top.players;
            //ShowTopPlayers();
            string json = JsonUtility.ToJson(top);
            Debug.Log(json);
        },
        () =>
        {
            
            //playButton.gameObject.SetActive(false);
            //lobbyInfo.text = "Не удалось получить информацию об игре";
            Debug.Log("Не удалось получить информацию об игре");
        });
    }

    public void StopQueue()
    {
        loadingScreen.SetActive(false);
        startScreen.SetActive(true);
        upperMenu.SetActive(true);
    }
    
    public void StartQueue()
    {
        loadingScreen.SetActive(true);
        SelectRandomLevel();
        startScreen.SetActive(false);
    }
    
    public void ClaimRewards()
    {
        claimButton.SetActive(false);
        claimedButton.SetActive(true);
    }

    private IEnumerator TimerUpdater()
    {
        while (Timer > TimeSpan.Zero)
        {
            Timer = timer.Add(TimeSpan.FromSeconds(-1));

            if (timerMenuSlider != null)
            {
                timerMenuSlider.fillAmount = (float)timer.TotalSeconds/gameTimeInSeconds;
            }

            if (Timer <= TimeSpan.FromMinutes(2) && startButton.IsActive())
            {
                startButton.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1);
        }
    }


}
