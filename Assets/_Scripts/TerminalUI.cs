using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TerminalUI : MonoBehaviour
{
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI instructionField;
    public TextMeshProUGUI objectiveField;

    // BACK TO NORMAL: Just a simple text display
    public TextMeshProUGUI inputField;

    public GameObject stickyNotePanel;
    public GameObject hoverPrompt;
    public TextMeshProUGUI hoverText;

    public Image blackOverlay;
    public TextMeshProUGUI transitionText;

    public GameObject attachmentPanel;
    public Image photoDisplay;

    public PauseManager pauseManager;

    // CLEAN START: No more listeners fighting your TerminalInput script
    void Start()
    {
        if (inputField != null) inputField.text = "";
        if (pauseManager == null) pauseManager = FindFirstObjectByType<PauseManager>();
    }

    public void SetOutput(string text) => outputField.text = text;
    public void SetInstructions(string text) => instructionField.text = text;
    public void SetObjective(int count) => objectiveField.text = "PENDING FILES: " + count;

    // THIS IS THE BRIDGE: TerminalInput calls this to show your typing
    public void SetInput(string text)
    {
        if (inputField != null) inputField.text = text;
    }

    public void ShowPhoto(Sprite photo)
    {
        attachmentPanel.SetActive(true);
        photoDisplay.sprite = photo;
    }

    public void HideAttachment() => attachmentPanel.SetActive(false);

    public void ShowHoverPrompt(bool show, string message = "")
    {
        hoverPrompt.SetActive(show);
        if (show && hoverText != null) hoverText.text = message;
    }

    public IEnumerator ShowTransitionText(string text, float duration)
    {
        // 1. I-lock ang pause bago lumabas ang transition text
        if (pauseManager != null)
        {
            pauseManager.canPause = false;
        }

        transitionText.text = text;
        transitionText.gameObject.SetActive(true);

        // Dito hihinto ang laro habang naka-display ang "Day 1" etc.
        yield return new WaitForSeconds(duration);

        transitionText.gameObject.SetActive(false);

        // 2. I-unlock ang pause dahil tapos na ang transition
        if (pauseManager != null)
        {
            pauseManager.canPause = true;
        }
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackOverlay.color = new Color(0, 0, 0, 1 - (elapsed / duration));
            yield return null;
        }
        blackOverlay.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeInBlack(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            blackOverlay.color = new Color(0, 0, 0, elapsed / duration);
            yield return null;
        }
        blackOverlay.color = new Color(0, 0, 0, 1);
    }
}