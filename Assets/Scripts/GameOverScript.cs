using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI pointsText;
    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = score.ToString() + " points";
    }

    public void RestartBtn()
    {
        // Reset the time scale in case it was modified (e.g., for slow-motion effects)
        Time.timeScale = 1f;
        // resets the game speed
        EnvironmentScroller.gameSpeed = 15f;
        // Reload the current scene to restart the game
        SceneManager.LoadScene("SampleScene");
    }

    public void MenuBtn()
    {
        Time.timeScale = 1f;
        EnvironmentScroller.gameSpeed = 15f;
        EnvironmentScroller.acceleration = 0.1f;
        SceneManager.LoadScene("Main Menu");
    }
}
