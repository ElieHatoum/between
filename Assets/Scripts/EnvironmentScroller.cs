using UnityEngine;

public class EnvironmentScroller : MonoBehaviour
{
    [Header("Speed Settings")]
    public static float gameSpeed = 15f;    
    public float maxSpeed = 40f;           
    public float acceleration = 0.2f;       

    [Header("Looping Settings")]
    public float tileLength = 3f;
    public int totalSegments = 11;
    
    
    private float resetThreshold;

    void Start()
    {
        // When a single tile goes past Z = -3, it resets
        resetThreshold = -tileLength;
    }

    void Update()
    {
        // Accelerate the game speed over time
        if (gameSpeed < maxSpeed)
        {
            gameSpeed += acceleration * Time.deltaTime;
        }

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
}