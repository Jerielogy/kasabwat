using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KasabwatMovement : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    private Rigidbody rb;
    private KasabwatController mainController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainController = FindFirstObjectByType<KasabwatController>();
        rb.freezeRotation = true;
        this.enabled = false;
    }

    void FixedUpdate()
    {
        if (mainController == null || mainController.currentState != GameState.Day3_Breach) return;
        if (mainController.isReadingNote) { rb.velocity = Vector3.zero; return; }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        Vector3 targetVel = moveDirection * walkSpeed;

        // FIXED: Ginamit ang .velocity para sa Unity 2022
        rb.velocity = new Vector3(targetVel.x, rb.velocity.y, targetVel.z);
    }
}