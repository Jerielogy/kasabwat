using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TerminalLogic : MonoBehaviour
{
    public enum GameState { Awakening, Login, Intro, Mailbox, ViewingMail, Evaluation, PhoneCall }
    public GameState currentState = GameState.Awakening;

    [Header("UI Objects")]
    public TMP_Text outputDisplay;
    public TMP_Text currentInputDisplay;
    public TMP_Text objectiveDisplay;    // Top right: Files remaining
    public TMP_Text instructionDisplay;  // Bottom: Current SOP instruction

    [Header("Audio")]
    public AudioSource officeSpeaker;
    public AudioClip goodCallClip;
    public AudioClip badCallClip;

    private string typedString = "";
    private int selectedEmailIndex = -1;
    private int totalErrors = 0;
    private List<Email> inbox = new List<Email>();

    private class Email
    {
        public string subject;
        public string body;
        public bool isProcessed;
        public bool isSensitive;
        public Email(string s, string b, bool sensitive)
        {
            subject = s; body = b; isSensitive = sensitive; isProcessed = false;
        }
    }

    void Start()
    {
        GenerateDay1Emails();
        UpdateStateUI();
    }

    void GenerateDay1Emails()
    {
        inbox.Add(new Email("AUDIT: Green Valley", "URGENT: Remove Mayor's name from land conversion grant. Page 4 signature must be purged.", true));
        inbox.Add(new Email("PAYROLL: Confidential", "Transfer of 500k to 'Project Linis' slush fund. Destroy evidence.", true));
        inbox.Add(new Email("MEMO: Land Conversion", "Approval for Brgy 143. No environmental clearance attached. Purge record.", true));
        inbox.Add(new Email("INTERNAL: CCTV Footage", "Request to erase logs from Tuesday night at the back entrance.", true));
        inbox.Add(new Email("RECORDS: Contractor X", "Invoice for kickbacks. Destroy immediately to prevent audit trail.", true));

        inbox.Add(new Email("HR: ID Badge", "Your new ID is ready at the lobby.", false));
        inbox.Add(new Email("Office Potluck", "Ma'am Susan is bringing Lumpia. Sign up in the breakroom.", false));
        inbox.Add(new Email("Weather: Manila", "Expect heavy rains tonight. Close all office windows.", false));
        inbox.Add(new Email("IT: Password Reset", "Monthly security reminder. Do not share credentials.", false));
        inbox.Add(new Email("AD: Office Supplies", "Discount on bond paper at National Book Store.", false));
        inbox.Add(new Email("Parking Notice", "Basement 2 closed for cleaning.", false));
        inbox.Add(new Email("Draft: Speech", "Draft for the Mayor's address on 'Transparency'.", false));
        inbox.Add(new Email("Missing: Tupperware", "Blue lid container in fridge. Please claim.", false));
        inbox.Add(new Email("Training: Ethics", "Mandatory video training due in 30 days.", false));
        inbox.Add(new Email("System Maintenance", "Scheduled server reboot at 03:00 AM.", false));
    }

    void Update()
    {
        if (currentState == GameState.Awakening && Input.anyKeyDown)
        {
            currentState = GameState.Login;
            UpdateStateUI();
            return;
        }

        if (currentState != GameState.PhoneCall)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') { if (typedString.Length > 0) typedString = typedString.Substring(0, typedString.Length - 1); }
                else if (c == '\n' || c == '\r') { HandleInput(typedString); typedString = ""; }
                else { typedString += c; }
            }
            currentInputDisplay.text = "> " + typedString + "_";
        }
    }

    void UpdateStateUI()
    {
        switch (currentState)
        {
            case GameState.Awakening:
                outputDisplay.text = "SYSTEM STATUS: OFFLINE\n\nBOOT LOADER v4.2.1\n\n[CRITICAL: INPUT REQUIRED TO INITIALIZE]";
                instructionDisplay.text = "SOP: PRESS ANY KEY TO WAKE SYSTEM";
                break;
            case GameState.Login:
                outputDisplay.text = "KASABWAT OS v1.1\n\nSTATION: CLERK_8802\nSTATUS: LOCKED\n\nENTER CREDENTIALS:";
                instructionDisplay.text = "SOP: TYPE 'admin' AND PRESS ENTER TO ACCESS RECORDS";
                break;
            case GameState.Intro:
                outputDisplay.text = "--- BRIEFING: OPERATION 'LINIS' ---\n\n" +
                                     "OBJECTIVE: Maintain the Mayor's public integrity.\n" +
                                     "METHOD: Purge incriminating digital footprints.\n\n" +
                                     "INSTRUCTIONS:\n" +
                                     "1. Identify [SYSTEM RECORDS] containing signatures or transfers.\n" +
                                     "2. Use /delete for SENSITIVE files.\n" +
                                     "3. Use /keep for MUNDANE/SPAM files to mask activity.\n\n" +
                                     "FAILURE TO COMPLY IS A CONTRACT VIOLATION.";
                instructionDisplay.text = "SOP: PRESS ENTER TO PROCEED TO WORKSTATION";
                break;
            case GameState.Mailbox:
                ShowMailbox();
                instructionDisplay.text = "SOP: SELECT FILE USING /view [number]. LOGOUT PERMITTED ONLY AT 0 UNREAD.";
                break;
            case GameState.ViewingMail:
                instructionDisplay.text = "SOP: ANALYZE CONTENT. USE /delete (SENSITIVE) OR /keep (MUNDANE). /close TO EXIT.";
                break;
            case GameState.Evaluation:
                instructionDisplay.text = "SOP: REVIEW DATA ACCURACY. PRESS ENTER TO FINALIZE SHIFT.";
                break;
        }
    }

    void HandleInput(string input)
    {
        string cmd = input.ToLower().Trim();

        if (currentState == GameState.Login)
        {
            if (cmd == "admin") { currentState = GameState.Intro; UpdateStateUI(); }
            else { outputDisplay.text = "ERROR: ACCESS DENIED. RE-ENTER PASSWORD:"; }
        }
        else if (currentState == GameState.Intro) { currentState = GameState.Mailbox; UpdateStateUI(); }
        else if (currentState == GameState.Mailbox)
        {
            if (cmd.StartsWith("/view "))
            {
                string indexStr = cmd.Replace("/view ", "");
                if (int.TryParse(indexStr, out int idx)) ViewEmail(idx - 1);
            }
            else if (cmd == "/logout") TryLogout();
        }
        else if (currentState == GameState.ViewingMail)
        {
            if (cmd == "/delete") ProcessEmail(true);
            else if (cmd == "/keep") ProcessEmail(false);
            else if (cmd == "/close") { currentState = GameState.Mailbox; UpdateStateUI(); }
        }
        else if (currentState == GameState.Evaluation) { AutoAnswerPhone(); }
    }

    void ShowMailbox()
    {
        string list = "--- INBOX: STATION 8802 ---\n";
        for (int i = 0; i < inbox.Count; i++)
        {
            string status = inbox[i].isProcessed ? "[DONE]" : "[NEW ]";
            list += (i + 1) + ". " + status + " " + inbox[i].subject + "\n";
        }
        outputDisplay.text = list;
        UpdateObjective();
    }

    void ViewEmail(int index)
    {
        if (index >= 0 && index < inbox.Count && !inbox[index].isProcessed)
        {
            selectedEmailIndex = index;
            currentState = GameState.ViewingMail;
            UpdateStateUI();
            outputDisplay.text = "RECORD TYPE: " + (inbox[index].isSensitive ? "[SENSITIVE]" : "[MUNDANE]") +
                                 "\nSUBJECT: " + inbox[index].subject +
                                 "\n----------------\n" + inbox[index].body +
                                 "\n\nCOMMANDS: /delete | /keep | /close";
        }
    }

    void ProcessEmail(bool deleteChoice)
    {
        Email current = inbox[selectedEmailIndex];
        current.isProcessed = true;
        if (current.isSensitive != deleteChoice) totalErrors++;
        outputDisplay.text = "TRANSACTION COMPLETED.\nDATA STATE: " + (deleteChoice ? "PURGED" : "ARCHIVED") +
                             "\n\nTYPE /close TO RETURN TO INBOX.";
        UpdateObjective();
    }

    void TryLogout()
    {
        bool allDone = true;
        foreach (var mail in inbox) if (!mail.isProcessed) allDone = false;

        if (allDone) ShowEvaluation();
        else outputDisplay.text = "LOGOUT DENIED: PENDING TASKS DETECTED (" + GetPendingCount() + ").";
    }

    void ShowEvaluation()
    {
        currentState = GameState.Evaluation;
        UpdateStateUI();
        outputDisplay.text = "--- SESSION EVALUATION: CLERK 8802 ---\n\n" +
                             "TOTAL RECORDS PROCESSED: 15\n" +
                             "INTEGRITY ERRORS: " + totalErrors + "\n" +
                             "LOYALTY RATING: " + (totalErrors == 0 ? "100% - IDEAL" : totalErrors < 3 ? "80% - ACCEPTABLE" : "UNRELIABLE") +
                             "\n\n[PRESS ENTER TO LOGOUT]";
        currentInputDisplay.text = "";
    }

    void AutoAnswerPhone()
    {
        currentState = GameState.PhoneCall;
        outputDisplay.text = "SHUTTING DOWN...\n\nSESSION ENDED.\n\n(The phone ringer sounds...)";
        instructionDisplay.text = "SOP: LISTEN TO THE MESSAGE.";

        if (officeSpeaker != null)
        {
            if (totalErrors >= 3)
            {
                officeSpeaker.clip = badCallClip;
            }
            else
            {
                officeSpeaker.clip = goodCallClip;
            }
            officeSpeaker.Play();
        }
    } // Function closed correctly here

    int GetPendingCount()
    {
        int count = 0;
        foreach (var mail in inbox) if (!mail.isProcessed) count++;
        return count;
    }

    void UpdateObjective()
    {
        objectiveDisplay.text = "PENDING: " + GetPendingCount();
    }
} // Class closed correctly here