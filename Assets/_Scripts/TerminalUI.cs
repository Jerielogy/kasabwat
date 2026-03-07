using UnityEngine;
using TMPro;
using System.Collections;

public class TerminalUI : MonoBehaviour
{
    [Header("UI Text References")]
    public TMP_Text outputDisplay;
    public TMP_Text currentInputDisplay;
    public TMP_Text objectiveDisplay;
    public TMP_Text instructionDisplay;

    [Header("Transition References")]
    public CanvasGroup fadeGroup;
    public TMP_Text transitionDayText;

    public void SetOutput(string text) => outputDisplay.text = text;
    public void SetInput(string text) => currentInputDisplay.text = "> " + text + "_";
    public void SetObjective(int pending) => objectiveDisplay.text = "FILES: " + pending;
    public void SetInstructions(string text) => instructionDisplay.text = "INSTRUCTION: " + text;

    public IEnumerator FadeInBlack(float duration)
    {
        float t = 0;
        while (t < 1) { t += Time.deltaTime / duration; fadeGroup.alpha = t; yield return null; }
        fadeGroup.alpha = 1;
    }

    public IEnumerator FadeOutBlack(float duration)
    {
        float t = 1;
        while (t > 0) { t -= Time.deltaTime / duration; fadeGroup.alpha = t; yield return null; }
        fadeGroup.alpha = 0;
    }

    public IEnumerator ShowTransitionText(string content, float duration)
    {
        if (transitionDayText == null) yield break;

        // 1. Setup the text
        transitionDayText.text = content;
        transitionDayText.color = new Color(1, 1, 1, 0); // Force White & Transparent

        // 2. Fade In
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transitionDayText.color = new Color(1, 1, 1, t);
            yield return null;
        }
        transitionDayText.color = new Color(1, 1, 1, 1); // Ensure fully solid

        // 3. Hold
        yield return new WaitForSeconds(2.5f);

        // 4. Fade Out
        while (t > 0f)
        {
            t -= Time.deltaTime / duration;
            transitionDayText.color = new Color(1, 1, 1, t);
            yield return null;
        }
        transitionDayText.color = new Color(1, 1, 1, 0); // Ensure fully hidden
    }
}