using UnityEngine;

public class ObstacleCar : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(0.1f, 0.9f)]
    public float speedFactor = 1f; 

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
                    if (player.isShieldActive) 
                    {
                        player.isShieldActive = false;
                        // Find the HUD controller and turn off ONLY the shield text
                        PowerUpHUDController hud = FindAnyObjectByType<PowerUpHUDController>();
                        if (hud != null) hud.HideHUD();
                        
                        Destroy(gameObject);
                    }
                    else 
                    {
                        Debug.Log("CRASHED FROM BEHIND! Instant Game Over.");
                        player.TakeDamage(3);
                    }
                }
                // Handle normal Side-Swipe
                else
                {
                    Debug.Log("SIDE SWIPE! Took 1 damage.");
                    player.TakeDamage(1); 
                    Destroy(gameObject);  
                }
            }
        }
    }
}