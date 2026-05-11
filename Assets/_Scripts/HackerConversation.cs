using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    void Start() => mainController = FindFirstObjectByType<KasabwatController>();

    public void StartBreach()
    {
        hackerPanel.SetActive(true);
        currentStep = 0;
        StartCoroutine(TypeHackerMessage(conversationSteps[currentStep].hackerStatement));
    }

    IEnumerator TypeHackerMessage(string message)
    {
        isAwaitingInput = false; // LOCK INPUT while Reyes is typing
        hackerDisplayText.text = "";

        // REYES TYPING...
        foreach (char c in message.ToCharArray())
        {
            hackerDisplayText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        // Display Options
        hackerDisplayText.text += "\n\n<color=yellow>/ask 1: " + conversationSteps[currentStep].option1 + "</color>";
        hackerDisplayText.text += "\n<color=yellow>/ask 2: " + conversationSteps[currentStep].option2 + "</color>";
        hackerDisplayText.text += "\n<color=yellow>/ask 3: " + conversationSteps[currentStep].option3 + "</color>";

        isAwaitingInput = true; // UNLOCK INPUT
    }

    public void ReceiveInput(string input)
    {
        // 1. Check if we are even allowed to type (No spamming)
        if (!isAwaitingInput) return;

        // 2. Clean the input (Case-insensitive)
        string cmd = input.ToLower().Trim();
        string chosenOptionText = "";
        string responseText = "";

        // 3. Logic for choosing the question
        if (cmd == "/ask 1")
        {
            chosenOptionText = conversationSteps[currentStep].option1;
            responseText = conversationSteps[currentStep].response1;
        }
        else if (cmd == "/ask 2")
        {
            chosenOptionText = conversationSteps[currentStep].option2;
            responseText = conversationSteps[currentStep].response2;
        }
        else if (cmd == "/ask 3")
        {
            chosenOptionText = conversationSteps[currentStep].option3;
            responseText = conversationSteps[currentStep].response3;
        }
        else return; // If they typed something else, ignore it.

        // 4. Start the response sequence
        StartCoroutine(HandleResponse(chosenOptionText, responseText));
    }

    IEnumerator HandleResponse(string playerQuestion, string hackerResponse)
    {
        isAwaitingInput = false; // LOCK INPUT (Busy Flag)

        // SHOW PAUL'S QUESTION (This fixes the "not displaying" issue)
        hackerDisplayText.text = "PAUL: " + playerQuestion;
        yield return new WaitForSeconds(1f);

        // ENCRYPTION EFFECT
        hackerDisplayText.text += "...";
        yield return new WaitForSeconds(1.5f);

        // SHOW REYES' RESPONSE
        hackerDisplayText.text = "REYES: " + hackerResponse;
        yield return new WaitForSeconds(4f);

        // Move to next step or end
        currentStep++;
        if (currentStep < conversationSteps.Count)
        {
            StartCoroutine(TypeHackerMessage(conversationSteps[currentStep].hackerStatement));
        }
        else
        {
            EndBreach();
        }
    }

    void EndBreach()
    {
        hackerPanel.SetActive(false);
        mainController.TriggerEndOfDay();
    }
}