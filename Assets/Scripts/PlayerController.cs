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
}
