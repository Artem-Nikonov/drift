using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Button playAgainButton;
    [SerializeField] private RaitingController raitingController;


    [Header("Car Colors")]
    public List<Color> carColors = new List<Color> { Color.green, Color.yellow, Color.red, Color.blue, new Color(1.0f, 0.5f, 0.0f), Color.magenta }; // Orange & Pink
    private int selectedCarColor = -1;

    [Header("Other")]
    [SerializeField] private CarTransformSender carTransformSender;
    public int SelectedCarColor
    {
        get
        {
            if (selectedCarColor == -1)
                selectedCarColor = UnityEngine.Random.Range(0, carColors.Count);
            return selectedCarColor;
        }
    }

    private GameObject spawnedCar; // Reference to the spawned car
    public int maxScore = 0;


    [SerializeField] private EnemyCar EnemyCarPrefab;
    private Dictionary<long, EnemyCar> EnemyCars = new();

    private void Start()
    {
        MultiplayerController.OnStartGame += StartQueue;
        MultiplayerController.OnPlayerHasCompletedRace += DestroyEnemyCar;
        MultiplayerController.OnLeaveLobbyNotify += DestroyEnemyCar;
        MultiplayerController.OnGameOver += GameOverHandler;

        SetPlayAgainButtonState(false);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        MultiplayerController.OnCarTransformReciecved -= UpdateEnemyCarTransform;
        MultiplayerController.OnStartGame -= StartQueue;
        MultiplayerController.OnPlayerHasCompletedRace -= DestroyEnemyCar;
        MultiplayerController.OnGameOver -= GameOverHandler;
        MultiplayerController.OnLeaveLobbyNotify -= DestroyEnemyCar;
    }

    public void SelectRandomLevel()
    {
        if (levelPrefabs.Count == 0)
        {
            Debug.LogWarning("LevelManager: No level prefabs assigned!");
            return;
        }

        // Select a random level but do not activate it yet
        var levelIndex = GameManager.Instance.SelectedLevel;
        selectedLevelIndex = levelIndex < levelPrefabs.Count ? levelIndex : 0;

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
        SpawnCars(selectedLevel);

        Debug.Log("Activated Level: " + selectedLevel.name);
    }

    private void SpawnCars(GameObject level)
    {
        Level levelData = level.GetComponent<Level>();
        if (levelData == null)
        {
            Debug.LogError("LevelManager: LevelData component is missing on " + level.name);
            return;
        }

        var connections = GameManager.Instance.Connections;



        var drifterInfo = connections.FirstOrDefault(c => c.userId == GameManager.Instance.SelfId);
        if (drifterInfo == null) return;

        var spawnPointIndex = connections.IndexOf(drifterInfo);
        if (connections.Count > levelData.spawnPoints.Count) return;

        spawnedCar = Instantiate(levelData.carPrefab, levelData.spawnPoints[spawnPointIndex].position, levelData.spawnPoints[spawnPointIndex].rotation);

        for (int i = 0; i < connections.Count; i++)
        {
            if (i == spawnPointIndex) continue;
            var enemyCar = Instantiate(EnemyCarPrefab, levelData.spawnPoints[i].position, levelData.spawnPoints[i].rotation);

            enemyCar.Renderer.material.color = carColors[connections[i].carColor];
            enemyCar.UserName.text = connections[i].userName;

            EnemyCars.TryAdd(connections[i].userId, enemyCar);
        }

        MultiplayerController.OnCarTransformReciecved += UpdateEnemyCarTransform;
        carTransformSender.StartSendCarTransform(spawnedCar.transform);

        // Assign a random color
        Renderer carRenderer = spawnedCar.GetComponentInChildren<Renderer>(); // Assuming the car has a Renderer
        if (carRenderer != null)
        {
            carRenderer.material.color = carColors[SelectedCarColor];
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
        Debug.Log("FinishRace");
        carTransformSender.StopSendCarTransform();
        results.SetActive(true);
        upperMenu.SetActive(true);
    }

    public void CloseResults()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void StopQueue()
    {
        loadingScreen.SetActive(false);
        startScreen.SetActive(true);
        upperMenu.SetActive(true);
    }
    
    public void StartQueue()
    {
        startButton.SetActive(false);
        loadingScreen.SetActive(true);
        SelectRandomLevel();
        startScreen.SetActive(false);
    }
    
    public void ClaimRewards()
    {
        claimButton.SetActive(false);
        claimedButton.SetActive(true);
    }

    private void UpdateEnemyCarTransform(CarTransformInfo carTransform)
    {
        if (EnemyCars.TryGetValue(carTransform.userId, out var car))
        {
            car.UpdateCarTransform(carTransform);
        }
    }

    private void DestroyEnemyCar(long userId)
    {
        if(EnemyCars.TryGetValue(userId, out var car) && car.gameObject != null)
        {
            Destroy(car.gameObject);
            EnemyCars.Remove(userId);
        }
    }

    private void GameOverHandler(GameTop top)
    {
        raitingController.ShowPlayers(top.players);
        SetPlayAgainButtonState(true);
        
    }

    private void SetPlayAgainButtonState(bool setInteractable)
    {
        if (setInteractable)
        {
            playAgainButton.interactable = true;
            playAgainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
        }
        else
        {
            playAgainButton.interactable = false;
            playAgainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for the others...";
        }

    }

}
