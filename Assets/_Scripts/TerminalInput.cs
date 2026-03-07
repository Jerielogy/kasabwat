using UnityEngine;

public class TerminalInput : MonoBehaviour
{
    private string currentTyped = "";
    private KasabwatController controller;
    private TerminalUI ui;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    private float xRot = 0f, yRot = 0f;

    void Start()
    {
        controller = GetComponent<KasabwatController>();
        ui = GetComponent<TerminalUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Clamped Camera Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRot = Mathf.Clamp(xRot - mouseY, -45f, 45f);
        yRot = Mathf.Clamp(yRot + mouseX, -70f, 70f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);

        // 2. Typing Logic
        if (controller.currentState == KasabwatController.GameState.Prologue ||
            controller.currentState == KasabwatController.GameState.PhoneCall ||
            controller.currentState == KasabwatController.GameState.StoryCall) return;

        if (controller.currentState == KasabwatController.GameState.Awakening && Input.anyKeyDown)
        {
            controller.HandleInput(""); // Triggers login
            return;
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b') { if (currentTyped.Length > 0) currentTyped = currentTyped.Substring(0, currentTyped.Length - 1); }
            else if (c == '\n' || c == '\r')
            {
                if (controller.currentState == KasabwatController.GameState.Evaluation) controller.TriggerEndOfDay();
                else controller.HandleInput(currentTyped);
                currentTyped = "";
            }
            else { currentTyped += c; }
        }
        ui.SetInput(currentTyped);
    }
}