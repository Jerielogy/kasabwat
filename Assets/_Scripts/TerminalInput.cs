using UnityEngine;

public class TerminalInput : MonoBehaviour
{
    private string currentTyped = "";
    private KasabwatController controller;
    private TerminalUI ui;

    public float mouseSensitivity = 100f;
    private float xRot = 0f;
    private float yRot = 0f;
    public float interactDistance = 5.0f;

    void Start()
    {
        controller = GetComponent<KasabwatController>();
        ui = GetComponent<TerminalUI>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRot = Mathf.Clamp(xRot - mouseY, -45f, 45f);
        yRot = Mathf.Clamp(yRot + mouseX, -70f, 70f);
        Camera.main.transform.localRotation = Quaternion.Euler(xRot, yRot, 0f);

        if (Input.GetMouseButtonDown(0)) PerformRaycast();

        if (controller.currentState == GameState.Prologue ||
            controller.currentState == GameState.PhoneCall ||
            controller.currentState == GameState.StoryCall ||
            controller.currentState == GameState.Intrusion) 
        { 
            return; 
        }

        if (controller.currentState == GameState.Awakening && Input.anyKeyDown)
        {
            controller.HandleInput("");
            return;
        }

        foreach (char c in Input.inputString)
        {
            if (c == '\b') { if (currentTyped.Length > 0) currentTyped = currentTyped.Substring(0, currentTyped.Length - 1); }
            else if (c == '\n' || c == '\r')
            {
                if (controller.currentState == GameState.Evaluation) controller.TriggerEndOfDay();
                else controller.HandleInput(currentTyped);
                currentTyped = "";
            }
            else { currentTyped += c; }
        }
        ui.SetInput(currentTyped);
    }

    void PerformRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Telephone") && controller.currentState == GameState.PhoneCall)
            {
                controller.AnswerPhone();
            }
        }
    }
}