using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float roadWidthLimit = 4.5f; 

    [Header("Player Stats")]
    public int lives = 3;
    public float invulnerabilityDuration = 2f;

    [Header("Power Up Active States")]
    public bool isShieldActive = false;
    private float scoreMultiplier = 1f;

    [Header("UI Settings")]
    public Image[] heartImages;  // Tes 3 objets Images dans le Canvas
    public Sprite fullHeart;     // Glisse l'image du cœur rouge ici
    public Sprite emptyHeart;    // Glisse l'image du cœur gris ici

    private float currentXPosition = 0f;
    public bool isInvulnerable = false;
    private Renderer[] carRenderers;

    private ScoreManager scoreManager;

    public PowerUpHUDController hudController;

    void Start()
    {
        currentXPosition = transform.position.x;
        carRenderers = GetComponentsInChildren<Renderer>();

        scoreManager = FindAnyObjectByType<ScoreManager>();
        hudController = FindAnyObjectByType<PowerUpHUDController>();
        UpdateHeartUI();
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
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
        }
        if (Gamepad.current != null) horizontalInput = Gamepad.current.leftStick.x.ReadValue();
        
        currentXPosition += horizontalInput * moveSpeed * Time.deltaTime;
        currentXPosition = Mathf.Clamp(currentXPosition, -roadWidthLimit, roadWidthLimit);
        transform.position = new Vector3(currentXPosition, transform.position.y, 0f);
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        lives -= amount;
        UpdateHeartUI();

        if (lives <= 0) GameOver();
        else StartCoroutine(InvulnerabilityRoutine());
    }

    void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // On change l'image selon si le joueur a encore cette vie ou pas
            if (i < lives) heartImages[i].sprite = fullHeart;
            else heartImages[i].sprite = emptyHeart;
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        ClearActiveObstacles();
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
        foreach (GameObject obstacle in obstacles) Destroy(obstacle);
    }

    private void SetRenderersEnabled(bool state)
    {
        foreach (var renderer in carRenderers) if (renderer != null) renderer.enabled = state;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        if (scoreManager != null) scoreManager.StopScoreTracking();
        Time.timeScale = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the item we drove through is tagged as a PowerUp
        if (other.CompareTag("PowerUp"))
        {
            PowerUpItem powerUp = other.GetComponent<PowerUpItem>();
            
            if (powerUp != null)
            {
                // The magic switch statement: filters out behavior based on the object's Enum Type
                switch (powerUp.type)
                {
                    case PowerUpItem.PowerUpType.Heart:
                        ApplyHeart();
                        break;
                    case PowerUpItem.PowerUpType.Star:
                        StartCoroutine(ApplyStar());
                        break;
                    case PowerUpItem.PowerUpType.SpeedUp:
                        StartCoroutine(ApplySpeedUpMultiplier());
                        break;
                    case PowerUpItem.PowerUpType.Bomb:
                        StartCoroutine(ApplyBombInvincibility());
                        break;
                    case PowerUpItem.PowerUpType.Shield:
                        ApplyShield();
                        break;
                }
            }

            // Destroy the power-up model from the highway so it looks eaten!
            Destroy(other.gameObject);
        }
    }

    // --- POWER UP EFFECTS LOGIC ---

    void ApplyHeart()
    {
        lives = Mathf.Min(3, lives + 1);
        UpdateHeartUI();
        Debug.Log($"+1 Life! Total lives: {lives}");
    }

    IEnumerator ApplyStar()
    {
        Debug.Log("STAR POWER!");
        if (hudController != null) hudController.DisplayTimedPowerUp("STAR", 5f); // <-- ADD THIS

        float originalSpeed = EnvironmentScroller.gameSpeed; 
        EnvironmentScroller.gameSpeed += 15f; 
        isInvulnerable = true; 

        yield return new WaitForSeconds(5f);

        EnvironmentScroller.gameSpeed = originalSpeed; 
        isInvulnerable = false;
    }

    IEnumerator ApplySpeedUpMultiplier()
    {
        Debug.Log("SCORE MULTIPLIER ACTIVE!");
        if (hudController != null) hudController.DisplayTimedPowerUp("2x SCORE", 6f); // <-- ADD THIS

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null) scoreManager.scoreMultiplier = 4f; 

        yield return new WaitForSeconds(6f); 

        if (scoreManager != null) scoreManager.scoreMultiplier = 2f; 
    }

    IEnumerator ApplyBombInvincibility()
    {
        Debug.Log("BOMB! Invincibility for 3 seconds.");
        isInvulnerable = false;
        ClearActiveObstacles(); // Blow up everything currently visible on screen!

        yield return new WaitForSeconds(3f);
        isInvulnerable = false;
    }

    void ApplyShield()
    {
        Debug.Log("SHIELD CHARGED!");
        isShieldActive = true;
        if (hudController != null) hudController.DisplayShieldHUD(); // <-- ADD THIS
    }
}
