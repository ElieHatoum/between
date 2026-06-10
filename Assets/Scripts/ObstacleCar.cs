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
        // Check if the thing we ran into is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            
            if (player != null)
            {
                // Calculate the direction from the Obstacle to the Player
                Vector3 contactPoint = collision.GetContact(0).point;
                Vector3 directionToPlayer = (collision.transform.position - transform.position).normalized;

                if (directionToPlayer.z < -0.4f)
                {
                    Debug.Log("CRASHED FROM BEHIND! Instant Game Over.");
                    player.TakeDamage(3); // Deducts all 3 lives immediately
                }
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