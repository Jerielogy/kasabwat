using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KasabwatController : MonoBehaviour
{
    public enum GameState { Prologue, Awakening, Login_User, Login_Pass, Intro, Mailbox, Viewing, Evaluation, PhoneCall, StoryCall }
    public GameState currentState;

    private TerminalUI ui;
    private EmailDatabase db;
    private TerminalAudio audioCtrl;

    private List<Email> inbox;
    private int currentDay = 1;
    private int errors = 0;
    private int selectedEmailIndex = -1;
    private int currentPage = 0;
    private int emailsPerPage = 10;

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
        currentPage = 0;
        inbox = db.GetDayContent(day);
        ui.SetObjective(GetPendingCount());
    }

    IEnumerator StartGameSequence()
    {
        currentState = GameState.Prologue;

        // Reset text alpha to 0 just in case it was left visible in the editor
        ui.transitionDayText.color = new Color(1, 1, 1, 0);

        // 1. Show the meaningful word on Black Screen
        yield return ui.ShowTransitionText("THE COMPLIANCE", 1.5f);

        // 2. Reveal the office workstation
        yield return ui.FadeOutBlack(1.5f);

        // 3. The Initial Friend Call (Story Setup)
        currentState = GameState.StoryCall;
        ui.SetOutput("STATION 8802: INITIALIZING...\n\n(Incoming Call: Paul)");
        audioCtrl.PlayClip(audioCtrl.friendDay1);

        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.officeSpeaker.clip) + 1f);

        ui.SetOutput("SYSTEM READY.\n\nPRESS ANY KEY TO LOGIN");
        currentState = GameState.Awakening;
        UpdateUI();
    }

    public void HandleInput(string input)
    {
        // TRIGGER: This wakes the terminal up from the "Press any key" screen
        if (currentState == GameState.Awakening)
        {
            currentState = GameState.Login_User;
            UpdateUI();
            return;
        }

        string cmd = input.ToLower().Trim();

        if (currentState == GameState.Login_User)
        {
            if (cmd == "admin") { currentState = GameState.Login_Pass; UpdateUI(); }
        }
        else if (currentState == GameState.Login_Pass)
        {
            if (cmd == "password") { currentState = GameState.Intro; UpdateUI(); }
            else { currentState = GameState.Login_User; UpdateUI(); }
        }
        else if (currentState == GameState.Intro)
        {
            currentState = GameState.Mailbox; UpdateUI();
        }
        else if (currentState == GameState.Mailbox)
        {
            if (cmd == "/next")
            {
                int p = Mathf.CeilToInt((float)inbox.Count / 10);
                if (currentPage < p - 1) { currentPage++; UpdateUI(); }
            }
            else if (cmd == "/back")
            {
                if (currentPage > 0) { currentPage--; UpdateUI(); }
            }
            else if (cmd.StartsWith("/view "))
            {
                string indexStr = cmd.Replace("/view ", "");
                if (int.TryParse(indexStr, out int idx)) ViewEmail(idx - 1);
            }
            else if (cmd == "/logout") TryLogout();
        }
        else if (currentState == GameState.Viewing)
        {
            if (cmd == "/delete") ProcessEmail(true);
            else if (cmd == "/keep") ProcessEmail(false);
            else if (cmd == "/close") { currentState = GameState.Mailbox; UpdateUI(); }
        }
    }

    void UpdateUI()
    {
        switch (currentState)
        {
            case GameState.Awakening:
                ui.SetInstructions("INITIALIZE STATION");
                break;
            case GameState.Login_User:
                ui.SetOutput("KASABWAT OS v1.1\n\nENTER USERNAME:");
                ui.SetInstructions("TYPE 'admin'");
                break;
            case GameState.Login_Pass:
                ui.SetOutput("KASABWAT OS v1.1\nUSER: admin\n\nENTER PASSWORD:");
                ui.SetInstructions("TYPE 'password'");
                break;
            case GameState.Intro:
                ui.SetOutput($"--- DAY {currentDay} BRIEFING ---\n\nOBJECTIVE: Maintain system integrity.\nMETHOD: Purge incriminating footprints.");
                ui.SetInstructions("PRESS ENTER TO START");
                break;
            case GameState.Mailbox:
                ShowMailbox();
                ui.SetInstructions("/view [num], /next, or /back");
                break;
            case GameState.Viewing:
                ui.SetInstructions("/delete OR /keep. /close TO EXIT.");
                break;
            case GameState.Evaluation:
                ui.SetOutput($"--- DAY {currentDay} EVALUATION ---\n\nERRORS: {errors}\n\nLOYALTY RATING: " + (errors == 0 ? "IDEAL" : errors < 3 ? "ACCEPTABLE" : "UNRELIABLE"));
                ui.SetInstructions("PRESS ENTER TO LOGOUT");
                break;
        }
    }

    void ShowMailbox()
    {
        int start = currentPage * emailsPerPage;
        int end = Mathf.Min(start + emailsPerPage, inbox.Count);
        string list = $"--- INBOX: DAY {currentDay} (Page {currentPage + 1}) ---\n";
        for (int i = start; i < end; i++)
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
            ui.SetOutput("SUBJECT: " + inbox[index].subject + "\n----------------\n" + inbox[index].body + "\n\nCOMMANDS: /delete | /keep | /close");
        }
    }

    void ProcessEmail(bool deleteChoice)
    {
        inbox[selectedEmailIndex].isProcessed = true;
        if (inbox[selectedEmailIndex].isSensitive != deleteChoice) errors++;

        ui.SetOutput("FILE PROCESSED.\n\nTYPE /close TO RETURN TO INBOX.");
        ui.SetObjective(GetPendingCount());
    }

    void TryLogout()
    {
        foreach (var m in inbox) if (!m.isProcessed) return;
        currentState = GameState.Evaluation;
        UpdateUI();
    }

    public void TriggerEndOfDay() { StartCoroutine(PhoneCallSequence()); }

    IEnumerator PhoneCallSequence()
    {
        currentState = GameState.PhoneCall;
        ui.SetOutput("SHUTTING DOWN...\n\n(Incoming: MAYOR)");

        audioCtrl.PlayClip(errors >= 3 ? audioCtrl.badCall : audioCtrl.goodCall);
        yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.officeSpeaker.clip) + 1f);

        if (currentDay == 1)
        {
            // Transition back to black for the next day
            yield return ui.FadeInBlack(1f);

            LoadDay(2);
            UpdateUI();

            yield return ui.ShowTransitionText("THE RAT", 1f);
            yield return ui.FadeOutBlack(1f);

            currentState = GameState.StoryCall;
            ui.SetOutput("STATION 8802: ACTIVE\n\n(Incoming Call: Paul)");
            audioCtrl.PlayClip(audioCtrl.friendDay2);

            yield return new WaitForSeconds(audioCtrl.GetLength(audioCtrl.officeSpeaker.clip) + 1f);

            currentState = GameState.Intro;
            UpdateUI();
        }
        else
        {
            ui.SetOutput("SYSTEM DISCONNECTED.\n\nEnd of Demo.");
        }
    }

    int GetPendingCount()
    {
        int c = 0;
        foreach (var m in inbox) if (!m.isProcessed) c++;
        return c;
    }
}