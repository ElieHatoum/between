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
    public int lives = 4;
    public float invulnerabilityDuration = 2f;

    [Header("Power Up Active States")]
    public bool isShieldActive = false;
    private float scoreMultiplier = 1f;

    [Header("UI Settings")]
    public Image[] heartImages;  // Tes 3 objets Images dans le Canvas
    public Sprite fullHeart;     // Glisse l'image du cœur rouge ici
    public Sprite emptyHeart;    // Glisse l'image du cœur gris ici

    private float currentXPosition = 0f;
    private bool isInvulnerable = false;
    private Renderer[] carRenderers;

    private ScoreManager scoreManager;

    void Start()
    {
        currentXPosition = transform.position.x;
        carRenderers = GetComponentsInChildren<Renderer>();

        scoreManager = FindAnyObjectByType<ScoreManager>();
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
        lives = Mathf.Min(4, lives + 1); // Adds 1 life, maxing out at 3
        Debug.Log($"+1 Life! Total lives: {lives}");
    }

    IEnumerator ApplyStar()
    {
        Debug.Log("STAR POWER! Hyper Speed + Invincibility active.");
        float originalAcceleration = EnvironmentScroller.gameSpeed; 
        
        EnvironmentScroller.gameSpeed += 15f; // Boost the scrolling highway speed up dramatically
        isInvulnerable = true; 

        yield return new WaitForSeconds(5f);

        EnvironmentScroller.gameSpeed = originalAcceleration; // Return to normal speed
        isInvulnerable = false;
        Debug.Log("Star Power ended.");
    }

    IEnumerator ApplySpeedUpMultiplier()
    {
        Debug.Log("SCORE MULTIPLIER ACTIVE!");
        // We will hook this into your ScoreManager via a quick multiplier modification
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null) scoreManager.scoreMultiplier = 4f; // Double the score accumulation rate

        yield return new WaitForSeconds(6f); // Lasts for a brief period

        if (scoreManager != null) scoreManager.scoreMultiplier = 2f; // Return to baseline
    }

    IEnumerator ApplyBombInvincibility()
    {
        Debug.Log("BOMB! Invincibility for 3 seconds.");
        isInvulnerable = true;
        ClearActiveObstacles(); // Blow up everything currently visible on screen!

        yield return new WaitForSeconds(3f);
        isInvulnerable = false;
    }

    void ApplyShield()
    {
        Debug.Log("SHIELD CHARGED! Backwards collision armor active.");
        isShieldActive = true;
        // Optional: toggle a blue energy ring visual child object on your car here!
    }
}
