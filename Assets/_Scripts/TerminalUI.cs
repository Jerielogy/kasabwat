using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TerminalUI : MonoBehaviour
{
    public TextMeshProUGUI outputField;
    public TextMeshProUGUI instructionsField;
    public TextMeshProUGUI objectiveField;
    public TextMeshProUGUI inputDisplayField;
    public TextMeshProUGUI transitionDayText;
    public Image blackOverlay;

    [Header("Attachment Panel")]
    public GameObject attachmentPanel;
    public Image photoDisplay;

    public void SetOutput(string text) => outputField.text = text;
    public void SetInstructions(string text)
    {
        if (instructionsField != null) instructionsField.text = text;
    }
    public void SetInput(string text)
    {
        if (inputDisplayField != null) inputDisplayField.text = "> " + text;
    }
    public void SetObjective(int count) => objectiveField.text = "PENDING: " + count;

    public IEnumerator ShowTransitionText(string text, float duration)
    {
        transitionDayText.text = text;
        float elapsed = 0;
        while (elapsed < 1f)
        {
            transitionDayText.color = new Color(1, 1, 1, elapsed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        transitionDayText.color = new Color(1, 1, 1, 0);
    }

    public IEnumerator FadeInBlack(float duration)
    {
        float alpha = 0;
        while (alpha < 1)
        {
            // Ang formula: $alpha += \frac{Time.deltaTime}{duration}$
            alpha += Time.deltaTime / duration;
            blackOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        float alpha = 1;
        while (alpha > 0)
        {
            // Ang formula: $alpha -= \frac{Time.deltaTime}{duration}$
            alpha -= Time.deltaTime / duration;
            blackOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    public void ShowPhoto(Sprite photo)
    {
        if (photo == null) return;
        attachmentPanel.SetActive(true);
        photoDisplay.sprite = photo;
    }

    public void HideAttachment() => attachmentPanel.SetActive(false);

}