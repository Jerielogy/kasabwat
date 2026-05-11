using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Email
{
    public string subject;
    [TextArea(8, 15)] public string body;
    public bool isProcessed, isSensitive;
    public Sprite photoAttachment;
    public AudioClip audioAttachment;
    public string attachmentName;

    public Email(string s, string b, bool sensitive, Sprite photo = null, AudioClip audio = null, string aName = "")
    {
        subject = s; body = b; isSensitive = sensitive; isProcessed = false;
        photoAttachment = photo; audioAttachment = audio; attachmentName = aName;
    }
}

public class EmailDatabase : MonoBehaviour
{
    [Header("Day 2 Evidence Assets")]
    public Sprite photoCCTVStill;
    public Sprite photoReyesLocker;
    public Sprite photoSoilReject;
    public Sprite photoVocolithDoc;
    public Sprite photoVictimSite;
    public Sprite photoPayload99;
    public Sprite photoResortPool;
    public Sprite photoDikeCloseup;
    public Sprite photoOfficialSeal;
    public Sprite photoStormMap;
    public Sprite photoMatapangIncident;
    public AudioClip audioWiretap22A;

    public List<Email> GetDayContent(int day)
    {
        List<Email> inbox = new List<Email>();

        if (day == 1)
        {
            inbox.Add(new Email("HR: WELCOME PO!", "Hi Paul! Welcome to the Admin team. You're the new lead clerk for Station 8802. Task mo is to just clean up the 'Proyektong Agos' database. Make sure you clear the backlogs from the guy before you, okay? Pag may nag-leak, literally terminated agad. G?", false));
            inbox.Add(new Email("IT: SECURITY REMINDER", "Friendly reminder: We’re monitoring your workstation for quality assurance. We caught some unauthorized 'pings' from outside nodes lately. Super sketch. Please update your encryption keys and 'wag iwanan yung terminal.", false));
            inbox.Add(new Email("MERCY GENERAL: SURGERY UPDATE", "Hi Mr. Paul, just a formal confirmation na the downpayment for 'MA. LUZ DE LA CRUZ' is fully paid na. Thanks to the Mayor’s SPECIAL FUND. Next Tuesday na yung heart valve replacement. Make sure the patient is chill lang and low-stress. Grabe, the Mayor is so bait for supporting us!", true));
            inbox.Add(new Email("MAYOR'S OFFICE: AGOS PHASE 1", "URGENT: Finalizing the drainage maps for Brgy. Makahiya. Since andun yung private estate ni Mayor, that area is the top priority, obviously. You're instructed to mark all soil tests as 'Optimal' and 'Highly Compatible.' Don't mind the technical data from the engineers. We cannot have delays, so please make it look good.", true));
            inbox.Add(new Email("BUDGET: ALLOCATION 2026", "BUDGET SUMMARY FOR PROYEKTONG AGOS:\n\nBrgy. Makahiya (Elite Zone): 15.2M PHP\nBrgy. Matulungin: 1.5M PHP\nBrgy. Matapang (Basin): 0.00 PHP\n\nNote: The budget for the Brgy. Matapang dike? Like, re-routed na to 'Administrative Supplies' per Mayor's order. Reconcile the ledger to reflect this change, please.", true));
            inbox.Add(new Email("PROCUREMENT: VOCOLITH CONST.", "Purchase Order #9902: 500 Bags of Cement. Deliver to: Private Residence Lot 12, Makahiya Heights. Tag this as 'Matapang Flood Wall Maintenance' sa system, okay? Make sure Vocolith gets paid by Friday. The Mayor wants this to be handled discreetly. Keep it lowkey.", true));
            inbox.Add(new Email("INTERNAL MEMO: CLERK REYES", "To all staff: Effective immediately, Clerk Reyes is no longer with the office. Voluntary resignation daw, super final. For security reasons, 'wag niyo siyang i-chat or call. Anything he left sa desk, HR na ang bahala.", false));
            inbox.Add(new Email("CITIZEN COMPLAINT: BRGY. MATAPANG", "Sir, I’m literally desperate na. The dike here in Matapang is cracking. It’s not even concrete, parang dirt lang. We were promised 'Agos' funds months ago. Takot na yung mga kids ko. Please help us.", true));
            inbox.Add(new Email("FAMILY: TITO LITO", "Paul! I just visited your mom at Mercy General. She's so happy, literally crying. Sabi niya the Mayor visited her and told her not to worry about the bills kasi you're a 'loyal worker.' We are so lucky! Don't do anything to lose it, okay?", false));
            inbox.Add(new Email("SYSTEM: LOGOUT REMINDER", "End of shift reached. System Audit requires that all sensitive files regarding land acquisition are SCRUBBED from the local drive. Protect the office, protect your career.", true));
        }
        else if (day == 2)
        {
            // === PART 1: THE CORRUPTION ===
            inbox.Add(new Email("SECURITY: SKETCHY GUY", "Someone looking like Clerk Reyes was spotted near the server room around 2:00 AM. Super sus. Security is on high alert now.", true, photoCCTVStill, null, "CCTV_044.img"));
            inbox.Add(new Email("HR: REYES' STUFF", "We found a yellow sticky note stuck sa ilalim ng desk mo. HR thinks it’s encrypted passwords. If you find it, surrender it to HR immediately. Don't read it.", false, photoReyesLocker, null, "REYES_LOCKER.img"));
            inbox.Add(new Email("CITIZEN: BRGY. MATULUNGIN", "The rains haven't even peaked yet but yung main road dito, like, waist-deep na. Nasaan na yung 'Agos' money?", true, photoVictimSite, null, "VICTIM_SITE.img"));
            inbox.Add(new Email("WIRETAP: MAYOR & CHIEF", "Corrupted audio of Call 22-A. The Mayor is telling the Chief to 'handle' the protestors sa lowland area. Mark this as 'SPAM' then purge.", true, null, audioWiretap22A, "REDACTED.mp3"));
            inbox.Add(new Email("ENGINEERING: SOIL REPORT", "TECHNICAL DATA: The foundation for the Matapang dike? Totally REJECTED. But Mayor overruled this. Evidence of the rejection stamp is attached.", true, photoSoilReject, null, "SOIL_REJECT.img"));
            inbox.Add(new Email("IT: DATA LEAK WARNING", "Someone is trying to tunnel into Station 8802 using a 'Ghost' account. Don't answer anything that looks like a terminal prompt.", true));
            inbox.Add(new Email("MAYOR'S OFFICE: LOYALTY CHECK", "Paul, the Mayor noticed you were viewing 'Phase 2' files for a bit too long. Remember who’s paying for your mom’s surgery. Focus on your work, okay?", true, photoOfficialSeal, null, "OFFICIAL_SEAL.img"));
            inbox.Add(new Email("LEDGER: AGOS PHASE 2", "PROJECT: AGOS PHASE 2\nCONTRACTOR: VOCOLITH\nBUDGET: 50,000,000 PHP\nACTUAL SPEND: 2,500,000 PHP\n\nNotes: The dike in Matapang is literally just a pile of dirt.", true, photoVocolithDoc, null, "VOCOLITH_DOC.img"));

            // === PART 2: THE BRUTAL TURN ===
            inbox.Add(new Email("INTERNAL: 'AGOS' CLEANUP", "Paul, clean the ledger now. We had a 'situation' sa Matapang protest site last night. Pag walang record ng people, walang record ng crackdown. We pay you for this, right?", true, photoMatapangIncident, null, "MATAPANG_INCIDENT.img"));
            inbox.Add(new Email("SYSTEM: COMPROMISED LEDGER", "CRITICAL ERROR. Someone restored the Phase 2 docs. The ledger shows 'Vocolith' used flood funds to build a 2M PHP swimming pool. Delete this whole folder.", true, photoResortPool, null, "RESORT_POOL.img"));
            inbox.Add(new Email("REYES: THEY ARE HERE.", "Paul, if you're reading this, nahanap na nila ako. Look at the photo. Those aren't protestors—those are the bodies of people who knew too much.", true));
            inbox.Add(new Email("VOCOLITH: 'LOGISTICS COST'", "Billing update: New item for 'Evidence Disposal' sa site (1.5M PHP). This includes concrete and deep disposal for 'project impediments.' Label this as 'Maintenance.'", true, photoPayload99, null, "DISPOSAL_COSTS.img"));
            inbox.Add(new Email("MERCY GENERAL: VISITOR NOTICE", "Paul, some guys in suits just arrived sa room ng mom mo. Sabi nila Mayor's orders daw to 'monitor' her. I can't stop them, Paul. They look super scary.", true));
            inbox.Add(new Email("SOS: LAST MESSAGE", "They are purging everyone before the storm hits. Everyone who complained is being taken. We're done for.", true, photoDikeCloseup, null, "THE_PURGE.img"));
            inbox.Add(new Email("URGENT: SYSTEM LOG", "FINAL WARNING: Station 8802 is compromised. Purge the Agos directory now or the system will purge you too. Choose your family, Paul.", true, photoStormMap, null, "SYSTEM_ERR.img"));
        }
        else if (day == 3)
        {
            inbox.Add(new Email("STORM ALERT: SIGNAL #4", "The storm hit na. Brgy. Matapang is 4 feet deep. The dirt dike is failing, obviously. Delete all incoming SOS messages immediately.", true));
            inbox.Add(new Email("MAYOR: THE CLEANUP", "Wipe everything. The whole 'Agos' folder. If the files disappear, the failure is just an 'Act of God' and not us. This is for your mom, Paul.", true));
            inbox.Add(new Email("SOS: BRGY. MATAPANG #1", "PLEASE HELP US. THE WATER IS AT THE ROOF. THE DIKE BROKE NA. WE ARE TRAPPED SA ATTIC.", true));
            inbox.Add(new Email("SOS: BRGY. MATAPANG #2", "NASAAN YUNG RESCUE? ILANG ORAS NA KAMI NAG-E-EMAIL. THE WATER IS SO COLD. PLEASE ANSWER.", true));
            inbox.Add(new Email("SYSTEM: BREACH DETECTED", "CRITICAL: Internal files are being copied to an external drive. Station 8802 is the source. STOP THE PROCESS NOW.", true));
            for (int i = 6; i <= 15; i++)
                inbox.Add(new Email("Corrupted Citizen Log #" + i, "They are drowning because you're deleting the proof. Look at the water, Paul. It's on your hands.", true));
            inbox.Add(new Email("MERCY GENERAL: EMERGENCY", "Paul, it's Nurse Sarah. The guys in suits are moving your mom. She's so scared. She won't go until she talks to you. Is this part of the plan?", true));
            inbox.Add(new Email("REYES: THE TRUTH", "The Mayor is using your mom as a hostage, Paul. I have the drive ready. If you don't keep the files, those people died for nothing. LEAK IT.", true));
            inbox.Add(new Email("MAYOR: FINAL ORDER", "I have your mother, Paul. Delete the last 3 files and log out. Betray me, and she never leaves that van. Choose wisely.", true));
            inbox.Add(new Email("SYSTEM: CRITICAL FAILURE", "99% compromised. Station 8802 is shutting down. The breach is complete.", true));
            inbox.Add(new Email("FINAL LOG: THE DECISION", "THE WATER IS AT THE LOBBY. LEAK THE DATA (/keep) OR PROTECT THE MAYOR (/delete). IT'S LITERALLY UP TO YOU.", true));
        }
        return inbox;
    }
}