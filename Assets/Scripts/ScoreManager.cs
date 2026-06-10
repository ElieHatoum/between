using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; // Assign a UI Text element here
    private float score = 0f;

    void Update()
    {
        if (Time.timeScale > 0) // Only count if game is running
        {
            score += EnvironmentScroller.gameSpeed * Time.deltaTime;
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }
}