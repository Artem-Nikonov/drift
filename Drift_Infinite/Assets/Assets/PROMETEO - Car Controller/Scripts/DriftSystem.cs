using TMPro;
using UnityEngine;
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

        Debug.Log(driftPoints);
        if (isDrifting)
        {
            noDriftTimer = 0f; // Reset the no-drift timer

            driftTimer += Time.deltaTime;

            // Calculate drift points based on time
            float pointsToAdd = driftMultiplier * pointsMultiplier * Time.deltaTime * 10f; // Scale by time
            if (isInDriftBoostZone)
            {
                pointsToAdd *= 2; // Double points in the drift boost zone
            }

            driftPoints += Mathf.RoundToInt(pointsToAdd); // Convert to integer for accuracy
            UpdateDriftPointsUI();

            // Dynamically increasing time to gain multipliers
            float requiredDriftTime = 3f + (driftMultiplier - 3); // Takes longer for higher multipliers

            // Update the slider fill amount
            if (driftProgressSlider != null)
            {
                driftProgressSlider.value = driftTimer / requiredDriftTime; // Normalize between 0-1
            }

            if (driftTimer >= requiredDriftTime)
            {
                driftMultiplier++;
                Instantiate(driftEffectPrefab, transform.position, Quaternion.identity); // Spawn effect
                explosion.TriggerExplosion();
                driftTimer = 0f; // Reset drift timer

                if (driftProgressSlider != null)
                {
                    driftProgressSlider.value = 0f; // Reset slider
                }

                UpdateDriftMultiplierUI();

                // Adjust car settings dynamically
                carController.MaxMotorTorque += 100;
                carController.CarConfig.MaxRPM += 500;
                carController.CarConfig.MinRPM += 50;
                carController.CarConfig.RpmToNextGear += 500;
                carController.CarConfig.RpmToPrevGear += 500;
            }
        }
        else
        {
            driftTimer = 0f; // Reset drift timer if not drifting
            noDriftTimer += Time.deltaTime;

            // Dynamically decreasing time before losing multiplier
            float decayTime = Mathf.Max(2f - ((driftMultiplier - 2) * 0.7f), 0.7f); // Takes less time to lose at high multipliers

            if (noDriftTimer >= decayTime && driftMultiplier > 1)
            {
                driftMultiplier--; // Decrease multiplier
                noDriftTimer = 0f; // Reset no-drift timer
                UpdateDriftMultiplierUI();

                // Adjust car settings dynamically
                carController.MaxMotorTorque -= 100;
                carController.CarConfig.MaxRPM -= 500;
                carController.CarConfig.MinRPM -= 50;
                carController.CarConfig.RpmToNextGear -= 500;
                carController.CarConfig.RpmToPrevGear -= 500;
            }

            if (driftProgressSlider != null)
            {
                driftProgressSlider.value = 0f; // Reset slider when not drifting
            }
        }
    }

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
        gameplayCanvas.SetActive(false);
        levelManager.FinishRace();
        levelManager.finalScore.text = driftPointsText.text;
        Debug.Log($"-=-=-=-=-=-=-=-=-=--={driftPoints}");
    }
    
    private void FinishRace()
    {
        Debug.Log($"=============={driftPoints}");
        gameplayCanvas.SetActive(false);
        levelManager.FinishRace();
        levelManager.finalScore.text = driftPointsText.text;
        
    }
}
    
