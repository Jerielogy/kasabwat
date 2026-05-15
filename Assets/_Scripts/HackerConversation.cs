using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// 1. THIS IS THE CLASS THE COMPILER IS LOOKING FOR
[System.Serializable]
public class DialogueStep
{
    [TextArea(3, 10)] public string hackerStatement;
    public string option1, option2, option3, response1, response2, response3;
}

public class HackerConversation : MonoBehaviour
{
    public GameObject hackerPanel;
    public TextMeshProUGUI hackerDisplayText;
    public List<DialogueStep> conversationSteps;

    private int currentStep = 0;
    private bool isAwaitingInput = false;
    private KasabwatController mainController;
    private string currentReyesMessage = ""; // Stores the current Reyes text

    void Start() => mainController = FindFirstObjectByType<KasabwatController>();

    // 2. THIS IS THE METHOD TERMINALINPUT IS LOOKING FOR
    public void UpdateLiveTyping(string text)
    {
        if (!isAwaitingInput) return;

        // Show Reyes' message + the options + what Paul is typing right now
        hackerDisplayText.text = currentReyesMessage + "\n\n> " + text;
    }

    public void StartBreach()
    {
        hackerPanel.SetActive(true);
        currentStep = 0;
        StartCoroutine(TypeHackerMessage(conversationSteps[currentStep].hackerStatement));
    }

    IEnumerator TypeHackerMessage(string message)
    {
        isAwaitingInput = false;
        currentReyesMessage = "";
        hackerDisplayText.text = "";

        foreach (char c in message.ToCharArray())
        {
            currentReyesMessage += c;
            hackerDisplayText.text = currentReyesMessage;
            yield return new WaitForSeconds(0.03f);
        }

        // Add the options to the base message
        currentReyesMessage += "\n\n<color=yellow>/ask 1: " + conversationSteps[currentStep].option1 + "</color>";
        currentReyesMessage += "\n<color=yellow>/ask 2: " + conversationSteps[currentStep].option2 + "</color>";
        currentReyesMessage += "\n<color=yellow>/ask 3: " + conversationSteps[currentStep].option3 + "</color>";

        hackerDisplayText.text = currentReyesMessage + "\n\n> ";
        isAwaitingInput = true;
    }

    public void ReceiveInput(string input)
    {
        if (!isAwaitingInput) return;

        string cmd = input.ToLower().Trim();
        string responseText = "";

        if (cmd == "/ask 1") responseText = conversationSteps[currentStep].response1;
        else if (cmd == "/ask 2") responseText = conversationSteps[currentStep].response2;
        else if (cmd == "/ask 3") responseText = conversationSteps[currentStep].response3;
        else return;

        StartCoroutine(HandleResponse(responseText));
    }

    IEnumerator HandleResponse(string hackerResponse)
    {
        isAwaitingInput = false;

        // Immediately clear the input line for that "encrypted" transition
        hackerDisplayText.text = "REYES: [DECRYPTING RESPONSE...]";
        yield return new WaitForSeconds(1.5f);

        hackerDisplayText.text = "REYES: " + hackerResponse;
        yield return new WaitForSeconds(4f);

        currentStep++;
        if (currentStep < conversationSteps.Count)
            StartCoroutine(TypeHackerMessage(conversationSteps[currentStep].hackerStatement));
        else
            EndBreach();
    }

    void EndBreach()
    {
        hackerPanel.SetActive(false);
        mainController.TriggerEndOfDay();
    }
}