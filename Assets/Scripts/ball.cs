// ball.cs
using UnityEngine;
using UnityEngine.InputSystem; // Keep this if needed elsewhere, otherwise optional for this script.


public class ball : MonoBehaviour
{
    public float initialBallForce = 50f;
    public float ballSpeedMultiplier = 1.1f;
    public float minBallSpeed = 9f;
    public float paddleVelocityInfluence = 0.5f;

    // CONSIDER: If your actual paddle is a separate GameObject from the camera,
    // you might want this to be of type 'pad' or assigned in the inspector.
    // public pad playerPaddleScript; // Example if referencing the script directly
    public Transform playerPaddleTransform; // Changed name for clarity, still assign in Inspector if not Camera.main

    public float playerHeight = 1f; // Use 'f' for float literals for consistency

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
        body.angularDamping = 1.0f; // FIXED: Changed from angularDamping to angularDrag

        // IMPORTANT: If 'playerPaddleTransform' is your actual paddle object,
        // it's best to assign it in the Inspector or find it by tag/type.
        // Assigning Camera.main.transform here means the ball homes to the player's head.
        // If that's your intent for a VR game, this is fine!
        // Otherwise, consider:
        // playerPaddleTransform = GameObject.FindGameObjectWithTag("PlayerPaddle").transform;
        // Or assign manually in the Inspector.
        // For now, I'll keep your line commented out to highlight this choice.
        // playerPaddleTransform = Camera.main.transform; // Your original line, reconsider if your paddle is separate

        // Using playerPaddleTransform for the null check, consistent with usage later.
        if (playerPaddleTransform == null)
        {
            Debug.LogError("Player Paddle Transform not set on " + gameObject.name + " ball script! Ball will not home towards player from walls.");
            // If you intend to use Camera.main.transform, this error might show up
            // if Camera.main hasn't initialized yet or is not tagged as "MainCamera".
        }
    }

    public void DropBall()
    {
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.useGravity = true; // Enable gravity for dropping

        // Removed the initial AddForce for dropping in place, as per previous discussion.
        // If you want it to launch forward again, uncomment this:
        // body.AddForce(transform.forward * initialBallForce, ForceMode.Impulse);
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

                float currentBallSpeed = body.linearVelocity.magnitude;
                float baseOutgoingMagnitude = Mathf.Max(currentBallSpeed, minBallSpeed);
                Vector3 newVelocity = collisionNormal * baseOutgoingMagnitude;
                newVelocity += (paddleRb.linearVelocity * paddleVelocityInfluence);
                newVelocity *= ballSpeedMultiplier; // Apply paddle hit speed progression

                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                body.linearVelocity = newVelocity;
                body.angularVelocity = Vector3.zero; // Stop spin on paddle hit
            }
        }
        // Handle collisions with walls: Now with homing logic
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // --- WALL HIT LOGIC: SIMULATED HOMING HIT ---

            // Using playerPaddleTransform for the homing target
            if (playerPaddleTransform != null) // Ensure target is set
            {
                float currentBallSpeed = body.linearVelocity.magnitude;
                float baseOutgoingMagnitude = Mathf.Max(currentBallSpeed, minBallSpeed);

                // Calculate the target destination, offset by playerHeight
                Vector3 destination = playerPaddleTransform.position + Vector3.up * playerHeight;
                Vector3 directionToPlayer = (destination - transform.position).normalized;

                Vector3 newVelocity = directionToPlayer * baseOutgoingMagnitude;
                // Removed: newVelocity *= ballSpeedMultiplier; as per your commented out line

                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                body.linearVelocity = newVelocity;
                body.angularVelocity = Vector3.zero; // Stop spin for predictable homing

                Debug.Log("Ball hit wall and is now homing towards player!");
            }
            else
            {
                // Fallback: If playerPaddleTransform is NOT assigned, reflect as before.
                Vector3 collisionNormal = collision.contacts[0].normal;
                Vector3 reflectedVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);

                // Ensure fallback reflection also has minimum speed
                if (reflectedVelocity.magnitude < minBallSpeed)
                {
                    reflectedVelocity = reflectedVelocity.normalized * minBallSpeed;
                }

                body.linearVelocity = reflectedVelocity;
                body.angularVelocity = Vector3.zero;
                Debug.LogWarning("Player Paddle Transform not assigned to ball script! Ball reflected off wall instead of homing.");
            }
        }
    }
}