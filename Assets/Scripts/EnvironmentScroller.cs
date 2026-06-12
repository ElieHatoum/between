using UnityEngine;
using System.Collections;

public class EnvironmentScroller : MonoBehaviour
{
    [Header("Speed Settings")]
    public static float gameSpeed = 15f;    
    public float maxSpeed = 40f;           
    public static float acceleration = 0.1f;

    [Header("Looping Settings")]
    public float tileLength = 3f;
    public int totalSegments = 11;
    
    
    private float resetThreshold;

    void Start()
    {
        gameSpeed = 15f;
        acceleration = 0.1f;
        resetThreshold = -tileLength;
    }

    void Update()
    {
        // Accelerate the game speed over time
        if (gameSpeed < maxSpeed)
        {
            gameSpeed += acceleration * Time.deltaTime;
        }
        print("Current Game Speed: " + gameSpeed);

        // Move the highway tile backward
        transform.Translate(Vector3.back * gameSpeed * Time.deltaTime);

        if (transform.position.z <= resetThreshold)
        {
            float totalChainLength = tileLength * totalSegments; 

            transform.position = new Vector3(
                transform.position.x, 
                transform.position.y, 
                transform.position.z + totalChainLength
            );

            SideTerrainGenerator terrainGen = GetComponent<SideTerrainGenerator>();
            if (terrainGen != null)
            {
                terrainGen.RegenerateTerrain();
            }
        }
    }

    public void ReduceSpeed(float percentage, float duration)
    {
        StartCoroutine(SpeedReductionCoroutine(percentage, duration));
    }

    private IEnumerator SpeedReductionCoroutine(float percentage, float duration)
    {
        float reducedSpeed = gameSpeed * (1f - percentage);
        float targetSpeed = gameSpeed;
        gameSpeed = reducedSpeed;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gameSpeed = Mathf.Lerp(reducedSpeed, targetSpeed, elapsed / duration);
            yield return null;
        }

        gameSpeed = targetSpeed;
    }
}