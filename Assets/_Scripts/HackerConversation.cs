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
        isAwaitingInput = false;
        hackerDisplayText.text = "";
        foreach (char c in message.ToCharArray()) { hackerDisplayText.text += c; yield return new WaitForSeconds(0.03f); }
        hackerDisplayText.text += "\n\n<color=yellow>/ask 1: " + conversationSteps[currentStep].option1 + "</color>\n<color=yellow>/ask 2: " + conversationSteps[currentStep].option2 + "</color>\n<color=yellow>/ask 3: " + conversationSteps[currentStep].option3 + "</color>";
        isAwaitingInput = true;
    }

    public void ReceiveInput(string input)
    {
        if (!isAwaitingInput) return;
        string response = "";
        if (input == "/ask 1") response = conversationSteps[currentStep].response1;
        else if (input == "/ask 2") response = conversationSteps[currentStep].response2;
        else if (input == "/ask 3") response = conversationSteps[currentStep].response3;
        else return;
        StartCoroutine(HandleResponse(response));
    }

    IEnumerator HandleResponse(string response)
    {
        isAwaitingInput = false;
        hackerDisplayText.text = "SYSTEM: ENCRYPTING RESPONSE...";
        yield return new WaitForSeconds(1f);
        hackerDisplayText.text = "REYES: " + response;
        yield return new WaitForSeconds(4f);
        currentStep++;
        if (currentStep < conversationSteps.Count) StartCoroutine(TypeHackerMessage(conversationSteps[currentStep].hackerStatement));
        else EndBreach();
    }

    void EndBreach()
    {
        hackerPanel.SetActive(false);
        mainController.TriggerEndOfDay();
    }
}