using UnityEngine;
using TMPro;

public class PowerUpHUDController : MonoBehaviour
{
    [Header("UI Canvas Components")]
    [Tooltip("Drag your standalone PowerUp_Status_Text here")]
    public TextMeshProUGUI statusText;        

    private string activePowerUpName = "";
    private float activeDuration = 0f;
    private bool isTimedPowerUp = false;

    void Start()
    {
        // Clear the text at launch so nothing shows on screen
        if (statusText != null) statusText.text = "";
    }

    void Update()
    {
        if (isTimedPowerUp && activeDuration > 0f)
        {
            activeDuration -= Time.deltaTime;

            if (activeDuration <= 0f)
            {
                HideHUD();
            }
            else
            {
                // Update the text directly frame-by-frame
                // statusText.text = $"{activePowerUpName}: {activeDuration.ToString("F1")}s";
                statusText.text = $"{activePowerUpName}";
            }
        }
    }

    // Call this for Star, SpeedUp, or Bomb
    public void DisplayTimedPowerUp(string name, float duration)
    {
        activePowerUpName = name.ToUpper();
        activeDuration = duration;
        isTimedPowerUp = true;

        // statusText.text = $"{activePowerUpName}: {activeDuration.ToString("F1")}s";
        statusText.text = $"{activePowerUpName}s";
    }

    // Call this when a Shield is picked up
    public void DisplayShieldHUD()
    {
        isTimedPowerUp = false;
        statusText.text = "SHIELD";
    }

    // Call this when a power-up expires or a shield breaks
    public void HideHUD()
    {
        isTimedPowerUp = false;
        activeDuration = 0f;
        
        // Simply empty out the text string to hide it from view!
        if (statusText != null) statusText.text = "";
    }
}