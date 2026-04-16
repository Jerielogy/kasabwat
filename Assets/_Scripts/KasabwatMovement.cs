using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KasabwatMovement : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    private Rigidbody rb;
    private KasabwatController mainController;

    // Reference to the camera's transform
    private Transform camTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainController = FindFirstObjectByType<KasabwatController>();

        // Cache the main camera transform for better performance
        if (Camera.main != null)
            camTransform = Camera.main.transform;

        rb.freezeRotation = true;
        this.enabled = false;
    }

    void FixedUpdate()
    {
        // Check game state specific to your project
        if (mainController == null || mainController.currentState != GameState.Day3_Breach) return;
        if (mainController.isReadingNote) { rb.velocity = Vector3.zero; return; }

        float moveX = Input.GetAxis("Horizontal"); // A and D
        float moveZ = Input.GetAxis("Vertical");   // W and S

        // 1. Get camera directions
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;

        // 2. "Flatten" the vectors (Ignore Y so player doesn't fly/sink)
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Calculate direction relative to camera
        Vector3 moveDirection = (camForward * moveZ + camRight * moveX).normalized;
        Vector3 targetVel = moveDirection * walkSpeed;

        // 4. Apply velocity while preserving existing vertical (gravity) velocity
        rb.velocity = new Vector3(targetVel.x, rb.velocity.y, targetVel.z);
    }
}