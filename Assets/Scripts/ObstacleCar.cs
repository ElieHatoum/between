using UnityEngine;

public class ObstacleCar : MonoBehaviour
{
    [Header("Movement Settings")]
    [Range(0.1f, 0.9f)]
    public float speedFactor = 1f; 

    void Update()
    {
        // Calculate the real relative backward speed
        // If road is 15f and driver speed is 5f, this car moves back at 10f.
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

                // DIAGNOSING CRASH DIRECTION:
                // Since obstacles travel backward (-Z), if the player hits the BACK of an obstacle, 
                // the player's position will be at a lower Z value relative to the obstacle.
                // If directionToPlayer.z is significantly negative, it's a rear-end crash.
                if (directionToPlayer.z < -0.4f)
                {
                    Debug.Log("CRASHED FROM BEHIND! Instant Game Over.");
                    player.TakeDamage(3); // Deducts all 3 lives immediately
                }
                else
                {
                    Debug.Log("SIDE SWIPE! Took 1 damage.");
                    player.TakeDamage(1); // Deducts 1 life, triggers flashing/clearing
                    Destroy(gameObject);  // Destroy this specific car so it doesn't hit again
                }
            }
        }
    }
}