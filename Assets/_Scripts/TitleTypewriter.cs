using UnityEngine;
using TMPro;
using System.Collections;

public class TitleTypewriter : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public string fullText = "KASABWAT";
    public float typeSpeed = 0.15f;
    public float deleteSpeed = 0.08f;
    public float pauseTime = 2f;

    private string cursorChar = "|";

    void Start()
    {
        titleText.text = "";
        StartCoroutine(AnimateTitle());
    }

    IEnumerator AnimateTitle()
    {
        while (true)
        {
            // 1. TYPING PHASE
            for (int i = 0; i <= fullText.Length; i++)
            {
                titleText.text = fullText.Substring(0, i) + cursorChar;

                // ADD THIS: Hanapin ang audio manager at patunugin
                TerminalAudio menuAudio = FindFirstObjectByType<TerminalAudio>();
                if (menuAudio != null) menuAudio.PlayTypingSound();

                yield return new WaitForSeconds(typeSpeed);
            }

            // 2. BLINKING PAUSE (Habang buo ang text)
            float elapsed = 0;
            while (elapsed < pauseTime)
            {
                titleText.text = fullText + (titleText.text.EndsWith(cursorChar) ? "" : cursorChar);
                yield return new WaitForSeconds(0.5f); // Bilis ng blink
                elapsed += 0.5f;
            }

            // 3. DELETING PHASE
            for (int i = fullText.Length; i >= 0; i--)
            {
                titleText.text = fullText.Substring(0, i) + cursorChar;
                yield return new WaitForSeconds(deleteSpeed);
            }

            // 4. SHORT PAUSE (Bago ulitin)
            yield return new WaitForSeconds(0.5f);
        }
    }
}