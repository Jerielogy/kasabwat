using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for the Button

public class IntroDirector : MonoBehaviour
{
    public TextMeshProUGUI narrativeDisplay;
    public float typingSpeed = 0.07f;
    public float timeBetweenSentences = 3.5f;
    public string nextScene = "MainScene";

    [Header("Audio Settings")]
    public AudioSource typingSource;
    public AudioClip typingKeySound;
    [Range(0.1f, 1f)] public float volume = 0.5f;

    [Header("The Paged Narrative")]
    [TextArea(3, 10)]
    public string[] introSentences;

    // Call this via a Button's OnClick event
    public void SkipIntro()
    {
        StopAllCoroutines(); // Kill the typing process
        SceneManager.LoadScene(nextScene); // Jump straight to the office
    }

    void Start()
    {
        narrativeDisplay.text = "";
        StartCoroutine(BeginSequence());
    }

    IEnumerator BeginSequence()
    {
        yield return new WaitForSeconds(2.0f);

        foreach (string sentence in introSentences)
        {
            narrativeDisplay.text = "";

            foreach (char c in sentence.ToCharArray())
            {
                narrativeDisplay.text += c;

                if (typingSource != null && typingKeySound != null)
                {
                    typingSource.pitch = Random.Range(0.85f, 1.15f);
                    typingSource.PlayOneShot(typingKeySound, volume);
                }

                yield return new WaitForSeconds(typingSpeed);

                // This still allows skipping just the *typing* of the current sentence
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    narrativeDisplay.text = sentence;
                    break;
                }
            }

            yield return new WaitForSeconds(timeBetweenSentences);
        }

        SceneManager.LoadScene(nextScene);
    }
}