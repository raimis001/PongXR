// ball.cs

using UnityEngine;
using UnityEngine.InputSystem; // Keep this if needed elsewhere, otherwise optional for this script.

public class ball : MonoBehaviour
{
    public float initialBallForce = 50f;
    public float ballSpeedMultiplier = 1.1f;
    public float minBallSpeed = 7f;
    public float paddleVelocityInfluence = 0.5f;

    public float paddleLaunchSpeed = 10f; // Set a default value, adjust in Inspector
    public float wallBounceSpeed = 15f; // Set a default value, adjust in Inspector

    // NEW: Maximum speed the ball can travel
    public float maxBallSpeed = 50f; // Set your desired max speed here, adjust in Inspector

    public Transform playerPaddle; // Reference to the player's paddle script

    public float playerHeight = 1f;

    public Transform ballResetPosition;
    private Vector3 initialBallPosition;
    private Quaternion initialBallRotation;

    private Rigidbody body; // Reference to the ball's Rigidbody component

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;

        initialBallPosition = transform.position;
        initialBallRotation = transform.rotation;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.angularDamping = 1.0f; // As per your working code

        // This line makes the ball home to the main camera's position.
        // If your actual paddle is a separate GameObject, you might want to assign it
        // in the Inspector instead, or find it by tag.
        playerPaddle = Camera.main.transform;

        // Check to ensure playerPaddle is assigned (will log error if Camera.main is null)
        if (playerPaddle == null)
        {
            Debug.LogError("Player Paddle (Camera.main) not found or assigned! Homing will not work for " + gameObject.name + ".");
        }
    }

    public void DropBall()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = true; // Enable gravity for dropping
    }

    public void ResetBall()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = false;

        if (ballResetPosition != null)
        {
            transform.position = ballResetPosition.position;
            transform.rotation = ballResetPosition.rotation;
        }
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
        if (collision.gameObject.CompareTag("Paddle"))
        {
            Rigidbody paddleRb = collision.rigidbody; // Get the Rigidbody component of the collided paddle

            if (paddleRb != null)
            {
                ContactPoint contact = collision.contacts[0];
                Vector3 collisionNormal = contact.normal; // Normal points OUT from the paddle surface

                // --- PADDLE PUSH LOGIC ---

                Vector3 newVelocity = collisionNormal * paddleLaunchSpeed;
                newVelocity += (paddleRb.linearVelocity * paddleVelocityInfluence);
                newVelocity *= ballSpeedMultiplier;

                // Ensure the ball maintains a minimum speed.
                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                // NEW: Clamp velocity to maximum speed after all other calculations
                if (newVelocity.magnitude > maxBallSpeed)
                {
                    newVelocity = newVelocity.normalized * maxBallSpeed;
                }

                body.linearVelocity = newVelocity;
                body.angularVelocity = Vector3.zero; // Stop spin on paddle hit
            }
        }
        // Handle collisions with walls: Now with homing logic
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // --- WALL HIT LOGIC: SIMULATED HOMING HIT (SIMPLIFIED) ---

            if (playerPaddle != null) // Essential check to prevent NullReferenceException
            {
                Vector3 destination = playerPaddle.position + Vector3.up * playerHeight;
                Vector3 directionToPlayer = (destination - transform.position).normalized;

                Vector3 newVelocity = directionToPlayer * wallBounceSpeed; // Using wallBounceSpeed

                // Ensure the ball maintains a minimum speed (redundant if wallBounceSpeed >= minBallSpeed but good for safety)
                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                // NEW: Clamp velocity to maximum speed after all other calculations
                if (newVelocity.magnitude > maxBallSpeed)
                {
                    newVelocity = newVelocity.normalized * maxBallSpeed;
                }

                body.linearVelocity = newVelocity;
                body.angularVelocity = Vector3.zero; // Stop spin for predictable homing

                Debug.Log("Ball hit wall and is now homing towards player with defined speed!");
            }
            else
            {
                // Fallback: If playerPaddle is NOT assigned, reflect as before, but with min/max speed guarantee.
                Vector3 collisionNormal = collision.contacts[0].normal;
                Vector3 reflectedVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);

                // Ensure fallback reflection also has a minimum speed
                if (reflectedVelocity.magnitude < minBallSpeed)
                {
                    reflectedVelocity = reflectedVelocity.normalized * minBallSpeed;
                }

                // NEW: Clamp reflected velocity to maximum speed
                if (reflectedVelocity.magnitude > maxBallSpeed)
                {
                    reflectedVelocity = reflectedVelocity.normalized * maxBallSpeed;
                }

                body.linearVelocity = reflectedVelocity;
                body.angularVelocity = Vector3.zero;
                Debug.LogWarning("Player Paddle not assigned to ball script! Ball reflected off wall instead of homing.");
            }
        }
    }
}