// ball.cs

using UnityEngine;

using UnityEngine.InputSystem; // Keep this if needed elsewhere, otherwise optional for this script.



public class ball : MonoBehaviour

{

    public float initialBallForce = 50f;

    public float ballSpeedMultiplier = 1.1f;

    public float minBallSpeed = 7f;

    public float paddleVelocityInfluence = 0.5f;


    public Transform playerPaddle; // Reference to the player's paddle script

    public float playerHeight = 1;


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

        body.angularDamping = 1.0f; // Fixed: Changed from angularDamping to angularDrag


        playerPaddle = Camera.main.transform;


        // Check to ensure playerPaddle is assigned in the Inspector

        if (playerPaddle == null)

        {

            Debug.LogError("Player Paddle (pad) reference not set on " + gameObject.name + " ball script! Ball will not home towards player from walls.");

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

                newVelocity *= ballSpeedMultiplier;


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


            if (playerPaddle != null) // Ensure playerPaddle reference is set

            {

                float currentBallSpeed = body.linearVelocity.magnitude;

                float baseOutgoingMagnitude = Mathf.Max(currentBallSpeed, minBallSpeed);


                // Calculate the direction from the ball to the player's paddle

                Vector3 destination = playerPaddle.position + Vector3.up * playerHeight;

                Vector3 directionToPlayer = (destination - transform.position).normalized;


                Vector3 newVelocity = directionToPlayer * baseOutgoingMagnitude;

                //newVelocity *= ballSpeedMultiplier; // Apply game speed progression


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

                // Fallback: If playerPaddle is NOT assigned, reflect as before.

                Vector3 collisionNormal = collision.contacts[0].normal;

                body.linearVelocity = Vector3.Reflect(body.linearVelocity, collisionNormal);

                body.angularVelocity = Vector3.zero;

                Debug.LogWarning("Player Paddle not assigned to ball script! Ball reflected off wall instead of homing.");

            }

        }

    }

}
