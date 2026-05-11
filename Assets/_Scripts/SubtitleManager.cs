using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel; // Optional: A semi-transparent background

    private Coroutine currentSubtitleCoroutine;

    void Start()
    {
        ClearSubtitles();
    }

    public void DisplaySubtitle(string text, float duration)
    {
        if (currentSubtitleCoroutine != null) StopCoroutine(currentSubtitleCoroutine);
        currentSubtitleCoroutine = StartCoroutine(ShowSubtitleRoutine(text, duration));
    }

    IEnumerator ShowSubtitleRoutine(string text, float duration)
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(true);
        subtitleText.text = text;

        yield return new WaitForSeconds(duration);

        ClearSubtitles();
    }

    public void ClearSubtitles()
    {
        subtitleText.text = "";
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }
}