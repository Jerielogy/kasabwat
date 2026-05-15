using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState { Prologue, Awakening, Login_User, Login_Pass, Intro, Mailbox, Viewing, Evaluation, PhoneCall, StoryCall, Intrusion, Day3_Breach }

public class KasabwatController : MonoBehaviour
{
    public GameState currentState;
    public Light officeLight;
    public KasabwatMovement movement;
    public HackerConversation hackerComp;
    public bool isReadingNote = false;

    private TerminalUI ui;
    private EmailDatabase db;
    private TerminalAudio audioCtrl;
    private List<Email> inbox;
    private int currentDay = 1, errors = 0, processedToday = 0, selectedEmailIndex = -1;
    private AudioClip pendingEvalClip;
    public SubtitleManager subManager;

    private bool isPhoneCallActive = false;
    void Start()
    {
        ui = GetComponent<TerminalUI>();
        db = GetComponent<EmailDatabase>();
        audioCtrl = GetComponent<TerminalAudio>();
        hackerComp = FindFirstObjectByType<HackerConversation>();
        LoadDay(1);
        StartCoroutine(StartGameSequence());
    }

    public void LoadDay(int day)
    {
        currentDay = day;
        errors = 0;
        processedToday = 0;
        inbox = db.GetDayContent(day);
        ui.SetObjective(inbox.Count);
        if (movement) movement.enabled = false;
        if (day == 3 && officeLight) StartCoroutine(Day3FlickerLight());
    }

    IEnumerator StartGameSequence()
    {
        currentState = GameState.Prologue;
        yield return ui.ShowTransitionText("DAY 1: THE COMPLIANCE", 1.5f);
        yield return ui.FadeOutBlack(1.5f);
        currentState = GameState.PhoneCall;
        audioCtrl.SetRingtone(true);
        ui.SetOutput("(Telephone ringing... Click the phone to answer.)");
        ui.SetInstructions("INCOMING: PARE");
    }

    public void HandleInput(string input)
    {
        string cmd = input.ToLower().Trim();
        if (currentState == GameState.Intrusion)
        {
            if (hackerComp != null) hackerComp.ReceiveInput(cmd);
            return;
        }

        if (currentState == GameState.Awakening) { currentState = GameState.Login_User; UpdateUI(); return; }

        if (currentState == GameState.Login_User && cmd == "/admin") { currentState = GameState.Login_Pass; UpdateUI(); }
        else if (currentState == GameState.Login_Pass && cmd == "/password") { currentState = GameState.Intro; UpdateUI(); }
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
            selectedEmailIndex = idx;
            currentState = GameState.Viewing;
            UpdateUI();
            string content = "SUBJECT: " + inbox[idx].subject + "\n----------------\n" + inbox[idx].bodyPages[0];
            if (!string.IsNullOrEmpty(inbox[idx].attachmentName)) content += "\n\n[ATTACHMENT: " + inbox[idx].attachmentName + "]";
            ui.SetOutput(content);
        }
    }

    void ProcessEmail(bool deleteChoice)
    {
        // 1. HIDE THE ATTACHMENT IMMEDIATELY
        // This ensures photos/audio stop playing as soon as you make a choice
        ui.HideAttachment();

        // 2. Logic for processing the file
        inbox[selectedEmailIndex].isProcessed = true;
        processedToday++;

        if (inbox[selectedEmailIndex].isSensitive != deleteChoice) errors++;

        // 3. Check for special events (Intrusion / Day transitions)
        if (currentDay == 2 && processedToday == 8) StartCoroutine(IntrusionSequence());
        else if (currentDay == 2 && processedToday == 15)
        {
            currentState = GameState.Intrusion;
            if (hackerComp) hackerComp.StartBreach();
        }
        else if (currentDay == 3 && processedToday == 20) StartCoroutine(Day3BreachSequence());
        else
        {
            // Display the success message on the terminal
            ui.SetOutput("FILE PROCESSED.\n\nTYPE /close TO RETURN TO INBOX.");
            ui.SetObjective(GetPendingCount());
        }
    }

    IEnumerator IntrusionSequence()
    {
        currentState = GameState.Intrusion;
        ui.HideAttachment();
        audioCtrl.PlayClip(audioCtrl.IntrusionSFX);
        float elapsed = 0;
        while (elapsed < 3f)
        {
            ui.outputField.gameObject.transform.parent.gameObject.SetActive(Random.value > 0.5f);
            ui.SetOutput(Random.value > 0.5f ? "SYSTEM COMPROMISED" : "INTRUDER ALERT");
            ui.outputField.color = Color.red;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            elapsed += 0.1f;
        }
        ui.outputField.gameObject.transform.parent.gameObject.SetActive(true);
        ui.outputField.color = Color.white;
        currentState = GameState.Mailbox;
        UpdateUI();
    }

    public void UpdateUI()
    {
        switch (currentState)
        {
            case GameState.Login_User: ui.SetInstructions("ENTER USERNAME (read sticky note)"); break;
            case GameState.Login_Pass: ui.SetInstructions("ENTER PASSWORD (read sticky note)"); break;
            case GameState.Intro: ui.SetInstructions("PRESS ENTER TO START"); break;
            case GameState.Mailbox: ShowInbox(); ui.SetInstructions("TYPE: /view [number] | /logout"); break;
            case GameState.Viewing: ui.SetInstructions("TYPE: /keep | /delete | /open | /close"); break;
            case GameState.Evaluation: ui.SetInstructions("STATION OFFLINE."); break;
        }
    }

    void ShowInbox()
    {
        string list = "INBOX - DAY " + currentDay + "\n----------------\n";
        for (int i = 0; i < inbox.Count; i++) list += (i + 1) + ". " + (inbox[i].isProcessed ? "[X] " : "[ ] ") + inbox[i].subject + "\n";
        ui.SetOutput(list);
    }

    public void TriggerEndOfDay() => StartCoroutine(PhoneCallSequence());

    IEnumerator PhoneCallSequence()
    {
        currentState = GameState.PhoneCall;
        pendingEvalClip = (errors > 0) ? audioCtrl.badCall : audioCtrl.goodCall;
        audioCtrl.SetRingtone(true);
        ui.SetOutput("SYSTEM SHUTDOWN.\n\n(Incoming call... Click the phone.)");
        ui.SetInstructions("INCOMING: MAYOR");
        yield break;
    }

    public void AnswerPhone()
    {
        if (isPhoneCallActive) return;

        if (currentState == GameState.PhoneCall)
        {
            audioCtrl.SetRingtone(false);
            audioCtrl.PlayInteractSound();
            if (pendingEvalClip != null) StartCoroutine(ExecuteEvaluationCall());
            else if (currentDay == 1) StartCoroutine(Day1IntroCallSequence());
            else if (currentDay == 2) StartCoroutine(Day2PhoneCallSequence());
        }
    }

    IEnumerator Day1IntroCallSequence()
    {
        isPhoneCallActive = true; // LOCK
        string dialogue = "Alvin, Friend: Hey, Paul! You actually made it. Welcome to the 'inner circle.' Look, the terminal looks like a dinosaur, but it’s the most important seat in the entire Municipality. Just keep your head down and clear the backlog. And hey... don't be like Reyes, okay? He started asking questions he shouldn't have, and now nobody knows where he went. Just do it for your Mom. I’ll check on you later.";
        float length = audioCtrl.GetLength(audioCtrl.friendDay1);

        subManager.DisplaySubtitle(dialogue, length);
        audioCtrl.PlayClip(audioCtrl.friendDay1);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay1));
        ui.SetOutput("SYSTEM READY.\n\nPRESS ANY KEY TO LOGIN");
        currentState = GameState.Awakening;
        UpdateUI();
        isPhoneCallActive = false; // UNLOCK
    }

    IEnumerator Day2PhoneCallSequence()
    {
        isPhoneCallActive = true; // LOCK
        audioCtrl.PlayClip(audioCtrl.friendDay2);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay2));
        currentState = GameState.Intro;
        UpdateUI();
        isPhoneCallActive = false; // UNLOCK
    }

    IEnumerator ExecuteEvaluationCall()
    {
        isPhoneCallActive = true; // LOCK
        string dialogue = (errors > 0)
        ? "MAYOR: I’m disappointed, Paul. I see files left in the 'Agos' directory. These... 'discrepancies'... they make the office look bad. And when the office looks bad, I have to cut costs. I'd hate for your mother’s hospital fund to be the first thing we cut. Do better tomorrow, Paul. For her sake."
        : "MAYOR: Paul... I’m looking at the logs. Perfection. That’s what I like about you—you understand how this world works. Your mother is resting comfortably at my private hospital. The surgeons are the best in the province. Keep this up, and your family will never have to worry about a single cent again. Get some rest, son. Tomorrow is a big day for our town.";

        float length = audioCtrl.GetLength(pendingEvalClip);

        subManager.DisplaySubtitle(dialogue, length); 
        
        audioCtrl.PlayClip(pendingEvalClip);
        yield return new WaitForSeconds(length);
        pendingEvalClip = null;
        isPhoneCallActive = false; // UNLOCK
        if (currentDay == 1)
        {
            yield return ui.FadeInBlack(2f);
            LoadDay(2);
            yield return ui.ShowTransitionText("DAY 2: THE RAT", 2f);
            yield return ui.FadeOutBlack(2f);
            ui.SetOutput("STATION 8802: ACTIVE\n\n(Telephone ringing. Click phone.)");
            currentState = GameState.PhoneCall;
            audioCtrl.SetRingtone(true);
        }
        else if (currentDay == 2)
        {
            yield return ui.FadeInBlack(2f);
            LoadDay(3);
            yield return ui.ShowTransitionText("DAY 3: THE TRAP", 2f);
            yield return ui.FadeOutBlack(2f);
            currentState = GameState.Intro;
            UpdateUI();
        }
    }

    IEnumerator Day3BreachSequence()
    {
        currentState = GameState.Day3_Breach;
        ui.SetOutput("CRITICAL ERROR: SYSTEM TERMINATED.");
        audioCtrl.PlayClip(audioCtrl.Day2EndingCall);
        yield return new WaitForSeconds(3f);
        ui.SetInstructions("STAND UP. ESCAPE NOW.");
        if (movement) movement.enabled = true;
        if (officeLight) officeLight.intensity = 0;
    }

    IEnumerator Day3FlickerLight()
    {
        while (currentDay == 3 && currentState != GameState.Day3_Breach)
        {
            officeLight.intensity = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            officeLight.intensity = 0.4f;
            yield return new WaitForSeconds(Random.Range(1f, 4f));
        }
    }

    public void ToggleNote(bool show)
    {
        isReadingNote = show;
        ui.stickyNotePanel.SetActive(show);
        if (audioCtrl) audioCtrl.PlayInteractSound();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        if (show) ui.ShowHoverPrompt(false);
    }

    void OpenAttachment()
    {
        Email e = inbox[selectedEmailIndex];
        if (e.photoAttachment) ui.ShowPhoto(e.photoAttachment);
        if (e.audioAttachment) audioCtrl.PlayClip(e.audioAttachment);
    }

    void TryLogout()
    {
        // 1. Check if any emails are still unprocessed
        foreach (var e in inbox)
        {
            if (!e.isProcessed)
            {
                ui.SetOutput("ACCESS DENIED: PENDING FILES REMAINING.");
                return;
            }
        }

        currentState = GameState.Evaluation;
        UpdateUI();

        TriggerEndOfDay();
    }

    int GetPendingCount()
    {
        int c = 0;
        foreach (var e in inbox) if (!e.isProcessed) c++;
        return c;
    }
}