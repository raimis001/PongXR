// pad.cs
using UnityEngine;
using UnityEngine.InputSystem; 

public class pad : MonoBehaviour
{
    public float moveSpeed = 6f; // Speed at which the paddle moves
    public ball bumba;           // Reference to the ball script instance

    private Vector2 deltaMove;   // Stores the input value for movement (X for horizontal, Y for vertical)
    private InputSystem_Actions actions; // Reference to the generated Input Actions class
    private Rigidbody rb;        // Reference to the Rigidbody component of the paddle

    // Add boundaries for paddle movement
    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -2f;
    public float maxY = 2f;


    void Awake()
    {
        actions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (bumba == null)
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

        // REMOVED: actions.Player.Reset.performed += onResetBall;
        // The 'R' key reset will now be handled in Update()
    }

    void OnDisable()
    {
        actions.Player.Move.performed -= onMove;
        actions.Player.Move.canceled -= onMove;
        actions.Player.Attack.performed -= onDrop;

        // REMOVED: actions.Player.Reset.performed -= onResetBall;
        actions.Disable();
    }

    void onMove(InputAction.CallbackContext context)
    {
        deltaMove = context.ReadValue<Vector2>();
    }

    void onDrop(InputAction.CallbackContext context)
    {
        if (bumba != null)
        {
            bumba.DropBall();
        }
    }

    // REMOVED: The onResetBall(InputAction.CallbackContext context) method is gone.
    // Its functionality is moved to Update() directly.


    // ** NEW/RESTORED: Update method for traditional input checks **
    void Update()
    {
        // Check if the 'R' key was just pressed down using the old Input system
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (bumba != null)
            {
                bumba.ResetBall(); // Call the ResetBall method directly on the ball
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 currentPosition = rb.position;
        Vector3 desiredMovement = Vector3.zero;

        // Using transform.right and transform.up if your paddle's local axes align with world axes.
        // Otherwise, consider Vector3.right and Vector3.up for global movement.
        desiredMovement += transform.right * deltaMove.x * moveSpeed * Time.fixedDeltaTime;
        desiredMovement += transform.up * deltaMove.y * moveSpeed * Time.fixedDeltaTime;

        Vector3 targetPosition = currentPosition + desiredMovement;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        rb.MovePosition(targetPosition);
    }
}