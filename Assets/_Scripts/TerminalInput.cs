using UnityEngine;

public class TerminalInput : MonoBehaviour
{
    public float mouseSensitivity = 100f, interactDistance = 5f;
    private float xRot = 0f, yRot = 0f;
    private string currentTyped = "";
    private KasabwatController controller;
    private TerminalUI ui;

    void Start()
    {
        controller = FindFirstObjectByType<KasabwatController>();
        ui = FindFirstObjectByType<TerminalUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (controller == null) return;

        if (!controller.isReadingNote)
        {
            float mX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            if (controller.currentState == GameState.Day3_Breach) { xRot -= mY; yRot += mX; }
            else { xRot = Mathf.Clamp(xRot - mY, -45f, 45f); yRot = Mathf.Clamp(yRot + mX, -70f, 70f); }

            Camera.main.transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);
            if (controller.currentState == GameState.Day3_Breach && controller.movement != null)
                controller.movement.transform.rotation = Quaternion.Euler(0f, yRot, 0f);
        }

        if (controller.isReadingNote)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) controller.ToggleNote(false);
            return;
        }

        PerformHoverCheck();
        if (Input.GetMouseButtonDown(0)) PerformRaycast();
        if (IsNarrativeState()) return;

        if (controller.currentState == GameState.Awakening && Input.anyKeyDown) { controller.HandleInput(""); return; }

        foreach (char c in Input.inputString)
        {
            if (c == '\b') { if (currentTyped.Length > 0) currentTyped = currentTyped.Substring(0, currentTyped.Length - 1); }
            else if (c == '\n' || c == '\r') { if (controller.currentState == GameState.Evaluation) controller.TriggerEndOfDay(); else controller.HandleInput(currentTyped); currentTyped = ""; }
            else currentTyped += c;
        }
        ui.SetInput(currentTyped);
    }

    bool IsNarrativeState() => controller.currentState == GameState.Prologue || controller.currentState == GameState.PhoneCall || controller.currentState == GameState.Intrusion || controller.currentState == GameState.Day3_Breach;

    void PerformHoverCheck()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("StickyNote")) { ui.ShowHoverPrompt(true, "Press Left Mouse to read sticky note"); return; }
            if (hit.collider.CompareTag("Telephone") && controller.currentState == GameState.PhoneCall) { ui.ShowHoverPrompt(true, "Press Left Mouse to answer call"); return; }
        }
        ui.ShowHoverPrompt(false);
    }

    void PerformRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            if (hit.collider.CompareTag("Telephone") && controller.currentState == GameState.PhoneCall) controller.AnswerPhone();
            if (hit.collider.CompareTag("StickyNote")) controller.ToggleNote(true);
        }
    }
}