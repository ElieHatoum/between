using UnityEngine;

public class ObstacleCar : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(0.1f, 0.9f)]
    public float speedFactor = 1f; 
    private EnvironmentScroller environmentScroller;

    private ScoreManager scoreManager;


    void Awake()
    {
        environmentScroller = FindAnyObjectByType<EnvironmentScroller>();
        scoreManager = FindAnyObjectByType<ScoreManager>();
    }

    void Update()
    {
        // Calculate the real relative backward speed
        float relativeBackwardSpeed = EnvironmentScroller.gameSpeed - speedFactor;
        
        // Ensure the car never accidentally moves forward if the game is too slow
        relativeBackwardSpeed = Mathf.Max(0f, relativeBackwardSpeed);

        // Move backward down the highway
        transform.Translate(Vector3.back * relativeBackwardSpeed * Time.deltaTime);

        // Self-destruct once it passes safely behind the player
        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            //reduce player's speed by 20% for 2 seconds
            if (player != null)
            {
                environmentScroller.ReduceSpeed(0.2f, 2f);
            }

            if (player != null)
            {
                if (player.isInvulnerable && EnvironmentScroller.gameSpeed > 25f) 
                {
                    Debug.Log("Star Power smashed through an obstacle!");
                    Destroy(gameObject); 
                    return; // Stop right here, don't execute any code below!
                }

                // Calculate the direction from the Obstacle to the Player
                Vector3 directionToPlayer = (collision.transform.position - transform.position).normalized;

                // Handle normal Rear-End crash
                if (directionToPlayer.z < -0.4f)
                {
                    if (player.isShieldActive) {
                        player.isShieldActive = false;
                        // Find the HUD controller and turn off ONLY the shield text
                        PowerUpHUDController hud = FindAnyObjectByType<PowerUpHUDController>();
                        if (hud != null) hud.HideHUD();
                        
                        Destroy(gameObject);
                        
                    } else {

                        Debug.Log("CRASHED FROM BEHIND! Instant Game Over.");
                        player.TakeDamage(3); // Deducts all 3 lives immediately
                        // highscore save
                        int finalScore = scoreManager.GetFinalScore();
                        int lastHightScore = PlayerPrefs.GetInt("HighScore", 0);

                        if (finalScore > lastHightScore)
                        {
                            PlayerPrefs.SetInt("HighScore", finalScore);
                        }
                    }
                // Handle normal Side-Swipe
                } else {
                    Debug.Log("SIDE SWIPE! Took 1 damage.");
                    player.TakeDamage(1); 
                    Destroy(gameObject);  
                    print("hit from the side");
                    player.TakeDamage(1);
                    Destroy(gameObject);
                }
            }
        }
    }
}