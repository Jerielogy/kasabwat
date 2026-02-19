using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Look Settings")]
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Locks the mouse so it controls your eyes/head
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 2. Vertical Look (Up/Down) - Clamped so you don't break your neck
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);

        // 3. Horizontal Look (Left/Right) - Clamped to see the desk and door ONLY
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, -70f, 70f);

        // 4. Apply Rotation
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}