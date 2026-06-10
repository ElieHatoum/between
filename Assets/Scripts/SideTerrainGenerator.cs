using UnityEngine;
using System.Collections.Generic;

public class SideTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Assets")]
    public GameObject grassTilePrefab;

    [Header("Grid Settings")]
    public int columnsWidth = 4;        
    public float tileWidthSize = 3f;    
    public float tileLengthSize = 3f;   

    [Header("Road Alignment")]
    [Tooltip("Distance from the center (X=0) to where the grass should start. Increase this if grass overlaps the road.")]
    public float edgeDistanceOffset = 6.5f; 

    [Header("Random Elevation")]
    [Tooltip("Slightly negative value keeps the grass flush or gently sloping down from the road edge.")]
    public float minElevation = -0.15f;   
    [Tooltip("Maximum height the hills can reach. Keep this low (e.g., 0.1 or 0.2) so it doesn't block the camera view.")]
    public float maxElevation = 0.2f;    
    public float noiseRoughness = 0.3f;  

    private List<GameObject> activeTerrainTiles = new List<GameObject>();
    private float zSeedOffset = 0f;


    void Start()
    {
        GenerateTerrain();
    }

    public void RegenerateTerrain()
    {
        zSeedOffset = Random.Range(0f, 5000f);

        ClearOldTerrain();
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        if (grassTilePrefab == null) return;

        for (int x = 0; x < columnsWidth; x++)
        {
            float targetX = -edgeDistanceOffset - (x * tileWidthSize);
            SpawnTileAt(targetX);
        }

        for (int x = 0; x < columnsWidth; x++)
        {
            float targetX = edgeDistanceOffset + (x * tileWidthSize);
            SpawnTileAt(targetX);
        }
    }

    void SpawnTileAt(float xPos)
    {
        float worldX = xPos + transform.position.x;
        
        float worldZ = transform.position.z + zSeedOffset;
        
        float noiseValue = Mathf.PerlinNoise(worldX * noiseRoughness, worldZ * noiseRoughness);
        float randomY = Mathf.Lerp(minElevation, maxElevation, noiseValue);

        Vector3 spawnPos = new Vector3(xPos, randomY, 0f);

        GameObject tile = Instantiate(grassTilePrefab, transform);
        tile.transform.localPosition = spawnPos;
        tile.transform.localRotation = Quaternion.identity;

        activeTerrainTiles.Add(tile);
    }

    void ClearOldTerrain()
    {
        foreach (GameObject tile in activeTerrainTiles)
        {
            if (tile != null) Destroy(tile);
        }
        activeTerrainTiles.Clear();
    }
}