using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TerminalUI : MonoBehaviour
{
    public TextMeshProUGUI outputField, instructionsField, objectiveField, inputDisplayField, transitionDayText;
    public UnityEngine.UI.Image blackOverlay;

    [Header("Panels")]
    public GameObject attachmentPanel;
    public UnityEngine.UI.Image photoDisplay;
    public GameObject hoverPrompt, stickyNotePanel;

    public void SetOutput(string text) => outputField.text = text;
    public void SetInstructions(string text) => instructionsField.text = text;
    public void SetInput(string text) => inputDisplayField.text = "> " + text;
    public void SetObjective(int count) => objectiveField.text = "PENDING: " + count;

    public IEnumerator ShowTransitionText(string text, float duration)
    {
        transitionDayText.text = text;
        transitionDayText.color = new Color(1, 1, 1, 0);
        float elapsed = 0;
        while (elapsed < 1f)
        {
            transitionDayText.color = new Color(1, 1, 1, elapsed);
            elapsed += Time.deltaTime; yield return null;
        }
        yield return new WaitForSeconds(duration);
        float fadeOut = 1f;
        while (fadeOut > 0f)
        {
            fadeOut -= Time.deltaTime;
            transitionDayText.color = new Color(1, 1, 1, fadeOut); yield return null;
        }
    }

    public IEnumerator FadeInBlack(float duration)
    {
        float a = 0;
        while (a < 1) { a += Time.deltaTime / duration; blackOverlay.color = new Color(0, 0, 0, a); yield return null; }
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        float a = 1;
        while (a > 0) { a -= Time.deltaTime / duration; blackOverlay.color = new Color(0, 0, 0, a); yield return null; }
    }

    public void ShowPhoto(Sprite p) { if (p) { attachmentPanel.SetActive(true); photoDisplay.sprite = p; } }
    public void HideAttachment() => attachmentPanel.SetActive(false);
    public void ShowHoverPrompt(bool s, string m = "")
    {
        hoverPrompt.SetActive(s);
        TextMeshProUGUI t = hoverPrompt.GetComponentInChildren<TextMeshProUGUI>();
        if (s && t != null) t.text = m;
    }
}