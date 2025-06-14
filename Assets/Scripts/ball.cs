// ball.cs
using UnityEngine;
using UnityEngine.InputSystem; 


public class ball : MonoBehaviour
{
    public float initialBallForce = 50f;
    public float ballSpeedMultiplier = 1.1f;
    public float minBallSpeed = 7f;
    public float paddleVelocityInfluence = 0.5f;

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
        body.angularDamping = 1.0f; // Ensure angular drag is set
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

                // --- NEW PADDLE PUSH LOGIC ---

                // 1. Get the current ball's speed. We'll use this to determine the base outgoing speed.
                float currentBallSpeed = body.linearVelocity.magnitude;

                // 2. Determine a base outgoing speed for the ball after contact.
                // It should be at least minBallSpeed, and potentially carry over some of the current speed.
                float baseOutgoingMagnitude = Mathf.Max(currentBallSpeed, minBallSpeed);

                // 3. Set the new velocity to primarily go in the direction of the collision normal,
                // scaled by our base outgoing magnitude. This is the "push away" part.
                Vector3 newVelocity = collisionNormal * baseOutgoingMagnitude;

                // 4. Add the paddle's linear velocity (scaled by influence) to the ball's new velocity.
                // This gives the ball extra speed if the paddle is moving into it,
                // and influences its direction if the paddle is moving sideways.
                newVelocity += (paddleRb.linearVelocity * paddleVelocityInfluence);

                // 5. Apply the overall game speed multiplier.
                newVelocity *= ballSpeedMultiplier;

                // --- END NEW PADDLE PUSH LOGIC ---

                // 6. Final check to ensure the ball maintains a minimum speed.
                if (newVelocity.magnitude < minBallSpeed)
                {
                    newVelocity = newVelocity.normalized * minBallSpeed;
                }

                // Apply the final calculated linear velocity to the ball.
                body.linearVelocity = newVelocity;

                // 7. Stop all angular velocity after a paddle hit for more predictable linear motion.
                body.angularVelocity = Vector3.zero;
            }
        }
        // Handle collisions with walls.
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            body.linearVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal); // Walls still reflect
            body.angularVelocity = Vector3.zero; // Stop spin on wall hit too
        }
    }
}