using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Volume globalVolume;
    public KasabwatController controller;

    private DepthOfField depthOfField;
    private bool isPaused = false;
    public bool canPause = true;

    void Start()
    {
        if (controller == null)
        {
            controller = FindFirstObjectByType<KasabwatController>();
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (globalVolume != null && globalVolume.profile.TryGet(out depthOfField))
        {
            depthOfField.active = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 1. BLOCKER: Kung may transition, huwag ituloy ang kahit anong logic sa baba
            if (!canPause)
            {
                Debug.Log("Pause is currently disabled (Transition in progress).");
                return;
            }

            // 2. PRIORITY: Sticky Note logic (from previous fix)
            if (controller != null && controller.isReadingNote)
            {
                controller.ToggleNote(false);
                return;
            }

            // 3. Normal Pause/Resume
            if (isPaused) Resume();
            else Pause();
        }
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        AudioListener.pause = false;

        if (depthOfField != null)
        {
            depthOfField.active = false;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = true;

        if (depthOfField != null)
        {
            depthOfField.active = true;
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu");
    }
}