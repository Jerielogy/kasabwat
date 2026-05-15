using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Prologue, Awakening, Login_User, Login_Pass, Intro, Mailbox, Viewing, Evaluation, PhoneCall, StoryCall, Intrusion, Day3_Breach }

public class KasabwatController : MonoBehaviour
{
    public GameState currentState;
    public Light officeLight;
    public KasabwatMovement movement;
    public HackerConversation hackerComp;
    public bool isReadingNote = false;

    [Header("Day 3 Killer Mechanics")]
    public GameObject blackFigure;   // The figure that appears at the end
    public Transform doorEndPoint;  // The location of the locked hallway door
    public AudioSource radioStatic; // To cut the sound during the final scare
    private int leakCount = 0;
    private int deleteCount = 0;
    private bool isEscaping = false;

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

        if (blackFigure) blackFigure.SetActive(false); // Ensure he's hidden

        LoadDay(1);
        StartCoroutine(StartGameSequence());
    }

    public void LoadDay(int day)
    {
        currentDay = day;
        errors = 0;
        processedToday = 0;
        leakCount = 0;
        deleteCount = 0;
        inbox = db.GetDayContent(day);
        ui.SetObjective(inbox.Count);
        if (movement) movement.enabled = false;
        if (day == 3 && officeLight) StartCoroutine(Day3FlickerLight());
    }

    IEnumerator StartGameSequence()
    {
        currentState = GameState.Prologue;
        string dayText = (currentDay == 3) ? "DAY 3: THE TRAP" : "DAY " + currentDay + ": THE COMPLIANCE";
        yield return ui.ShowTransitionText(dayText, 1.5f);
        yield return ui.FadeOutBlack(1.5f);
        currentState = GameState.PhoneCall;
        audioCtrl.SetRingtone(true);
        ui.SetOutput("(Telephone ringing... Click the phone to answer.)");
        ui.SetInstructions("INCOMING: FRIEND");
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
            else if (cmd == "/leak" && currentDay == 3) ProcessEmail(false); // Whistleblower choice
            else if (cmd == "/keep" && currentDay != 3) ProcessEmail(false); // Normal choice
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
        ui.HideAttachment();
        inbox[selectedEmailIndex].isProcessed = true;
        processedToday++;

        // Track choices for Day 3 Ending
        if (currentDay == 3)
        {
            if (deleteChoice) deleteCount++;
            else leakCount++;
        }
        else
        {
            if (inbox[selectedEmailIndex].isSensitive != deleteChoice) errors++;
        }

        if (currentDay == 2 && processedToday == 8) StartCoroutine(IntrusionSequence());
        else if (currentDay == 2 && processedToday == 15)
        {
            currentState = GameState.Intrusion;
            if (hackerComp) hackerComp.StartBreach();
        }
        else
        {
            ui.SetOutput("FILE PROCESSED.\n\nTYPE /close TO RETURN TO INBOX.");
            ui.SetObjective(GetPendingCount());
        }
    }

    void TryLogout()
    {
        foreach (var e in inbox)
        {
            if (!e.isProcessed)
            {
                ui.SetOutput("ACCESS DENIED: PENDING FILES REMAINING.");
                return;
            }
        }

        if (currentDay == 3) StartCoroutine(Day3FinalSequence());
        else
        {
            currentState = GameState.Evaluation;
            UpdateUI();
            TriggerEndOfDay();
        }
    }

    IEnumerator Day3FinalSequence()
    {
        ui.SetOutput("SYSTEM SHUTDOWN...");
        yield return new WaitForSeconds(2f);

        if (deleteCount >= leakCount)
        {
            // MAYOR ENDING
            yield return ui.FadeInBlack(2f);
            yield return ui.ShowTransitionText("WALA NA ANG EBIDENSYA.\n\nPERO WALA NA RIN ANG NANAY KO.", 4f);
            Application.Quit();
        }
        else
        {
            // WHISTLEBLOWER ENDING
            currentState = GameState.Day3_Breach;
            ui.SetOutput("DATA LEAKED. TAKBO.");
            if (officeLight) officeLight.intensity = 0;
            if (movement) movement.enabled = true;
            isEscaping = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (isEscaping && doorEndPoint != null)
        {
            float dist = Vector3.Distance(movement.transform.position, doorEndPoint.position);
            if (dist < 1.8f)
            {
                isEscaping = false;
                StartCoroutine(TheLockedDoorEnding());
            }
        }
    }

    IEnumerator TheLockedDoorEnding()
    {
        movement.enabled = false;
        ui.SetOutput("THE DOOR IS LOCKED.");
        yield return new WaitForSeconds(1.5f);

        // INSTANT 180 TURN
        Vector3 rot = Camera.main.transform.eulerAngles;
        Camera.main.transform.eulerAngles = new Vector3(rot.x, rot.y + 180f, rot.z);

        if (blackFigure) blackFigure.SetActive(true);
        if (radioStatic) radioStatic.Stop();

        yield return new WaitForSeconds(0.2f);
        yield return ui.FadeInBlack(0.1f);
        yield return new WaitForSeconds(2f);
        yield return ui.ShowTransitionText("KASABWAT", 5f);
        Application.Quit();
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
            case GameState.Viewing:
                string prompt = (currentDay == 3) ? "/leak | /delete" : "/keep | /delete";
                ui.SetInstructions("TYPE: " + prompt + " | /open | /close");
                break;
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
        isPhoneCallActive = true;
        string dialogue = "Friend: Buddy! Paul! Glad you accepted the offer. Welcome to the municipality's 'inner circle.' Station 8802 might look old, but it's the heart of all the 'Agos' documents. Look, man, your job is simple: just clean up the database. All of Reyes' backlogs—delete them all. And hey... don't be like Reyes, okay? He started asking too many questions, and well, he hasn't been seen since. Just think about your Mom. Her surgery is our priority here. Do your best, man. I’ll check on you later.";
        float length = audioCtrl.GetLength(audioCtrl.friendDay1);
        subManager.DisplaySubtitle(dialogue, length);
        audioCtrl.PlayClip(audioCtrl.friendDay1);
        yield return new WaitForSeconds(length);
        ui.SetOutput("SYSTEM READY.\n\nPRESS ANY KEY TO LOGIN");
        currentState = GameState.Awakening;
        UpdateUI();
        isPhoneCallActive = false;
    }

    IEnumerator Day2PhoneCallSequence()
    {
        isPhoneCallActive = true;
        string dialogue = "Friend: Paul... do you hear the rain outside? It sounds different this time, man. I've been talking to some people out here, folks from Brgy. Matapang. They say no cement was actually used for the dike. But in the files you're cleaning there, it says 'Optimal' and 'Complete,' doesn't it? Man, be careful. I saw some of the Mayor's men lurking near the Station earlier. It’s like someone is watching you. Just delete what needs to be deleted, don't read them anymore. You might end up like Reyes.";
        float length = audioCtrl.GetLength(audioCtrl.friendDay2);
        subManager.DisplaySubtitle(dialogue, length); 
        audioCtrl.PlayClip(audioCtrl.friendDay2);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.friendDay2));
        currentState = GameState.Intro;
        UpdateUI();
        isPhoneCallActive = false;
    }

    IEnumerator ExecuteEvaluationCall()
    {
        isPhoneCallActive = true;
        string dialogue = (errors > 0) ? "MAYOR: Paul... I am disappointed by what I see. There are files you didn't erase properly. Some 'discrepancies' were left in my directory. Those mistakes, Paul, could spark trouble for all of us. And when my office runs into trouble, we have to cut costs. It would be a shame for your mother’s private room and medications if we suddenly had to withdraw the budget, wouldn't it? Fix your work tomorrow, Paul. Don't let us both regret it in the end." : "MAYOR: Well done, Paul. I saw your logs tonight. Clean. No issues. That’s what I like about you—you know how to keep your word and show gratitude. Your mother’s last check-up just finished, and my doctors say she’s doing well. Wonderful news, isn't it? Just keep up your loyalty to me, and I'll make sure she has no problems at the hospital. Get some rest for now, Paul. Tomorrow is a big day for our town.";
        float length = audioCtrl.GetLength(pendingEvalClip);
        subManager.DisplaySubtitle(dialogue, length);
        audioCtrl.PlayClip(pendingEvalClip);
        yield return new WaitForSeconds(length);
        pendingEvalClip = null;
        isPhoneCallActive = false;

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

    int GetPendingCount()
    {
        int c = 0;
        foreach (var e in inbox) if (!e.isProcessed) c++;
        return c;
    }
}