using UnityEngine;
using TMPro; // Required to use TextMeshPro UI

public class ScoreManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI scoreText; // Reference to our Score UI element

    [Header("Score Configuration")]
    [Tooltip("Multiplier to make the score display as larger, arcade-style numbers")]
    public float scoreMultiplier = 2f; 

    private float currentScore = 0f;
    private bool isPlayerDead = false;

    void Update()
    {
        // Only accumulate score if the game is actively running and player is alive
        if (Time.timeScale > 0f && !isPlayerDead)
        {
            // Score based on distance: Speed * Time passed
            float distanceTraveledThisFrame = EnvironmentScroller.gameSpeed * Time.deltaTime;
            
            currentScore += distanceTraveledThisFrame * scoreMultiplier;

            // Display score as a whole integer number
            if (scoreText != null)
            {
                scoreText.text = "SCORE: " + Mathf.FloorToInt(currentScore).ToString();
            }
        }
    }

    // Call this from the player controller when they crash to halt the score calculations
    public void StopScoreTracking()
    {
        isPlayerDead = true;
    }

    public int GetFinalScore()
    {
        return Mathf.FloorToInt(currentScore);
    }
}