using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject carPrefab; // The car prefab to spawn

    [Header("Spawn Points")]
    public List<Transform> spawnPoints; // List of spawn points

    // Returns a random spawn point
    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("LevelData: No spawn points assigned!");
            return null;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }
}
