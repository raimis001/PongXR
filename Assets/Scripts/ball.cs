// ball.cs
using UnityEngine;
using UnityEngine.InputSystem; // Keep this if you plan to add ball-specific input later, otherwise optional

public class ball : MonoBehaviour
{
    public float initialBallForce = 50f; // Force applied when the ball is initially dropped
    public float ballSpeedMultiplier = 1.1f; // Multiplier to slightly increase ball speed after each hit
    public float minBallSpeed = 7f;          // Minimum speed the ball should maintain after a hit
    public float paddleVelocityInfluence = 0.5f; // How much of the paddle's velocity is added to the ball (0.0 to 1.0 or more)

    // NEW: Properties for ball reset
    public Transform ballResetPosition; // Optional: Assign a specific Transform in editor for reset
    private Vector3 initialBallPosition;
    private Quaternion initialBallRotation;

    private Rigidbody body; // Reference to the ball's Rigidbody component

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false; // Initially, gravity is off until the ball is "dropped" into play.
        body.collisionDetectionMode = CollisionDetectionMode.Continuous; // For fast-moving objects

        // Store the ball's initial position and rotation when the game starts.
        // This is its fallback reset point if ballResetPosition is not assigned.
        initialBallPosition = transform.position;
        initialBallRotation = transform.rotation;

        // Ensure the ball starts stopped and without spin
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.angularDamping = 1.0f; // Added from previous discussions to dampen spin
    }

    // Public method to start the ball's movement, typically called by a paddle or game manager.
    public void DropBall()
    {
        // Ensure ball is completely stopped before launching
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        // Apply an initial impulse force to get the ball moving.
        body.AddForce(transform.forward * initialBallForce, ForceMode.Impulse);
        body.useGravity = true; // Enable gravity so the ball falls.
    }

    // NEW: Public method to reset the ball, now living in the ball.cs script
    public void ResetBall()
    {
        // Stop all ball movement immediately
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false; // Turn off gravity until next drop

        // Reset position and rotation
        // Option 1: To a predefined reset position (e.g., center of the table) if assigned
        if (ballResetPosition != null)
        {
            transform.position = ballResetPosition.position;
            transform.rotation = ballResetPosition.rotation; // Or Quaternion.identity if you want no rotation
        }
        // Option 2: To its exact starting position when the game began
        else
        {
            transform.position = initialBallPosition;
            transform.rotation = initialBallRotation;
        }

        Debug.Log("Ball reset!");
    }


    // Called when this collider/rigidbody has begun touching another rigidbody/collider.
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided GameObject has the "Paddle" tag.
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Rigidbody paddleRb = collision.rigidbody; // Get the Rigidbody component of the collided paddle

            // Proceed only if the paddle has a Rigidbody attached.
            if (paddleRb != null)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 collisionNormal = contact.normal;

                Vector3 reflectedVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);

                Vector3 newVelocity = reflectedVelocity + (paddleRb.linearVelocity * paddleVelocityInfluence);

                newVelocity *= ballSpeedMultiplier;

                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                body.linearVelocity = newVelocity;
                body.angularVelocity = Vector3.zero; // Stop spin on hit
            }
        }
        // Handle collisions with walls.
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            body.linearVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);
            body.angularVelocity = Vector3.zero; // Stop spin on hit
        }
    }
}