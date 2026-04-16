using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Prologue, Awakening, Login_User, Login_Pass, Intro, Mailbox, Viewing, Evaluation, PhoneCall, StoryCall, Intrusion, Day3_Breach }

public class KasabwatController : MonoBehaviour
{
    public GameState currentState;
    public Light officeLight;
    public KasabwatMovement movement;
    public bool isReadingNote = false;

    private TerminalUI ui;
    private EmailDatabase db;
    private TerminalAudio audioCtrl;
    private List<Email> inbox;
    private int currentDay = 1, errors = 0, processedToday = 0, selectedEmailIndex = -1;
    private AudioClip pendingEvalClip;

    void Start()
    {
        ui = GetComponent<TerminalUI>(); db = GetComponent<EmailDatabase>();
        audioCtrl = GetComponent<TerminalAudio>(); LoadDay(1);
        StartCoroutine(StartGameSequence());
    }

    public void LoadDay(int day)
    {
        currentDay = day; errors = 0; processedToday = 0;
        inbox = (day == 3) ? new List<Email>() : db.GetDayContent(day);
        if (day == 3 && officeLight) officeLight.intensity = 0.1f;
        ui.SetObjective(inbox.Count);
    }

    // FIXED: Day 1 Intro now waits for telephone interaction
    IEnumerator StartGameSequence()
    {
        currentState = GameState.Prologue;
        yield return ui.ShowTransitionText("DAY 1: THE COMPLIANCE", 1.5f);
        yield return ui.FadeOutBlack(1.5f);

        currentState = GameState.PhoneCall;
        audioCtrl.SetRingtone(true); // START RINGTONE

        ui.SetOutput("(Telephone ringing... Click the phone to answer.)");
        ui.SetInstructions("INCOMING: PARE");
        yield break;
    }

    public void HandleInput(string input)
    {
        if (currentState == GameState.Awakening) { currentState = GameState.Login_User; UpdateUI(); return; }
        string cmd = input.ToLower().Trim();
        if (currentState == GameState.Login_User && cmd == "admin") { currentState = GameState.Login_Pass; UpdateUI(); }
        else if (currentState == GameState.Login_Pass && cmd == "password") { currentState = GameState.Intro; UpdateUI(); }
        else if (currentState == GameState.Intro) { currentState = GameState.Mailbox; UpdateUI(); }
        else if (currentState == GameState.Mailbox)
        {
            if (cmd.StartsWith("/view ")) { if (int.TryParse(cmd.Replace("/view ", ""), out int idx)) ViewEmail(idx - 1); }
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

    void ViewEmail(int idx)
    {
        if (idx >= 0 && idx < inbox.Count && !inbox[idx].isProcessed)
        {
            selectedEmailIndex = idx; currentState = GameState.Viewing; UpdateUI();
            string c = "SUBJECT: " + inbox[idx].subject + "\n----------------\n" + inbox[idx].body;
            if (!string.IsNullOrEmpty(inbox[idx].attachmentName)) c += "\n\n[ATTACHMENT: " + inbox[idx].attachmentName + "]";
            ui.SetOutput(c);
        }
    }

    void ProcessEmail(bool del)
    {
        inbox[selectedEmailIndex].isProcessed = true; processedToday++;
        if (inbox[selectedEmailIndex].isSensitive != del) errors++;
        if (currentDay == 2 && processedToday == 5) StartCoroutine(IntrusionSequence());
        else { ui.SetOutput("FILE PROCESSED.\n\nTYPE /close TO RETURN."); ui.SetObjective(GetPendingCount()); }
    }

    public void ToggleNote(bool show)
    {
        isReadingNote = show; ui.stickyNotePanel.SetActive(show);
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
        if (show) ui.ShowHoverPrompt(false);
    }

    void ShowInbox()
    {
        string list = "INBOX - DAY " + currentDay + "\n----------------\n";
        for (int i = 0; i < inbox.Count; i++) list += (i + 1) + ". " + (inbox[i].isProcessed ? "[X] " : "[ ] ") + inbox[i].subject + "\n";
        ui.SetOutput(list);
    }

    public void UpdateUI()
    {
        switch (currentState)
        {
            case GameState.Login_User: ui.SetInstructions("ENTER USERNAME (read sticky note)"); break;
            case GameState.Login_Pass: ui.SetInstructions("ENTER PASSWORD (read sticky note)"); break;
            case GameState.Intro: ui.SetInstructions("PRESS ENTER TO START"); break;
            case GameState.Mailbox: ShowInbox(); ui.SetInstructions("TYPE: /view [number]  |  /logout"); break;
            case GameState.Viewing: ui.SetInstructions("TYPE: /keep  |  /delete  |  /open  |  /close"); break;
            case GameState.Evaluation: ui.SetInstructions("STATION OFFLINE."); break;
        }
    }

    IEnumerator IntrusionSequence()
    {
        currentState = GameState.Intrusion; ui.HideAttachment(); audioCtrl.PlayClip(audioCtrl.IntrusionSFX);
        float e = 0; while (e < 6f)
        {
            ui.outputField.gameObject.transform.parent.gameObject.SetActive(Random.value > 0.5f);
            ui.SetOutput(Random.value > 0.5f ? "SYSTEM COMPROMISED" : "INTRUDER ALERT");
            ui.outputField.color = Color.red; yield return new WaitForSeconds(Random.Range(0.05f, 0.2f)); e += 0.1f;
        }
        ui.outputField.gameObject.transform.parent.gameObject.SetActive(true); ui.outputField.color = Color.white;
        ui.SetOutput("SYSTEM RESTORED...\n\nTYPE /close TO RETURN."); currentState = GameState.Viewing;
    }

    public void TriggerEndOfDay() => StartCoroutine(PhoneCallSequence());

    IEnumerator PhoneCallSequence()
    {
        currentState = GameState.PhoneCall;
        pendingEvalClip = (errors > 0) ? audioCtrl.badCall : audioCtrl.goodCall;

        audioCtrl.SetRingtone(true); // START RINGTONE para sa evaluation

        ui.SetOutput("SYSTEM SHUTDOWN.\n\n(Incoming call... Click the phone.)");
        ui.SetInstructions("INCOMING: MAYOR");
        yield break;
    }

    // FIXED: Handles Day 1 Intro, Day 2 Intro, and Evaluation calls
    public void AnswerPhone()
    {
        if (currentState == GameState.PhoneCall)
        {
            audioCtrl.SetRingtone(false); // STOP RINGTONE kapag sinagot na

            if (pendingEvalClip != null) StartCoroutine(ExecuteEvaluationCall());
            else if (currentDay == 1) StartCoroutine(Day1IntroCallSequence());
            else if (currentDay == 2) StartCoroutine(Day2PhoneCallSequence());
        }
    }

    IEnumerator Day1IntroCallSequence()
    {
        audioCtrl.PlayClip(audioCtrl.friendDay1);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay1));
        ui.SetOutput("SYSTEM READY.\n\nPRESS ANY KEY TO LOGIN");
        currentState = GameState.Awakening;
        UpdateUI();
    }

    IEnumerator ExecuteEvaluationCall()
    {
        audioCtrl.PlayClip(pendingEvalClip); yield return new WaitForSeconds(audioCtrl.GetLength(pendingEvalClip));
        pendingEvalClip = null;
        if (currentDay == 1)
        {
            yield return ui.FadeInBlack(2f); LoadDay(2);
            yield return ui.ShowTransitionText("DAY 2: THE RAT", 2f);
            yield return ui.FadeOutBlack(2f);
            ui.SetOutput("STATION 8802: ACTIVE\n\n(Click phone for Intro Call.)");
            currentState = GameState.PhoneCall; // Wait for Pare's Day 2 call
        }
        else if (currentDay == 2)
        {
            yield return ui.FadeInBlack(2f); LoadDay(3);
            yield return ui.ShowTransitionText("DAY 3: THE DECISION", 2f);
            yield return ui.FadeOutBlack(4f); StartCoroutine(Day3BreachSequence());
        }
    }

    IEnumerator Day3BreachSequence()
    {
        currentState = GameState.Day3_Breach; ui.SetOutput("CRITICAL ERROR: SYSTEM TERMINATED.");
        audioCtrl.PlayClip(audioCtrl.Day2EndingCall); yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.Day2EndingCall));
        ui.SetInstructions("STAND UP. ESCAPE."); if (movement) movement.enabled = true;
    }

    IEnumerator Day2PhoneCallSequence() { audioCtrl.PlayClip(audioCtrl.friendDay2); yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay2)); currentState = GameState.Intro; UpdateUI(); }
    void OpenAttachment() { Email e = inbox[selectedEmailIndex]; if (e.photoAttachment) ui.ShowPhoto(e.photoAttachment); if (e.audioAttachment) audioCtrl.PlayClip(e.audioAttachment); }
    void TryLogout() { foreach (var e in inbox) if (!e.isProcessed) return; currentState = GameState.Evaluation; UpdateUI(); }
    int GetPendingCount() { int c = 0; foreach (var e in inbox) if (!e.isProcessed) c++; return c; }
}