using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note prefabs here")]
    [SerializeField] private GameObject barNote;
    [SerializeField] private GameObject longBarNote;
    [SerializeField] private GameObject circleNote;
    [SerializeField] private GameObject longCircleNote;

    [Header("Note locations here")]
    [SerializeField] private List<Transform> level1locs = new List<Transform>();

    [Header("Colour Stuff")]
    private Color[] noteColours = {new Color32(0xFF, 0x7A, 0xA9, 0xFF), new Color32(0x77, 0xB8, 0xE0, 0xFF)};


    public List<Transform> level4locs = new List<Transform>();

    public Note spawnVisualCue(VisualCue cue, double startTime, double noteSpeed)
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
        }

        n = GameManager.Instance.spawnAtLocation(notePrefab, startLoc);

        // picking colours
        Color noteColour = Color.white;
        if(cue.colour >= 0 && cue.colour < noteColours.Length)
        {
            noteColour = noteColours[cue.colour];
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

        // lot of shuffling but you assign the note object to the Note value in one of the VisualCues in the current lane
        int currentLane = cue.lane;

        return n.GetComponent<Note>();
    }
}
