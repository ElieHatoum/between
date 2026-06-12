using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    public enum PowerUpType { Heart, Star, SpeedUp, Bomb, Shield }
    
    [Header("Power Up Configuration")]
    public PowerUpType type; 

    private Rigidbody rb;

    void Start()
    {
        // Grab the Rigidbody component we added to the prefab
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // For physics-based movement (IsKinematic Rigidbodies), we should use 
        // MovePosition inside FixedUpdate. This overrides any animation script locks!
        if (rb != null)
        {
            float relativeBackwardSpeed = EnvironmentScroller.gameSpeed;
            
            // Calculate the exact global backward vector position for this frame
            Vector3 forwardMovement = Vector3.back * relativeBackwardSpeed * Time.fixedDeltaTime;
            
            rb.MovePosition(rb.position + forwardMovement);
        }
    }

    void Update()
    {
        // We handle the self-destruction check in standard Update
        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }
}