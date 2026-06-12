using UnityEngine;
using TMPro;

public class bestScore : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best Score : " + highScore.ToString();
        }
    }

}
