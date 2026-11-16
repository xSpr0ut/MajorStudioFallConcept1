using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note prefabs here")]
    [SerializeField] private GameObject barNote;
    [SerializeField] private GameObject longBarNote;
    [SerializeField] private GameObject circleNote;
    [SerializeField] private GameObject longCircleNote;
    [SerializeField] private GameObject dialogueNote;

    [Header("Note locations here")]
    [SerializeField] private List<Transform> level1locs = new List<Transform>();

    [Header("Colour Stuff")]
    private Color[] noteColours = {new Color32(0xFF, 0x7A, 0xA9, 0xFF), new Color32(0x77, 0xB8, 0xE0, 0xFF)};

    private string[] twoADialogue;
    private string[] twoBDialogue;

    private void Start()
    {
        twoADialogue = new string[10];
        twoADialogue[0] = "Do you want to go on a date or something?";
        twoADialogue[1] = "You're kind of cute…";
        twoADialogue[2] = "If you're trying to seduce me—never mind.";
        twoADialogue[3] = "Whatever you say!";
        twoADialogue[4] = "I suppose I could go with you.";
        twoADialogue[5] = "I'll hear you out.";
        twoADialogue[6] = "You're quite harsh.";
        twoADialogue[7] = "Don’t be such a party pooper.";
        twoADialogue[8] = "If you keep talking like that, I'm calling security.";
        twoADialogue[9] = "Well, aren't you a rude b!tch.";

        twoBDialogue = new string[10];
        twoBDialogue[0] = "I can tell you're just sucking up to me.";
        twoBDialogue[1] = "Overdoing it much?";
        twoBDialogue[2] = "Well, you're clearly too charming to keep this job.";
        twoBDialogue[3] = "What are you trying to get at?";
        twoBDialogue[4] = "I'll make an exception just this once.";
        twoBDialogue[5] = "I suppose you do have a good track record…";
        twoBDialogue[6] = "Your logic isn’t sound.";
        twoBDialogue[7] = "Think too much? Or maybe, not enough?";
        twoBDialogue[8] = "You can't reason your way out of this.";
        twoBDialogue[9] = "I have more than enough reason to fire you.";
    }


    public List<Transform> level4locs = new List<Transform>();

    public Note spawnVisualCue(VisualCue cue, double startTime, double noteSpeed, float score, string songName)
    {
        // create a note prefab
        GameObject n;

        GameObject notePrefab = null;

        Vector3 startLoc = Vector3.zero;
        Vector3 endLoc = Vector3.zero;

        switch(cue.type)
        {
            case (VisualCue.NoteTypes.BAR):
                notePrefab = barNote;
                endLoc = level4locs[cue.lane].position;
                startLoc = new Vector3(endLoc.x, endLoc.y + 10, endLoc.z);
                break;
            case (VisualCue.NoteTypes.LONGBAR):
                notePrefab = longBarNote;
                endLoc = level4locs[cue.lane].position;
                startLoc = new Vector3(endLoc.x, endLoc.y + 10, endLoc.z);
                break;
            case (VisualCue.NoteTypes.CIRCLE):
                notePrefab = circleNote;
                startLoc = level1locs[cue.lane].position;
                endLoc = startLoc;
                break;
            case (VisualCue.NoteTypes.LONGCIRCLE):
                notePrefab = longCircleNote;
                startLoc = level1locs[cue.lane].position;
                endLoc = startLoc;
                break;
            case (VisualCue.NoteTypes.DIALOGUE):
                notePrefab = dialogueNote;
                startLoc = Vector3.zero;
                break;
        }

        n = GameManager.Instance.spawnAtLocation(notePrefab, startLoc);

        // picking colours
        Color noteColour = Color.white;
        if(cue.colour >= 0 && cue.colour < noteColours.Length)
        {
            noteColour = noteColours[cue.colour];
        }
        else
        {
            noteColour = Color.white;
        }

        n.GetComponent<Note>().setVars(cue.spawnTime + startTime,
                                       cue.actualTime + startTime,
                                       noteSpeed,
                                       endLoc, noteColour);


        if (cue.hold)
        {
            n.GetComponent<Note>().setLongNoteVars(cue.topSpawnTime + startTime);
        }

        if (cue.type == VisualCue.NoteTypes.CIRCLE)
        {
            n.transform.localScale = new Vector3(0, 0, 1);
        }
        else if (cue.type == VisualCue.NoteTypes.DIALOGUE)
        {
            n.GetComponent<NoteDialogue>().changeText(getCorrectDialogue(score, songName));
        }
        // lot of shuffling but you assign the note object to the Note value in one of the VisualCues in the current lane
        int currentLane = cue.lane;

        return n.GetComponent<Note>();
    }

    private string getCorrectDialogue(float score, string songName)
    {
        int index = 0;
        if (score <= -35)
        {
            index = 0;
        }
        else if (score <= -15)
        {
            index = 2;
        }
        else if (score <= 15)
        {
            index = 4;
        }
        else if (score <= 35)
        {
            index = 6;
        }
        else
        {
            index = 8;
        }

        if (songName.Equals("Level 2A"))
        {
            return twoADialogue[Random.Range(index, index + 1)];
        }
        else
        {
            return twoADialogue[Random.Range(index, index + 1)];
        }
    }
}
