using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Email
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

public class EmailDatabase : MonoBehaviour
{
    public List<Email> GetDayContent(int day)
    {
        List<Email> inbox = new List<Email>();
        if (day == 1)
        {
            inbox.Add(new Email("AUDIT: Green Valley", "URGENT: Remove Mayor's name from land conversion grant. Page 4 signature must be purged.", true));
            inbox.Add(new Email("INTERNAL: CCTV Footage", "Request to erase logs from Tuesday night at the back entrance.", true));
            inbox.Add(new Email("HR: ID Badge", "Your new ID is ready at the lobby.", false));
            inbox.Add(new Email("Office Potluck", "Ma'am Susan is bringing Lumpia. Sign up in the breakroom.", false));
            inbox.Add(new Email("MEMO: Land Conversion", "Approval for Brgy 143. No environmental clearance attached. Purge record.", true));
            inbox.Add(new Email("Weather: Manila", "Expect heavy rains tonight. Close all office windows.", false));
            inbox.Add(new Email("IT: Password Reset", "Monthly security reminder. Do not share credentials.", false));
            inbox.Add(new Email("PAYROLL: Confidential", "Transfer of 500k to 'Project Linis' slush fund. Destroy evidence.", true));
            inbox.Add(new Email("AD: Office Supplies", "Discount on bond paper at National Book Store.", false));
            inbox.Add(new Email("Parking Notice", "Basement 2 closed for cleaning.", false));
            inbox.Add(new Email("Draft: Speech", "Draft for the Mayor's address on 'Transparency'.", false));
            inbox.Add(new Email("RECORDS: Contractor X", "Invoice for kickbacks. Destroy immediately to prevent audit trail.", true));
            inbox.Add(new Email("Missing: Tupperware", "Blue lid container in fridge. Please claim.", false));
            inbox.Add(new Email("Training: Ethics", "Mandatory video training due in 30 days.", false));
            inbox.Add(new Email("System Maintenance", "Scheduled server reboot at 03:00 AM.", false));
        }
        else if (day == 2)
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
            inbox.Add(new Email("Asset: 044-V", "[ATTACHMENT: VIDEO] - CCTV back-lot. Subject: Journalist A. Relos. Ensure no digital trace remains.", true));
            inbox.Add(new Email("Evidence: B-01", "[ATTACHMENT: IMAGE] - Photo of secret ledger 'Project S'. Contains narcotic distribution signatures.", true));
            inbox.Add(new Email("Wiretap: 22-A", "[ATTACHMENT: AUDIO] - Recorded call between Mayor and Precinct 4. Re: 'Silencing' the audit witness.", true));
            inbox.Add(new Email("Payload: 99", "[ATTACHMENT: PHOTO] - Warehouse stock. 5,000 boxes of 'Ayuda' containing vote-buying cash and flyers.", true));
            inbox.Add(new Email("Purge: CCTV", "[ATTACHMENT: VIDEO] - Bridge footage 03:00 AM. Disposal of 'unwanted materials' into the Marilao river.", true));
            inbox.Add(new Email("Land Title Swap", "Transfer ownership of Lot 302 in Marilao to the Mayor's spouse.", true));
            inbox.Add(new Email("Witness Management", "The witness has been located. Trace their last digital login from the nearest terminal.", true));
            inbox.Add(new Email("\"The Package\"", "A 'package' was dropped in the Marilao river. Delete all inquiries regarding the smell.", true));
            inbox.Add(new Email("Bribe Log", "Input the 'donations' from the subdivision developers into the secret ledger.", true));
            inbox.Add(new Email("Warning: Station 8802", "We see you looking at these files. Just do your job if you want to stay safe.", true));
        }
        return inbox;
    }
}