using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Email
{
    public string subject;
    [TextArea(5, 12)] public string[] bodyPages;
    public bool isProcessed, isSensitive;
    public Sprite photoAttachment;
    public AudioClip audioAttachment;
    public string attachmentName;

    public Email(string s, string[] b, bool sensitive, Sprite photo = null, AudioClip audio = null, string aName = "")
    {
        subject = s;
        bodyPages = b;
        isSensitive = sensitive;
        isProcessed = false;
        photoAttachment = photo;
        audioAttachment = audio;
        attachmentName = aName;
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
            inbox.Add(new Email("HR: WELCOME PO!", new string[] { "Hi Paul! Welcome sa Admin team. Ikaw ang bagong lead clerk ng Station 8802.", "Ang trabaho mo ay linisin ang 'Proyektong Agos' database. Burahin ang mga backlogs. Pag may nag-leak, terminated ka agad. G?" }, false));
            inbox.Add(new Email("IT: SECURITY REMINDER", new string[] { "Paalala lang: Binabantayan namin ang workstation mo. May mga unauthorized 'pings' kaming nakukuha mula sa labas.", "Maging maingat. I-update ang encryption keys at huwag iiwanang bukas ang terminal." }, false));
            inbox.Add(new Email("ST.JOSEPH HOSPITAL: SURGERY UPDATE", new string[] { "Magandang araw Mr. Paul, kumpirmado na po ang downpayment para sa operasyon ng nanay niyo. Salamat sa SPECIAL FUND ni Mayor.", "Sa Martes na ang heart valve replacement. Siguraduhing hindi na-stress ang pasyente. Napakabuti talaga ni Mayor!" }, true));
            inbox.Add(new Email("MAYOR'S OFFICE: AGOS PHASE 1", new string[] { "URGENTE: Tinatapos na ang drainage maps para sa Brgy. Makahiya. Dahil nandoon ang private estate ni Mayor, priority ang area na 'yan.", "Markahan ang lahat ng soil tests bilang 'Optimal.' Huwag pansinin ang data mula sa mga engineer. Bawal ang delay." }, true));
            inbox.Add(new Email("BUDGET: ALLOCATION 2026", new string[] { "BUDGET SUMMARY - PROYEKTONG AGOS:\n\nBrgy. Makahiya: 15.2M PHP\nBrgy. Matulungin: 1.5M PHP\nBrgy. Matapang: 0.00 PHP", "Note: Ang budget para sa dike sa Matapang ay nailipat na sa 'Administrative Supplies' per order ni Mayor. I-reconcile ang ledger." }, true));
            inbox.Add(new Email("PROCUREMENT: VOCOLITH CONST.", new string[] { "Purchase Order #9902: 500 Bags ng Semento. Deliver sa: Private Residence Lot 12, Makahiya Heights.", "I-tag ito bilang 'Matapang Flood Wall Maintenance.' Siguraduhing mababayaran ang Vocolith sa Biyernes. Discreet lang tayo." }, true));
            inbox.Add(new Email("INTERNAL MEMO: CLERK REYES", new string[] { "Sa lahat: Effective immediately, wala na sa opisina si Clerk Reyes. Voluntary resignation daw, super final.", "Para sa security, huwag na siyang i-contact. Anumang gamit na naiwan niya, HR na ang bahala." }, false));
            inbox.Add(new Email("CITIZEN COMPLAINT: BRGY. MATAPANG", new string[] { "Sir, nagmamakaawa po kami. May mga bitak na ang dike rito sa Matapang. Parang lupa lang ang itinambak nila.", "Ilang buwan na kaming nanghihingi ng tulong. Takot na ang mga anak ko. Tulungan niyo po kami." }, true));
            inbox.Add(new Email("FAMILY: TITO LITO", new string[] { "Paul! Kakagaling ko lang sa Mercy General. Tuwang-tuwa ang nanay mo, umiiyak pa nga.", "Binisita raw siya ni Mayor at sinabing 'wag mag-alala sa bill dahil 'loyal worker' ka raw. Napakaswerte natin!" }, false));
            inbox.Add(new Email("SYSTEM: LOGOUT REMINDER", new string[] { "Tapos na ang shift. Siguraduhing lahat ng sensitive files ay BURADO na sa local drive.", "Protektahan ang opisina, protektahan ang career mo." }, true));
        }
        else if (day == 2)
        {
            inbox.Add(new Email("SECURITY: SKETCHY GUY", new string[] { "May nakitang kamukha ni Clerk Reyes malapit sa server room kaninang 2:00 AM. Napaka-suspisyoso.", "Naka-high alert na ang security ngayon." }, true, photoCCTVStill, null, "CCTV_044.img"));
            inbox.Add(new Email("HR: REYES' STUFF", new string[] { "May nahanap kaming yellow sticky note sa ilalim ng desk mo. Tingin ng HR, encrypted passwords ito.", "I-surrender agad ito sa HR. Huwag mong babasahin." }, false, photoReyesLocker, null, "REYES_LOCKER.img"));
            inbox.Add(new Email("CITIZEN: BRGY. MATULUNGIN", new string[] { "Hindi pa man lumalakas ang ulan pero hanggang kirot na ang baha rito. Nasaan na ang pera ng 'Agos'?", "Wala kaming makitang trabahador dito." }, true, photoVictimSite, null, "VICTIM_SITE.img"));
            inbox.Add(new Email("WIRETAP: MAYOR & CHIEF", new string[] { "Corrupted audio ng Call 22-A.", "Inuutusan ni Mayor ang Chief na 'ayusin' ang mga nagpo-protesta sa lowland area. I-tag bilang SPAM at burahin." }, true, null, audioWiretap22A, "REDACTED.mp3"));
            inbox.Add(new Email("ENGINEERING: SOIL REPORT", new string[] { "TECHNICAL DATA: Ang pundasyon para sa Matapang dike? REJECTED.", "Pero ni-overrule ito ni Mayor. Ang ebidensya ng rejection stamp ay naka-attach." }, true, photoSoilReject, null, "SOIL_REJECT.img"));
            inbox.Add(new Email("IT: DATA LEAK WARNING", new string[] { "May sumusubok mag-tunnel sa Station 8802 gamit ang isang 'Ghost' account.", "Huwag sasagot sa anumang terminal prompt." }, true));
            inbox.Add(new Email("MAYOR'S OFFICE: LOYALTY CHECK", new string[] { "Paul, napansin ni Mayor na matagal mong tinititigan ang 'Phase 2' files. Isipin mo ang nanay mo.", "Isipin mo kung sino ang nagbabayad ng surgery niya. Focus ka lang sa trabaho." }, true, photoOfficialSeal, null, "OFFICIAL_SEAL.img"));
            inbox.Add(new Email("LEDGER: AGOS PHASE 2", new string[] { "PROJECT: AGOS PHASE 2\nCONTRACTOR: VOCOLITH\nBUDGET: 50,000,000 PHP", "ACTUAL SPEND: 2,500,000 PHP\n\nNotes: Ang dike sa Matapang ay tumpok lang ng lupa." }, true, photoVocolithDoc, null, "VOCOLITH_DOC.img"));
            inbox.Add(new Email("INTERNAL: 'AGOS' CLEANUP", new string[] { "Paul, linisin mo na ang ledger. Nagkaroon ng 'insidente' sa Matapang kagabi.", "Pag walang record ng tao, walang record ng crackdown. Binabayaran ka namin para rito, 'di ba?" }, true, photoMatapangIncident, null, "MATAPANG_INCIDENT.img"));
            inbox.Add(new Email("SYSTEM: COMPROMISED LEDGER", new string[] { "CRITICAL ERROR. May nag-restore ng Phase 2 docs.", "Ginamit ng Vocolith ang flood funds para sa 2M PHP swimming pool. I-delete ang folder na ito." }, true, photoResortPool, null, "RESORT_POOL.img"));
            inbox.Add(new Email("REYES: NASAAN NA SILA.", new string[] { "Paul, kung nababasa mo ito, nahanap na nila ako.", "Tingnan mo ang litrato. Hindi 'yan mga protestors—katawan 'yan ng mga taong masyadong maraming alam." }, true));
            inbox.Add(new Email("VOCOLITH: 'LOGISTICS COST'", new string[] { "Billing update: Bagong item para sa 'Evidence Disposal' (1.5M PHP).", "Kasama rito ang semento para sa paglilibing ng 'mga sagabal sa proyekto.' I-label bilang Maintenance." }, true, photoPayload99, null, "DISPOSAL_COSTS.img"));
            inbox.Add(new Email("MERCY GENERAL: VISITOR NOTICE", new string[] { "Paul, may mga lalaking naka-suit na dumating sa room ng nanay mo. Orders daw ni Mayor.", "Hindi ko sila mapigilan. Paul, nakakatakot ang itsura nila." }, true));
            inbox.Add(new Email("SOS: LAST MESSAGE", new string[] { "Pinapatay nila lahat bago tumama ang bagyo.", "Lahat ng nagreklamo, kinukuha na nila. Katapusan na natin." }, true, photoDikeCloseup, null, "THE_PURGE.img"));
            inbox.Add(new Email("URGENT: SYSTEM LOG", new string[] { "HULING BABALA: Compromised na ang Station 8802. Burahin na ang Agos directory.", "Piliin mo ang pamilya mo, Paul." }, true, photoStormMap, null, "SYSTEM_ERR.img"));
        }
        else if (day == 3)
        {
            inbox.Add(new Email("STORM ALERT: SIGNAL #4", new string[] { "Tumama na ang bagyo. 4 feet deep na sa Matapang. Bumigay na ang dike. I-delete lahat ng SOS." }, true));
            inbox.Add(new Email("MAYOR: THE CLEANUP", new string[] { "I-wipe lahat. Ang buong 'Agos' folder. 'Act of God' na ito at hindi sa atin. Para ito sa nanay mo." }, true));
            inbox.Add(new Email("SOS: BRGY. MATAPANG #1", new string[] { "TULONG PO. NASA BUBUNGAN NA KAMI. BUMIGAY NA ANG DIKE. TRAPPED KAMI RITO." }, true));
            inbox.Add(new Email("SOS: BRGY. MATAPANG #2", new string[] { "NASAAN NA ANG RESCUE? ANG LAMIG NA NG TUBIG. SUMAGOT KAYO." }, true));
            inbox.Add(new Email("SYSTEM: BREACH DETECTED", new string[] { "CRITICAL: May kumokopya ng internal files sa external drive. ITIGIL ITO NGAYON DIN." }, true));
            for (int i = 6; i <= 15; i++)
                inbox.Add(new Email("Corrupted Log #" + i, new string[] { "Nalulunod sila dahil binubura mo ang ebidensya. Tingnan mo ang kamay mo, Paul." }, true));
            inbox.Add(new Email("MERCY GENERAL: EMERGENCY", new string[] { "Paul, si Nurse Sarah ito. Inililipat na ng mga lalaki ang nanay mo. Takot na takot siya." }, true));
            inbox.Add(new Email("REYES: ANG KATOTOHANAN", new string[] { "Ginagawang hostage ni Mayor ang nanay mo. I-leak mo ang files para hindi masayang ang buhay nila." }, true));
            inbox.Add(new Email("MAYOR: FINAL ORDER", new string[] { "Hawak ko ang nanay mo, Paul. Burahin ang huling 3 files at mag-logout. Kung hindi, hindi na siya makakalabas." }, true));
            inbox.Add(new Email("SYSTEM: CRITICAL FAILURE", new string[] { "99% compromised. Station 8802 is shutting down." }, true));
            inbox.Add(new Email("FINAL LOG: ANG DESISYON", new string[] { "I-LEAK ANG DATA (/keep) O PROTEKTAHAN SI MAYOR (/delete). NASA KAMAY MO NA ITO." }, true));
        }
        return inbox;
    }
}