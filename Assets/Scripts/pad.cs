// pad.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class pad : MonoBehaviour
{
    public float moveSpeed = 6f; // Speed at which the paddle moves
    public ball bumba;           // Reference to the ball script instance

    public Transform ballResetPosition; // Where the ball resets to

    private Vector2 deltaMove;   // Stores the input value for movement (X for horizontal, Y for vertical)
    private InputSystem_Actions actions; // Reference to the generated Input Actions class
    private Rigidbody rb;        // Reference to the Rigidbody component of the paddle

    private Vector3 initialBallPosition;
    private Quaternion initialBallRotation;

    // Add boundaries for paddle movement
    public float minX = -5f; // Adjust these values in the Inspector
    public float maxX = 5f;
    public float minY = -2f;
    public float maxY = 2f;
    // If you're moving along Z for depth, add minZ/maxZ instead of minY/maxY
    // public float minZ = -2f;
    // public float maxZ = 2f;


    void Awake()
    {
        actions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (bumba != null)
        {
            initialBallPosition = bumba.transform.position;
            initialBallRotation = bumba.transform.rotation;
        }
        else
        {
            Debug.LogError("Ball (bumba) reference not set on " + gameObject.name + " pad script!");
        }
    }

    void OnEnable()
    {
        actions.Enable();

        actions.Player.Move.performed += onMove;
        actions.Player.Move.canceled += onMove;
        actions.Player.Attack.performed += onDrop;
    }

    void OnDisable()
    {
        actions.Player.Move.performed -= onMove;
        actions.Player.Move.canceled -= onMove;
        actions.Player.Attack.performed -= onDrop;

        actions.Disable();
    }

    void onMove(InputAction.CallbackContext context)
    {
        // Read the 2D vector value. X will be -1/1 for A/D, Y will be -1/1 for S/W
        deltaMove = context.ReadValue<Vector2>();
        //Debug.Log("deltaMove: " + deltaMove); // Uncomment to see input values
       
        
    }

    void onDrop(InputAction.CallbackContext context)
    {
        if (bumba != null)
        {
            bumba.DropBall();
        }
    }

    public void ResetBallToInitialState()
    {
        if (bumba == null) return;

        bumba.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        bumba.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        bumba.GetComponent<Rigidbody>().useGravity = false;

        if (ballResetPosition != null)
        {
            bumba.transform.position = ballResetPosition.position;
            bumba.transform.rotation = ballResetPosition.rotation;
        }
        else
        {
            bumba.transform.position = initialBallPosition;
            bumba.transform.rotation = initialBallRotation;
        }

        Debug.Log("Ball reset!");
    }

    void Update() // Keep this for your 'R' key check
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBallToInitialState();
        }
    }

    void FixedUpdate()
    {
        // Calculate the desired position based on current Rigidbody position
        Vector3 currentPosition = rb.position;
        Vector3 desiredMovement = Vector3.zero;

        // Apply horizontal movement (deltaMove.x) along the paddle's local X-axis (transform.right)
        // or global X-axis (Vector3.right) if your paddle's local X isn't aligned.
        desiredMovement += transform.right * deltaMove.x * moveSpeed * Time.fixedDeltaTime;

        // Apply vertical movement (deltaMove.y) along the paddle's local Y-axis (transform.up)
        // or global Y-axis (Vector3.up). For ping-pong, global Y is typical for up/down.
        desiredMovement += transform.up * deltaMove.y * moveSpeed * Time.fixedDeltaTime;
        // If your "vertical" movement is actually into/out of the screen (along Z), use transform.forward or Vector3.forward/back instead.

        Vector3 targetPosition = currentPosition + desiredMovement;

        // --- Clamping (Important for keeping paddles on screen) ---
        // Assuming paddle moves in X and Y planes, and Z is its depth/forward direction
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        // targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ); // Uncomment if you want to clamp Z too

        // Move the Rigidbody to the new calculated position
        rb.MovePosition(targetPosition);
    }
}