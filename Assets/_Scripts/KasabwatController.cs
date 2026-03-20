using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Prologue, Awakening, Login_User, Login_Pass, Intro, Mailbox, Viewing, Evaluation, PhoneCall, StoryCall, Intrusion }

public class KasabwatController : MonoBehaviour
{
    public GameState currentState;
    private TerminalUI ui;
    private EmailDatabase db;
    private TerminalAudio audioCtrl;

    private List<Email> inbox;
    private int currentDay = 1;
    private int errors = 0;
    private int selectedEmailIndex = -1;
    private int processedToday = 0;

    void Start()
    {
        ui = GetComponent<TerminalUI>();
        db = GetComponent<EmailDatabase>();
        audioCtrl = GetComponent<TerminalAudio>();
        LoadDay(1);
        StartCoroutine(StartGameSequence());
    }

    public void LoadDay(int day)
    {
        currentDay = day;
        errors = 0;
        processedToday = 0; // Reset counter every morning
        inbox = db.GetDayContent(day);
        ui.SetObjective(GetPendingCount());
    }

    IEnumerator StartGameSequence()
    {
        currentState = GameState.Prologue;
        ui.transitionDayText.color = new Color(1, 1, 1, 0);
        yield return ui.ShowTransitionText("THE COMPLIANCE", 1.5f);
        yield return ui.FadeOutBlack(1.5f);

        currentState = GameState.StoryCall;
        ui.SetOutput("STATION 8802: INITIALIZING...\n\n(Incoming Call: Pare)");

        // PLAY AND WAIT FOR ACTUAL LENGTH
        audioCtrl.PlayClip(audioCtrl.friendDay1);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay1));

        ui.SetOutput("SYSTEM READY.\n\nPRESS ANY KEY TO LOGIN");
        currentState = GameState.Awakening;
        UpdateUI();
    }

    public void HandleInput(string input)
    {
        if (currentState == GameState.Awakening) { currentState = GameState.Login_User; UpdateUI(); return; }
        string cmd = input.ToLower().Trim();

        if (currentState == GameState.Login_User && cmd == "admin") { currentState = GameState.Login_Pass; UpdateUI(); }
        else if (currentState == GameState.Login_Pass)
        {
            if (cmd == "password") { currentState = GameState.Intro; UpdateUI(); }
            else { currentState = GameState.Login_User; UpdateUI(); }
        }
        else if (currentState == GameState.Intro) { currentState = GameState.Mailbox; UpdateUI(); }
        else if (currentState == GameState.Mailbox)
        {
            if (cmd.StartsWith("/view "))
            {
                if (int.TryParse(cmd.Replace("/view ", ""), out int idx)) ViewEmail(idx - 1);
            }
            else if (cmd == "/logout") TryLogout();
        }
        else if (currentState == GameState.Viewing)
        {
            if (cmd == "/delete") ProcessEmail(true);
            else if (cmd == "/keep") ProcessEmail(false);
            else if (cmd == "/open") OpenAttachment();
            else if (cmd == "/close") { ui.HideAttachment(); currentState = GameState.Mailbox; UpdateUI(); }
        }
    }

    public void UpdateUI()
    {
        switch (currentState)
        {
            case GameState.Login_User:
                ui.SetOutput("KASABWAT OS v1.1\n\nENTER USERNAME:");
                ui.SetInstructions("TYPE 'admin'");
                break;
            case GameState.Login_Pass:
                ui.SetOutput("KASABWAT OS v1.1\nUSER: admin\n\nENTER PASSWORD:");
                ui.SetInstructions("TYPE 'password'");
                break;
            case GameState.Intro:
                ui.SetOutput("--- DAY " + currentDay + " BRIEFING ---\n\nOBJECTIVE: Maintain system integrity.");
                ui.SetInstructions("PRESS ENTER TO START");
                break;
            case GameState.Mailbox:
                ShowMailbox();
                ui.SetInstructions("TYPE '/view [number]' OR '/logout'");
                break;
            case GameState.Viewing:
                ui.SetInstructions("TYPE '/delete' OR '/keep'. USE '/open' FOR ATTACHMENTS.");
                break;
            case GameState.PhoneCall:
                ui.SetInstructions("PHYSICAL ACTION: CLICK THE TELEPHONE");
                break;
            case GameState.Evaluation:
                ui.SetOutput("--- DAY " + currentDay + " EVALUATION ---\n\nERRORS: " + errors + "\n\nPRESS ENTER TO LOGOUT");
                ui.SetInstructions("PRESS ENTER TO FINISH DAY");
                break;
        }
    }

    void ShowMailbox()
    {
        string list = "--- INBOX: DAY " + currentDay + " ---\n";
        for (int i = 0; i < inbox.Count; i++)
        {
            string status = inbox[i].isProcessed ? "[DONE]" : "[NEW ]";
            list += (i + 1) + ". " + status + " " + inbox[i].subject + "\n";
        }
        ui.SetOutput(list);
    }

    void ViewEmail(int index)
    {
        if (index >= 0 && index < inbox.Count && !inbox[index].isProcessed)
        {
            selectedEmailIndex = index;
            currentState = GameState.Viewing;
            UpdateUI();

            // Construct the email display string
            string content = "SUBJECT: " + inbox[index].subject + "\n----------------\n" + inbox[index].body;

            // NEW: If an attachment name exists, append it to the bottom
            if (!string.IsNullOrEmpty(inbox[index].attachmentName))
            {
                content += "\n\n[ATTACHMENT FOUND: " + inbox[index].attachmentName + "]";
            }

            ui.SetOutput(content);
        }
    }

    void ProcessEmail(bool deleteChoice)
    {
        inbox[selectedEmailIndex].isProcessed = true;
        processedToday++; // Track progress

        if (inbox[selectedEmailIndex].isSensitive != deleteChoice) errors++;

        // TRIGGER INTRUSION ON DAY 2 AFTER 5 EMAILS
        if (currentDay == 2 && processedToday == 5)
        {
            StartCoroutine(IntrusionSequence());
        }
        else
        {
            ui.SetOutput("FILE PROCESSED.\n\nTYPE /close TO RETURN.");
            ui.SetObjective(GetPendingCount());
        }
    }

    void OpenAttachment()
    {
        if (selectedEmailIndex < 0) return;
        Email current = inbox[selectedEmailIndex];

        // Check if the email actually has something to open
        if (current.photoAttachment == null && current.audioAttachment == null)
        {
            ui.SetOutput("SYSTEM ERROR: NO ATTACHED MEDIA FOUND IN THIS FILE.");
            return;
        }

        if (current.photoAttachment != null) ui.ShowPhoto(current.photoAttachment);
        if (current.audioAttachment != null) audioCtrl.PlayClip(current.audioAttachment);
    }

    void TryLogout()
    {
        foreach (var m in inbox) if (!m.isProcessed) return;
        currentState = GameState.Evaluation;
        UpdateUI();
    }

    public void TriggerEndOfDay() { StartCoroutine(PhoneCallSequence()); }

    public void AnswerPhone() { StartCoroutine(Day2PhoneCallSequence()); }

    IEnumerator Day2PhoneCallSequence()
    {
        ui.SetOutput("(Incoming Call: PARE)\n\n'Pare is calling, answer using Telephone!'");

        // PLAY AND WAIT FOR ACTUAL LENGTH
        audioCtrl.PlayClip(audioCtrl.friendDay2);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay2));

        currentState = GameState.Intro;
        UpdateUI();
    }

    IEnumerator PhoneCallSequence()
    {
        currentState = GameState.PhoneCall;
        ui.SetOutput("SHUTTING DOWN...\n\n(Incoming: MAYOR)");

        // Determine which clip to play based on errors
        AudioClip evalClip = (errors > 0) ? audioCtrl.badCall : audioCtrl.goodCall;

        // PLAY AND WAIT FOR ACTUAL LENGTH
        audioCtrl.PlayClip(evalClip);
        yield return new WaitForSeconds(audioCtrl.GetLength(evalClip));

        if (currentDay == 1)
        {
            if (audioCtrl.officeSpeaker != null) audioCtrl.officeSpeaker.Stop();
            yield return ui.FadeInBlack(1f);

            float glitchDuration = 2.5f;
            float elapsed = 0f;
            while (elapsed < glitchDuration)
            {
                bool isRat = Random.value > 0.7f;
                ui.transitionDayText.text = isRat ? "THE RAT" : "DAY 2";
                ui.transitionDayText.color = isRat ? Color.red : Color.white;
                float wait = Random.Range(0.05f, 0.12f);
                elapsed += wait;
                yield return new WaitForSeconds(wait);
            }

            ui.transitionDayText.color = new Color(1, 1, 1, 0);
            ui.transitionDayText.text = "";

            LoadDay(2);
            yield return ui.FadeOutBlack(1.0f);
            ui.blackOverlay.color = new Color(0, 0, 0, 0);

            currentState = GameState.PhoneCall;
            ui.SetOutput("STATION 8802: ACTIVE\n\n(The telephone is ringing. Click the cube.)");
            UpdateUI();
        }
    }

    int GetPendingCount()
    {
        int c = 0;
        foreach (var m in inbox) if (!m.isProcessed) c++;
        return c;
    }
    IEnumerator IntrusionSequence()
    {
        currentState = GameState.Intrusion;
        ui.HideAttachment(); // Close any open photos

        float duration = 8f; // Lasts 8 seconds
        float elapsed = 0;

        while (elapsed < duration)
        {
            // Toggle the terminal panel on and off for a flickering effect
            bool toggle = Random.value > 0.5f;
            ui.outputField.gameObject.transform.parent.gameObject.SetActive(toggle);

            // Randomly switch between warning messages
            ui.SetOutput(Random.value > 0.5f ? "SYSTEM COMPROMISED" : "INTRUDER ALERT");
            ui.outputField.color = Color.red;

            float wait = Random.Range(0.05f, 0.2f);
            elapsed += wait;
            yield return new WaitForSeconds(wait);
        }

        // Restore Normalcy
        ui.outputField.gameObject.transform.parent.gameObject.SetActive(true);
        ui.outputField.color = Color.white;
        ui.SetOutput("SYSTEM RESTORED...\n\n(Evidence of intrusion purged.)\n\nTYPE /close TO RETURN.");

        // Briefly stay in Intrusion state to let them read, then go back to Viewing
        yield return new WaitForSeconds(2f);
        currentState = GameState.Viewing;
    }
}