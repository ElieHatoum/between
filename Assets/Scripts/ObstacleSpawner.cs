using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Assets")]
    public GameObject[] carPrefabs;
    public float[] lanePositions = new float[] { -2.5f, 0f, 2.5f }; // Left, Center, Right

    [Header("Dynamic Traffic Settings")]
    public float minTimeBetweenCars = 1.2f; // Fast pace limit
    public float maxTimeBetweenCars = 4.0f; // Slow pace limit
    
    [Header("Safety Escape Window")]
    [Tooltip("The minimum forward distance (Z) required between cars to ensure a lane is physically passable.")]
    public float safetyPassageWindow = 12f; 

    // Track the last spawned car in each individual lane
    private GameObject[] lastSpawnedCarInLane = new GameObject[3];
    // Track independent timers for each lane
    private float[] laneTimers = new float[3];
    private float[] laneNextSpawnTimes = new float[3];

    void Start()
    {
        // Initialize random start timers for each lane so they don't sync up at the beginning
        for (int i = 0; i < 3; i++)
        {
            ResetLaneTimer(i);
            // Stagger initial spawn times so they don't fire at the exact same millisecond
            laneTimers[i] = Random.Range(0f, laneNextSpawnTimes[i]);
        }
    }

    void Update()
    {
        // Calculate a normalized difficulty multiplier based on game speed (0 to 1)
        float speedRatio = Mathf.InverseLerp(15f, 40f, EnvironmentScroller.gameSpeed);
        
        // Dynamically compress the spawn timers as the game accelerates
        float currentMinTime = Mathf.Lerp(minTimeBetweenCars, 0.5f, speedRatio);
        float currentMaxTime = Mathf.Lerp(maxTimeBetweenCars, 1.8f, speedRatio);

        // Run independent loops for each of the 3 lanes
        for (int lane = 0; lane < 3; lane++)
        {
            laneTimers[lane] += Time.deltaTime;

            if (laneTimers[lane] >= laneNextSpawnTimes[lane])
            {
                // Attempt to spawn a car in this specific lane
                TrySpawnCarInLane(lane);
                
                // Reset this lane's timer with a completely fresh random interval
                laneNextSpawnTimes[lane] = Random.Range(currentMinTime, currentMaxTime);
                laneTimers[lane] = 0f;
            }
        }
    }

    void TrySpawnCarInLane(int targetLane)
    {
        if (carPrefabs.Length == 0) return;

        // --- SAFETY CHECK SYSTEM ---
        // Identify the other two lanes we are NOT currently spawning in
        int otherLaneA = (targetLane + 1) % 3;
        int otherLaneB = (targetLane + 2) % 3;

        // Check if a car exists in those lanes and find its current distance
        float distA = GetLastCarDistance(otherLaneA);
        float distB = GetLastCarDistance(otherLaneB);

        // CRITICAL RULE: If BOTH other lanes have a car sitting within the safety window, 
        // spawning a car here would create a solid horizontal wall. We MUST skip/cancel this spawn!
        if (distA < safetyPassageWindow && distB < safetyPassageWindow)
        {
            // Debug.Log($"Spawn blocked in lane {targetLane} to preserve player safety gap!");
            return; 
        }

        // Check to ensure we don't spawn a car directly inside another car in the SAME lane
        if (GetLastCarDistance(targetLane) < 15f)
        {
            return;
        }

        // --- SPAWN EXECUTION ---
        float spawnX = lanePositions[targetLane];
        GameObject randomPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        
        Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, transform.position.z);
        GameObject newObstacle = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);

        // Store a reference to this car so this lane can track its position on future frames
        lastSpawnedCarInLane[targetLane] = newObstacle;
    }

    float GetLastCarDistance(int laneIndex)
    {
        // If there is no car, or it was already destroyed/passed the player, the lane is wide open (Infinite distance)
        if (lastSpawnedCarInLane[laneIndex] == null) 
            return float.MaxValue;

        // Calculate how close the car physically is to the Spawner's baseline (Z axis)
        return Mathf.Abs(transform.position.z - lastSpawnedCarInLane[laneIndex].transform.position.z);
    }

    void ResetLaneTimer(int laneIndex)
    {
        laneTimers[laneIndex] = 0f;
        laneNextSpawnTimes[laneIndex] = Random.Range(minTimeBetweenCars, maxTimeBetweenCars);
    }
}