using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TerminalLogic : MonoBehaviour
{
    // Added Login_Pass to handle the new two-step login
    public enum GameState { Awakening, Login_User, Login_Pass, Intro, Mailbox, ViewingMail, Evaluation, PhoneCall }
    public GameState currentState = GameState.Awakening;

    [Header("UI Objects")]
    public TMP_Text outputDisplay;
    public TMP_Text currentInputDisplay;
    public TMP_Text objectiveDisplay;
    public TMP_Text instructionDisplay;

    [Header("Audio (Drag Main Camera here)")]
    public AudioSource officeSpeaker;
    public AudioClip goodCallClip;
    public AudioClip badCallClip;

    private string typedString = "";
    private int selectedEmailIndex = -1;
    private int totalErrors = 0;
    public int currentDay = 1;
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
        LoadDay(1);
        UpdateStateUI();
    }

    public void LoadDay(int day)
    {
        currentDay = day;
        totalErrors = 0;
        inbox.Clear();

        if (day == 1) GenerateDay1Content();
        else if (day == 2) GenerateDay2Content();

        UpdateObjective();
    }

    void GenerateDay1Content()
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

    void GenerateDay2Content()
    {
        inbox.Add(new Email("Office Canteen Memo", "No single-use plastics allowed starting tomorrow. Please bring your own containers.", false));
        inbox.Add(new Email("IT Password Update", "Mandatory password change for all stations. Update your credentials by 5:00 PM.", false));
        inbox.Add(new Email("Birthday sa Accounting", "Salu-salo for Ma'am Tess at 3 PM. There will be pansit and pitsi-pitsi.", false));
        inbox.Add(new Email("Aircon Maintenance", "Technicians will clean the split-type units in Level 1 tomorrow. Cover your monitors.", false));
        inbox.Add(new Email("Lost Umbrella", "Blue umbrella found at the lobby. Claim at the Security Guard station.", false));
        inbox.Add(new Email("Flag Ceremony", "Attendance is mandatory for the Monday morning ceremony. Wear complete uniform.", false));
        inbox.Add(new Email("Water Dispenser Glitch", "The hallway dispenser is making noises due to a broken valve. Repair is scheduled.", false));
        inbox.Add(new Email("Printer Ink Refill", "We are out of cyan ink. Procurement is following up on the delivery.", false));
        inbox.Add(new Email("Missing Chair", "One white monoblock chair is missing from Conference Room A. Please return it.", false));
        inbox.Add(new Email("SSS Documents", "Please file the SSS contributions for the contractual workers this month.", false));
        inbox.Add(new Email("Seminar on Ethics", "Mandatory seminar on 'Government Ethics' next Friday. Clear your schedules.", false));
        inbox.Add(new Email("Office Cleaning", "General cleaning this Saturday. Ensure no confidential docs are on your desks.", false));
        inbox.Add(new Email("Request for Travel Order", "Processing travel order for Sir Ben's inspection in San Jose Del Monte.", false));
        inbox.Add(new Email("Slow Internet", "Expect slow speeds today during server firewall upgrades.", false));
        inbox.Add(new Email("Request for Supplies", "Request for 2 boxes of black pens and 5 reams of A4 paper for Admin.", false));
        inbox.Add(new Email("Calamity Fund Transfer", "Transfer 10M PHP from Typhoon Relief to the Mayor's 'Special Events' budget. No receipts.", true));
        inbox.Add(new Email("Ghost Employees List", "Delete payroll entries for Batch 404. They are not real employees but must be paid.", true));
        inbox.Add(new Email("Auditor Relocation", "The auditor asking about road budgets has been relocated. Wipe all their emails.", true));
        inbox.Add(new Email("Quarrying Permits", "Approve 'GoldRock Mining' permit immediately. No environmental clearance required.", true));
        inbox.Add(new Email("Hospital Budget Cut", "Divert oxygen tank funds to the Mayor's campaign posters. Mark as 'Admin Expense'.", true));
        inbox.Add(new Email("Land Title Swap", "Transfer ownership of Lot 302 in Marilao to the Mayor's spouse.", true));
        inbox.Add(new Email("Witness Management", "The witness has been located. Trace their last digital login from the nearest terminal.", true));
        inbox.Add(new Email("\"The Package\"", "A 'package' was dropped in the Marilao river. Delete all inquiries regarding the smell.", true));
        inbox.Add(new Email("Bribe Log", "Input the 'donations' from the subdivision developers into the secret ledger.", true));
        inbox.Add(new Email("Warning: Station 8802", "We see you looking at these files. Just do your job if you want to stay safe.", true));
    }

    void Update()
    {
        if (currentState == GameState.Awakening && Input.anyKeyDown)
        {
            currentState = GameState.Login_User;
            UpdateStateUI();
            return;
        }

        if (currentState != GameState.PhoneCall && currentState != GameState.Evaluation)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') { if (typedString.Length > 0) typedString = typedString.Substring(0, typedString.Length - 1); }
                else if (c == '\n' || c == '\r') { HandleInput(typedString); typedString = ""; }
                else { typedString += c; }
            }
            currentInputDisplay.text = "> " + typedString + "_";
        }
        else if (Input.GetKeyDown(KeyCode.Return)) { HandleInput(""); }
    }

    void UpdateStateUI()
    {
        switch (currentState)
        {
            case GameState.Awakening:
                outputDisplay.text = "SYSTEM STATUS: OFFLINE\n\nBOOT LOADER v4.2.1\n\n[CRITICAL: INPUT REQUIRED]";
                instructionDisplay.text = "SOP: PRESS ANY KEY TO INITIALIZE";
                break;
            case GameState.Login_User:
                outputDisplay.text = "KASABWAT OS v1.1\n\nSTATION: CLERK_8802\n\nENTER USERNAME:";
                instructionDisplay.text = "SOP: ENTER AUTHORIZED USERNAME";
                break;
            case GameState.Login_Pass:
                outputDisplay.text = "KASABWAT OS v1.1\n\nSTATION: CLERK_8802\nUSER: admin\n\nENTER PASSWORD:";
                instructionDisplay.text = "SOP: ENTER SECURITY CREDENTIALS";
                break;
            case GameState.Intro:
                outputDisplay.text = $"--- DAY {currentDay} BRIEFING: OPERATION 'LINIS' ---\n\n" +
                                     "OBJECTIVE: Maintain the Mayor's public integrity.\n" +
                                     "METHOD: Purge incriminating digital footprints.\n\n" +
                                     "INSTRUCTIONS:\n" +
                                     "1. Use /delete for SENSITIVE files.\n" +
                                     "2. Use /keep for MUNDANE files.\n";
                instructionDisplay.text = "SOP: PRESS ENTER TO START SHIFT";
                break;
            case GameState.Mailbox:
                ShowMailbox();
                instructionDisplay.text = "SOP: SELECT FILE USING /view [number].";
                break;
            case GameState.ViewingMail:
                instructionDisplay.text = "SOP: ANALYZE CONTENT. USE /delete OR /keep. /close TO EXIT.";
                break;
            case GameState.Evaluation:
                instructionDisplay.text = "SOP: PRESS ENTER TO LOGOUT.";
                break;
        }
    }

    void HandleInput(string input)
    {
        string cmd = input.ToLower().Trim();

        if (currentState == GameState.Login_User)
        {
            if (cmd == "admin") { currentState = GameState.Login_Pass; UpdateStateUI(); }
            else { outputDisplay.text = "ERROR: UNKNOWN USER."; }
        }
        else if (currentState == GameState.Login_Pass)
        {
            if (cmd == "password") { currentState = GameState.Intro; UpdateStateUI(); }
            else { currentState = GameState.Login_User; outputDisplay.text = "ERROR: ACCESS DENIED. RE-ENTER USERNAME:"; }
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
        else if (currentState == GameState.Evaluation) { StartCoroutine(PhoneCallSequence()); }
    }

    void ShowMailbox()
    {
        string list = $"--- INBOX: DAY {currentDay} ---\n";
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
                                 "\n\nCOMMANDS: /delete | /keep | /close"; // Added Command Footer back
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
        if (allDone) { currentState = GameState.Evaluation; ShowEvaluation(); }
        else outputDisplay.text = "LOGOUT DENIED: PENDING TASKS.";
    }

    void ShowEvaluation()
    {
        UpdateStateUI();
        outputDisplay.text = $"--- DAY {currentDay} EVALUATION ---\n\n" +
                             "INTEGRITY ERRORS: " + totalErrors + "\n" +
                             "LOYALTY RATING: " + (totalErrors == 0 ? "IDEAL" : totalErrors < 3 ? "ACCEPTABLE" : "UNRELIABLE") +
                             "\n\n[PRESS ENTER TO LOGOUT]";
    }

    IEnumerator PhoneCallSequence()
    {
        currentState = GameState.PhoneCall;
        outputDisplay.text = "SHUTTING DOWN...\n\nSESSION ENDED.\n\n(The phone rings...)";
        yield return new WaitForSeconds(2f);

        if (officeSpeaker != null)
        {
            officeSpeaker.clip = (totalErrors >= 3) ? badCallClip : goodCallClip;
            outputDisplay.text = "(Incoming: MAYOR)\n\n" +
                                 (totalErrors >= 3 ? "<color=red>\"Disappointing, Clerk.\"</color>" : "<color=green>\"Excellent work.\"</color>");
            officeSpeaker.Play();
            yield return new WaitForSeconds(officeSpeaker.clip.length + 1f);
        }

        if (currentDay == 1)
        {
            outputDisplay.text = "PREPARING NEXT SHIFT...";
            yield return new WaitForSeconds(2f);
            LoadDay(2);
            currentState = GameState.Intro;
            UpdateStateUI();
        }
        else outputDisplay.text = "SYSTEM DISCONNECTED.";
    }

    void UpdateObjective() { objectiveDisplay.text = "FILES: " + GetPendingCount(); }
    int GetPendingCount() { int c = 0; foreach (var m in inbox) if (!m.isProcessed) c++; return c; }
}