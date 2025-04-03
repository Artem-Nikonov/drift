using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class DriftSystem : MonoBehaviour
{
    [Header("Drift Points System")]
    [SerializeField] private CarController carController; // Reference to the car controller script
    public bool useDriftPoints = true; // Enable/disable drift points system
    public int driftPoints = 0; // Current drift points
    public float pointsMultiplier = 1f; // Base multiplier
    public int driftMultiplier = 1; // Current drift multiplier

    public GameObject driftEffectPrefab; // Particle effect prefab

    private float driftTimer = 0f; // Timer to track how long the car has been drifting
    private float noDriftTimer = 0f; // Timer to track how long the car has stopped drifting

    [SerializeField] private Explosion explosion;

    public bool isDrifting;
    private bool isInDriftBoostZone = false; // NEW: Track if inside boost zone

    [Header("Drift UI Elements")]
    public Slider driftProgressSlider; // NEW: Slider to show progress towards next multiplier
    public TextMeshProUGUI driftPointsText; // UI text to display drift points
    public TextMeshProUGUI driftMultiplierText; // UI text to display drift multiplier
    public TextMeshProUGUI boostedText;
    public GameObject gameplayCanvas;
    
    [SerializeField] private LevelManager levelManager;
    
    [SerializeField] private bool isFinished = false;

    private int maxPointsPerDrift => 2000;
    private int currentDriftPoints = 0;
    private int maxDriftMultiplier = 10;

    private void Start()
    {
        if (driftProgressSlider != null)
        {
            driftProgressSlider.value = 0f;
        }

        levelManager = FindAnyObjectByType<LevelManager>();
        isFinished = false;
    }

    private void Update()
    {
        if (useDriftPoints && carController != null)
        {
            UpdateDriftPoints();
        }
    }


    private void UpdateDriftPoints()
    {
        if (isDrifting)
        {
            noDriftTimer = 0f; // Сбрасываем таймер отсутствия дрифта

            if (currentDriftPoints < maxPointsPerDrift)
            {
                // Вычисляем очки на основе времени
                float pointsToAdd = driftMultiplier * pointsMultiplier * Time.deltaTime * 10f;
                if (isInDriftBoostZone)
                {
                    pointsToAdd *= 2; // Удваиваем очки в зоне усиления дрифта
                }

                driftPoints += Mathf.RoundToInt(pointsToAdd);
                currentDriftPoints += Mathf.RoundToInt(pointsToAdd);
                UpdateDriftPointsUI();
            }

            // Продолжаем обновлять таймер дрифта и множитель, даже если очки больше не начисляются
            driftTimer += Time.deltaTime;

            float requiredDriftTime = 3f + (driftMultiplier - 3); // Чем выше множитель, тем больше времени требуется

            // Обновляем заполнение слайдера
            if (driftProgressSlider != null)
            {
                driftProgressSlider.value = driftTimer / requiredDriftTime;
            }

            // Продолжаем увеличивать множитель, пока не достигнут максимум
            if (driftTimer >= requiredDriftTime && driftMultiplier < maxDriftMultiplier)
            {
                driftMultiplier++;
                Instantiate(driftEffectPrefab, transform.position, Quaternion.identity);
                explosion.TriggerExplosion();
                driftTimer = 0f;

                if (driftProgressSlider != null)
                {
                    driftProgressSlider.value = 0f;
                }

                UpdateDriftMultiplierUI();
                carController.MaxMotorTorque += 100;
                carController.CarConfig.MaxRPM += 500;
                carController.CarConfig.MinRPM += 50;
                carController.CarConfig.RpmToNextGear += 500;
                carController.CarConfig.RpmToPrevGear += 500;
            }
        }
        else
        {
            // Если дрифт прекратился, сбрасываем таймеры и текущие очки за дрифт
            driftTimer = 0f;
            currentDriftPoints = 0; // Сбрасываем текущие очки за дрифт
            noDriftTimer += Time.deltaTime;

            // Динамически уменьшаем время перед потерей множителя
            float decayTime = Mathf.Max(2f - ((driftMultiplier - 2) * 0.7f), 0.7f); // Чем выше множитель, тем быстрее он уменьшается

            if (noDriftTimer >= decayTime && driftMultiplier > 1)
            {
                driftMultiplier--; // Уменьшаем множитель
                noDriftTimer = 0f; // Сбрасываем таймер отсутствия дрифта
                UpdateDriftMultiplierUI();

                // Динамически настраиваем параметры машины
                carController.MaxMotorTorque -= 100;
                carController.CarConfig.MaxRPM -= 500;
                carController.CarConfig.MinRPM -= 50;
                carController.CarConfig.RpmToNextGear -= 500;
                carController.CarConfig.RpmToPrevGear -= 500;
            }

            if (driftProgressSlider != null)
            {
                driftProgressSlider.value = 0f; // Сбрасываем слайдер, если дрифта нет
            }
        }
    }

    //private void UpdateDriftPoints()
    //{

    //    if (isDrifting)
    //    {
    //        noDriftTimer = 0f; // Reset the no-drift timer

    //        driftTimer += Time.deltaTime;

    //        // Calculate drift points based on time
    //        float pointsToAdd = driftMultiplier * pointsMultiplier * Time.deltaTime * 10f; // Scale by time
    //        if (isInDriftBoostZone)
    //        {
    //            pointsToAdd *= 2; // Double points in the drift boost zone
    //        }

    //        driftPoints += Mathf.RoundToInt(pointsToAdd); // Convert to integer for accuracy
    //        UpdateDriftPointsUI();

    //        // Dynamically increasing time to gain multipliers
    //        float requiredDriftTime = 3f + (driftMultiplier - 3); // Takes longer for higher multipliers

    //        // Update the slider fill amount
    //        if (driftProgressSlider != null)
    //        {
    //            driftProgressSlider.value = driftTimer / requiredDriftTime; // Normalize between 0-1
    //        }

    //        if (driftTimer >= requiredDriftTime)
    //        {
    //            driftMultiplier++;
    //            Instantiate(driftEffectPrefab, transform.position, Quaternion.identity); // Spawn effect
    //            explosion.TriggerExplosion();
    //            driftTimer = 0f; // Reset drift timer

    //            if (driftProgressSlider != null)
    //            {
    //                driftProgressSlider.value = 0f; // Reset slider
    //            }

    //            UpdateDriftMultiplierUI();

    //            // Adjust car settings dynamically
    //            carController.MaxMotorTorque += 100;
    //            carController.CarConfig.MaxRPM += 500;
    //            carController.CarConfig.MinRPM += 50;
    //            carController.CarConfig.RpmToNextGear += 500;
    //            carController.CarConfig.RpmToPrevGear += 500;
    //        }
    //    }
    //    else
    //    {
    //        driftTimer = 0f; // Reset drift timer if not drifting
    //        noDriftTimer += Time.deltaTime;

    //        // Dynamically decreasing time before losing multiplier
    //        float decayTime = Mathf.Max(2f - ((driftMultiplier - 2) * 0.7f), 0.7f); // Takes less time to lose at high multipliers

    //        if (noDriftTimer >= decayTime && driftMultiplier > 1)
    //        {
    //            driftMultiplier--; // Decrease multiplier
    //            noDriftTimer = 0f; // Reset no-drift timer
    //            UpdateDriftMultiplierUI();

    //            // Adjust car settings dynamically
    //            carController.MaxMotorTorque -= 100;
    //            carController.CarConfig.MaxRPM -= 500;
    //            carController.CarConfig.MinRPM -= 50;
    //            carController.CarConfig.RpmToNextGear -= 500;
    //            carController.CarConfig.RpmToPrevGear -= 500;
    //        }

    //        if (driftProgressSlider != null)
    //        {
    //            driftProgressSlider.value = 0f; // Reset slider when not drifting
    //        }
    //    }
    //}

    private void UpdateDriftPointsUI()
    {
        if (driftPointsText != null)
        {
            driftPointsText.text = driftPoints.ToString();
        }
    }

    private void UpdateDriftMultiplierUI()
    {
        if (driftMultiplierText != null)
        {
            driftMultiplierText.text = "x" + driftMultiplier;
        }
    }

    public void ResetDriftPoints()
    {
        driftPoints = 0;
        driftMultiplier = 1;
        UpdateDriftPointsUI();
        UpdateDriftMultiplierUI();

        if (driftProgressSlider != null)
        {
            driftProgressSlider.value = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DriftBoost"))
        {
            isInDriftBoostZone = true;
            boostedText.enabled = true;
        }

        if (!isFinished)
        {
            if (other.CompareTag("Death"))
            {
                DeathActivate();
                isFinished = true;
            }
        }
        
        if (!isFinished)
        {
            if (other.CompareTag("Finish"))
            {
                FinishRace();
                isFinished = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DriftBoost"))
        {
            isInDriftBoostZone = false;
            boostedText.enabled = false;
        }
    }

    private void DeathActivate()
    {
        Instantiate(driftEffectPrefab, transform.position, Quaternion.identity);
        FinishGame();
        
    }

    private void FinishRace() => FinishGame();

    private void FinishGame()
    {
        gameplayCanvas.SetActive(false);
        levelManager.finalScore.text = driftPointsText.text;

        levelManager.maxScore = driftPoints;
        
        var result = new GameResult
        {
            score = driftPoints,
            messageId = AllGamesServer.Instance.startData?.startParam,
            key = EncryptionService.GenerateGameResultKey(GameManager.Instance.SelfId, GameManager.LobbyGuid, driftPoints)
        };
        MultiplayerController.sendGameResult(GameManager.LobbyId, JsonUtility.ToJson(result));

        levelManager.FinishRace();


        //AllGamesServer.Instance.SendLobbyGameResult(GameManager.LobbyId, driftPoints, AllGamesServer.Instance.startData?.startParam, levelManager.FinishRace);


    }
}
    
