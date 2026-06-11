using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    // How far left and right the car can go from the center lane (Z-axis line)
    public float roadWidthLimit = 4.5f; 

    [Header("Player Stats")]
    public int lives = 3;
    public float invulnerabilityDuration = 2f;

    private float currentXPosition = 0f;
    private bool isInvulnerable = false;
    private Renderer[] carRenderers; // Used for the flashing effect

    private ScoreManager scoreManager;

    public GameOverScript GameOverScript;

    void Start()
    {
        currentXPosition = transform.position.x;
        // Grab all renderers on the car (including child objects) for visual feedback
        carRenderers = GetComponentsInChildren<Renderer>();

        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontalInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) 
                horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) 
                horizontalInput = 1f;
        }
        
        if (Gamepad.current != null)
        {
            horizontalInput = Gamepad.current.leftStick.x.ReadValue();
        }
        
        currentXPosition += horizontalInput * moveSpeed * Time.deltaTime;
        
        currentXPosition = Mathf.Clamp(currentXPosition, -roadWidthLimit, roadWidthLimit);

        transform.position = new Vector3(currentXPosition, transform.position.y, 0f);
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        lives -= amount;
        Debug.Log($"Player hit! Lives remaining: {lives}");

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        ClearActiveObstacles();

        // Flash the car material to show invulnerability
        float timer = 0;
        while (timer < invulnerabilityDuration)
        {
            SetRenderersEnabled(false);
            yield return new WaitForSeconds(0.1f);
            SetRenderersEnabled(true);
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        isInvulnerable = false;
    }

    private void ClearActiveObstacles()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
    }

    private void SetRenderersEnabled(bool state)
    {
        foreach (var renderer in carRenderers)
        {
            if (renderer != null) renderer.enabled = state;
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (scoreManager != null) scoreManager.StopScoreTracking();
        Time.timeScale = 0f;
        GameOverScript.Setup(100); // Pass the player's score here instead of 100

    }
}