using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    private float score = 0f;

    void Update()
    {
        if (Time.timeScale > 0)
        {
            score += EnvironmentScroller.gameSpeed * Time.deltaTime;
            scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }
}