using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Email
{
    public string subject;
    [TextArea(3, 10)] public string body;
    public bool isProcessed;
    public bool isSensitive;
    public Sprite photoAttachment;
    public AudioClip audioAttachment;
    public string attachmentName; // NEW: Stores the filename

    public Email(string s, string b, bool sensitive, Sprite photo = null, AudioClip audio = null, string aName = "")
    {
        subject = s;
        body = b;
        isSensitive = sensitive;
        isProcessed = false;
        photoAttachment = photo;
        audioAttachment = audio;
        attachmentName = aName; // NEW: Assigns the filename
    }
}

public class EmailDatabase : MonoBehaviour
{
    [Header("Day 2 Evidence Assets")]
    public Sprite photoEvidenceB01;
    public Sprite photoPayload99;
    public Sprite photoCCTVStill; // Replaced video with a still image
    public AudioClip audioWiretap22A;

    public List<Email> GetDayContent(int day)
    {
        List<Email> inbox = new List<Email>();

        if (day == 1)
        {
            inbox.Add(new Email("AUDIT: Green Valley", "URGENT: Remove Mayor's name from land conversion grant. Page 4 signature must be purged.", true));
            inbox.Add(new Email("HR: ID Badge", "Your new ID is ready at the lobby.", false));
            inbox.Add(new Email("Office Potluck", "Ma'am Susan is bringing Lumpia. Sign up in the breakroom.", false));
            inbox.Add(new Email("MEMO: Land Conversion", "Approval for Brgy 143. No environmental clearance attached. Purge record.", true));
            inbox.Add(new Email("Weather: Manila", "Expect heavy rains tonight. Close all office windows.", false));
            inbox.Add(new Email("IT: Password Reset", "Monthly security reminder. Do not share credentials.", false));
            inbox.Add(new Email("PAYROLL: Confidential", "Transfer of 500k to 'Project Linis' slush fund. Destroy evidence.", true));
        }
        else if (day == 2)
        {
            inbox.Add(new Email("Birthday sa Accounting", "Salu-salo for Ma'am Tess.", false));
            inbox.Add(new Email("Travel Order: SJDM", "Inspection in San Jose Del Monte.", false));

            // Emails with specific attachment names
            inbox.Add(new Email("Asset: 044-S", "CCTV back-lot. Journalist A. Relos spotted.", true, photoCCTVStill, null, "CCTV_STILL_044.img"));
            inbox.Add(new Email("Wiretap: 22-A", "Recorded call: Mayor and Precinct 4.", true, null, audioWiretap22A, "EVIDENCE_22A.mp3"));
            inbox.Add(new Email("Evidence: B-01", "Photo of secret ledger 'Project S'.", true, photoEvidenceB01, null, "LEDGER_S.img"));
            inbox.Add(new Email("Payload: 99", "Warehouse stock. 5,000 boxes of 'Ayuda'.", true, photoPayload99, null, "WAREHOUSE_99.img"));

            inbox.Add(new Email("Warning: Station 8802", "We see you, Paul. Just do your job.", true));
        }
        return inbox;
    }
}