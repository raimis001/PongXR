// ball.cs
using UnityEngine;
using UnityEngine.InputSystem; // Not strictly necessary for this version of ball.cs, but often included for consistency

public class ball : MonoBehaviour
{
    public float initialBallForce = 50f; // Force applied when the ball is initially dropped
    public float ballSpeedMultiplier = 1.1f; // Multiplier to slightly increase ball speed after each hit
    public float minBallSpeed = 7f;        // Minimum speed the ball should maintain after a hit
    public float paddleVelocityInfluence = 0.5f; // How much of the paddle's velocity is added to the ball (0.0 to 1.0 or more)

    private Rigidbody body; // Reference to the ball's Rigidbody component

    void Start()
    {
        body = GetComponent<Rigidbody>();
        // Initially, gravity is off until the ball is "dropped" into play.
        body.useGravity = false;
        // For fast-moving objects like a ping-pong ball, 'Continuous' collision detection
        // helps prevent it from passing through other colliders.
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Public method to start the ball's movement, typically called by a paddle or game manager.
    public void DropBall()
    {
        // Apply an initial impulse force to get the ball moving.
        body.AddForce(transform.forward * initialBallForce, ForceMode.Impulse);
        body.useGravity = true; // Enable gravity so the ball falls.
    }

    // Called when this collider/rigidbody has begun touching another rigidbody/collider.
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided GameObject has the "Paddle" tag.
        // IMPORTANT: Ensure your paddle GameObjects in Unity have the tag "Paddle"!
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Rigidbody paddleRb = collision.rigidbody; // Get the Rigidbody component of the collided paddle

            // Proceed only if the paddle has a Rigidbody attached.
            if (paddleRb != null)
            {
                // Get the contact point information, specifically the normal (direction away from surface).
                ContactPoint contact = collision.contacts[0];
                Vector3 collisionNormal = contact.normal;

                // Reflect the ball's current velocity off the collision normal.
                // This gives the primary "bounce" effect.
                Vector3 reflectedVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);

                // Calculate the new velocity by adding the reflected velocity
                // and a component of the paddle's velocity.
                // 'paddleVelocityInfluence' controls how strongly the paddle's movement affects the ball.
                Vector3 newVelocity = reflectedVelocity + (paddleRb.linearVelocity * paddleVelocityInfluence);

                // Apply a speed boost to the ball after a hit.
                newVelocity *= ballSpeedMultiplier;

                // Ensure the ball maintains a minimum speed to keep the game engaging.
                // Prevents the ball from stalling after a weak hit.
                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                // Set the ball's Rigidbody velocity to the newly calculated velocity.
                body.linearVelocity = newVelocity;

                // Optional: You could add a small additional force (impulse) directly
                // at the collision point for a stronger 'hit' feel.
                // body.AddForceAtPosition(collisionNormal * 50f, contact.point, ForceMode.Impulse);
            }
        }
        // Handle collisions with walls.
        // IMPORTANT: Ensure your wall GameObjects have the tag "Wall"!
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Simple reflection for walls, ensuring the ball bounces off them.
            Vector3 collisionNormal = collision.contacts[0].normal;
            body.linearVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);
        }
        
    }
}