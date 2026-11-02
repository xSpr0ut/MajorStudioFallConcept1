using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

// unbastardized documentation https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html
public class GameManager : MonoBehaviour
{
    #region vars and such
    
    private SongSelector.Song selectedSong;

    [Header("Note info")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject longNotePrefab;

    [SerializeField] private double noteSpeed = 1; // in units/second, player controlled

    [SerializeField] private float initialHeight = 5f;
    [SerializeField] private float endHeight = 0f;
    [SerializeField] private float initialDistFromPlayer = 5f;
    [SerializeField] private float endDistFromPlayer = 5;

    [SerializeField] private bool spawnHorizontally = true;

    [Header("Pre integration ONLY. Remove when done")]
    public AudioSource audioSrc;
    public AudioClip defaultClip;

    [Header("Keys")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private List<Sprite> arrows = new List<Sprite>();

    private double startDelay = 0;
    private double startTime = 0;
    private double gameEndTime = 0;

    //lists of all events
    List<AudioCue> audioCues = new List<AudioCue>();
    List<VisualCue> visualCues = new List<VisualCue>();

    // keep track of positions in the list (instead of removing them for easy replayability)
    private int currentAudioCue = 0;
    private int currentVisualCue = 0;
    
    private int[] currentNoteSpawning = new int[8]; // keeping track of which position each lane is in
    private int[] closestNoteToBottom = new int[8]; // keeping track of the lowest note in each lane

    //list of all notes
    List<VisualCue>[] allLanes = new List<VisualCue>[8];
    List<GameObject> keyLines = new List<GameObject>();

    // game variables
    private ChartParser chartParser;
    [SerializeField] private Scoring scorer;
    private bool started = false;

    // singleton bs
    private static GameManager Manager;
    public static GameManager Instance { get { return Manager; } }
    #endregion
    private void Awake()
    {
        if (Manager != null && Manager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Manager = this;
        }

    }
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        // these may take a while
        Resources.LoadAll("Resouces"); // CHANGE LATER WHEN ORGANIZED
    }

    private void Update()
    {
        if (started)
        {
            double currentTime = AudioSettings.dspTime; // current sample position

            spawnAudioCue(currentTime);

            spawnVisualCue(currentTime, spawnHorizontally);

            checkMissedNotes(currentTime);

            if (gameEndTime <= currentTime)
            {
                endGame();
            }

            detectKeyPress();
        }
    }

    public void startGame(SongSelector.Song songToPlay)
    {
        scorer.setEndHeight(endHeight);

        chartParser = new ChartParser();
        selectedSong = songToPlay;
        chartParser.parseChart(selectedSong, noteSpeed, initialHeight);

        visualCues = chartParser.getVisualCues();
        audioCues = chartParser.getAudioCues();
        allLanes = chartParser.getAllLanes();

        // it's possible some events start very early, so we need to make sure that they start properly
        // also, I anticipate the above two methods taking a long time, so dspTime will definitely be positive when update comes around
        if (audioCues.Count > 0 && visualCues.Count > 0)
        {
            if (audioCues[0].scheduleTime < 0 && audioCues[0].scheduleTime < visualCues[0].spawnTime)
            {
                startDelay = Math.Abs(audioCues[0].scheduleTime) + 1;
            }
            else if (visualCues[0].spawnTime < 0 && visualCues[0].spawnTime < audioCues[0].scheduleTime)
            {
                startDelay = Math.Abs(visualCues[0].spawnTime) + 1;
            }
        }

        // lines to indicate timing
        for (int i = 0; i < 8; i++)
        {
            var k = spawnAtLocation(keyPrefab, new Vector3(-4 + i, 0, 0));

            //Image keyRect = k.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            //keyRect.sprite = arrows[i];
            ////keyRect.rotation = Quaternion.Euler(0, 90, i * 45);
            //k.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = i.ToString();
            keyLines.Add(k);
        }

        scorer.gameObject.SetActive(true);

        AudioClip a = Resources.Load<AudioClip>("Audio/" + selectedSong.musicFilePath + audioCues[currentAudioCue].name.Replace(".wav", ""));

        started = true;
        startTime = AudioSettings.dspTime + startDelay;

        gameEndTime = Math.Max(a.length, visualCues[visualCues.Count - 1].actualTime + (scorer.getMissRange())/1000) + startTime + 1f;
    }

    public void endGame()
    {
        started = false;
        scorer.endGame();
    }
    private void spawnAudioCue(double currentTime)
    {
        double songTime = currentTime - startTime; // track/trajectory time: 0 == backing-track start

        // sendToMax == false: Unity plays the backing track locally from the note chart's audio cues.
        while (currentAudioCue < audioCues.Count && audioCues[currentAudioCue].actualTime + startTime <= currentTime)
        {
            AudioClip a = Resources.Load<AudioClip>("Audio/" + selectedSong.musicFilePath + audioCues[currentAudioCue].name.Replace(".wav", ""));
            if (a != null) AudioSource.PlayClipAtPoint(a, new Vector3(0, 0, 0));
            currentAudioCue++;
        }
    }

    private void spawnVisualCue(double currentTime, bool vertical)
    {
        // spawning notes
        while (currentVisualCue < visualCues.Count && visualCues[currentVisualCue].spawnTime + startTime <= currentTime)
        {
            // create a note prefab
            GameObject n;
            if (visualCues[currentVisualCue].hold)
            {
                if (spawnHorizontally)
                {
                    n = spawnAtLocation(longNotePrefab, new Vector3(visualCues[currentVisualCue].lane - 4, initialHeight, 0));
                }
                else
                {
                    n = spawnAtLocation(longNotePrefab, new Vector3(visualCues[currentVisualCue].lane - 4, initialHeight, 0));
                }
                
            }
            else
            {
                if (spawnHorizontally)
                {
                    n = spawnAtLocation(notePrefab, new Vector3(visualCues[currentVisualCue].lane - 4, initialHeight, 0));
                }
                else
                {
                    n = spawnAtLocation(notePrefab, new Vector3(visualCues[currentVisualCue].lane - 4, initialHeight, 0));
                }
                
            }

            

            Vector3 endlocation = new Vector3(keyLines[visualCues[currentVisualCue].lane].transform.position.x,
                                                             keyLines[visualCues[currentVisualCue].lane].transform.position.y,
                                                             keyLines[visualCues[currentVisualCue].lane].transform.position.z); // lol

            n.GetComponent<Note>().setVars(visualCues[currentVisualCue].spawnTime + startTime,
                                           visualCues[currentVisualCue].actualTime + startTime,
                                           noteSpeed,
                                           endlocation, Color.white);

            //n.GetComponent<Note>().startTime = visualCues[currentVisualCue].spawnTime + startTime;
            //n.GetComponent<Note>().endTime = visualCues[currentVisualCue].actualTime + startTime;
            //n.GetComponent<Note>().fallSpd = noteSpeed;
            ////n.GetComponent<Note>().endHeight = endHeight;
            //n.GetComponent<Note>().endLocation = new Vector3(keyLines[visualCues[currentVisualCue].lane].transform.position.x,
            //                                                 keyLines[visualCues[currentVisualCue].lane].transform.position.y,
            //                                                 keyLines[visualCues[currentVisualCue].lane].transform.position.z); // lol

            if (visualCues[currentVisualCue].hold)
            {
                n.GetComponent<NoteBarLong>().topStartTime = visualCues[currentVisualCue].topSpawnTime + startTime;
            }

            // lot of shuffling but you assign the note object to the Note value in one of the VisualCues in the current lane
            int currentLane = visualCues[currentVisualCue].lane;

            allLanes[currentLane][currentNoteSpawning[currentLane]].note = n.GetComponent<Note>();

            currentVisualCue++;
            currentNoteSpawning[currentLane]++;
        }
    }

    private void checkMissedNotes(double currentTime)
    {
        // check if any notes escape from the bottom
        for (int i = 0; i < allLanes.Length; i++)
        {
            int count = 0;
            while (closestNoteToBottom[i] < allLanes[i].Count && allLanes[i][closestNoteToBottom[i]].evaluationTime + startTime + (double)scorer.getMissRange() / 1000.0 <= currentTime)
            {
                // sometimes it breaks idk
                if (allLanes[i][closestNoteToBottom[i]].note != null)
                {
                    Destroy(allLanes[i][closestNoteToBottom[i]].note.gameObject);
                    allLanes[i][closestNoteToBottom[i]].note = null;

                    scorer.inputScore(scorer.getMiss(), i);

                    closestNoteToBottom[i]++;
                    continue;
                }

                count++;
                if (count == 5)
                {
                    Debug.Log("OOPS! INIFNITE LOP " + (allLanes[i][closestNoteToBottom[i]].actualTime + startTime) + (double)scorer.getMissRange() / 1000.0 + "   " + currentTime);
                    break;
                }
            }

            while (closestNoteToBottom[i] < allLanes[i].Count &&
                   allLanes[i][closestNoteToBottom[i]].hold &&
                   allLanes[i][closestNoteToBottom[i]].evaluationTime + startTime + (double)scorer.getMissRange() / 1000.0 <= currentTime)
            {
                // sometimes it breaks idk
                if (allLanes[i][closestNoteToBottom[i]].note != null)
                {
                    if (!allLanes[i][closestNoteToBottom[i]].hit)
                    {
                        allLanes[i][closestNoteToBottom[i]].note.disableNote();
                        allLanes[i][closestNoteToBottom[i]].note = null;

                        scorer.inputScore(scorer.getMiss(), i);

                        closestNoteToBottom[i]++;
                        continue;
                    }
                }

                count++;
                if (count == 5)
                {
                    Debug.Log("OOPS! INIFNITE LOP HOLD" + (allLanes[i][closestNoteToBottom[i]].actualTime + startTime) + (double)scorer.getMissRange() / 1000.0 + "   " + currentTime);
                    break;
                }
            }

        }
    }
    private void detectKeyPress()
    {
        for (int i = 0; i < 8; i++)
        {
            // Check Top Row Numbers (KeyCode.Alpha0 is 48, Alpha1 is 49...)
            KeyCode topRowKey = KeyCode.Alpha0 + i;

            if (Input.GetKeyDown(topRowKey))
            {
                resolveHit(i);
            }

            if (Input.GetKeyUp(topRowKey))
            {
                resolveRelease(i);
            }
        }
    }
    public void resolveHit(int lane)
    {
        if (closestNoteToBottom[lane] < allLanes[lane].Count) // if there are notes in the lane
        {
            // time difference = time labelled on note - current time
            double timeDifference = Math.Abs(((double)allLanes[lane][closestNoteToBottom[lane]].actualTime + startTime) - AudioSettings.dspTime);

            // if the note is nowhere near the player, don't accept input
            if (timeDifference > (double)scorer.getMissRange() / 1000)
            {
                return;
            }

            scorer.inputScore(timeDifference, lane);

            if (!String.IsNullOrEmpty(allLanes[lane][closestNoteToBottom[lane]].name))
            {
                AudioClip clip = defaultClip;
                clip = Resources.Load<AudioClip>(selectedSong.musicFilePath + allLanes[lane][closestNoteToBottom[lane]].name.Replace(".wav", ""));
                audioSrc.PlayOneShot(clip);
            }

            allLanes[lane][closestNoteToBottom[lane]].hit = true;
            if (allLanes[lane][closestNoteToBottom[lane]].hold)
            {
                // lmfao
                allLanes[lane][closestNoteToBottom[lane]].note.gameObject.GetComponent<NoteBarLong>().onHold();
            }
            else
            {
                Destroy(allLanes[lane][closestNoteToBottom[lane]].note.gameObject);
                //allLanes[lane][closestNoteToBottom[lane]].note.disableNote();
                allLanes[lane][closestNoteToBottom[lane]].note = null;
                closestNoteToBottom[lane]++;
            }
        }
    }

    public void resolveRelease(int lane)
    {
        if (closestNoteToBottom[lane] < allLanes[lane].Count) // if there are notes in the lane
        {
            // if it's not a long note, don't do anything
            if (!allLanes[lane][closestNoteToBottom[lane]].hold)
            {
                return;
            }

            // if somehow the player has started holding down before they were supposed to start and the note doesn't register as hit, don't do anything
            if (!allLanes[lane][closestNoteToBottom[lane]].hit)
            {
                return;
            }

            double initialTimeDifference = ((double)allLanes[lane][closestNoteToBottom[lane]].actualTime + startTime) - AudioSettings.dspTime;

            // if the note hasn't even started yet, don't do anything
            if (initialTimeDifference > 0)
            {
                return;
            }

            // release will for sure be after note started
            double timeDifference = Math.Abs(((double)allLanes[lane][closestNoteToBottom[lane]].topEndTime + startTime) - AudioSettings.dspTime);

            scorer.inputScore(timeDifference, lane);

            //Destroy(allLanes[lane][closestNoteToBottom[lane]].note.gameObject);
            allLanes[lane][closestNoteToBottom[lane]].note.disableNote();
            allLanes[lane][closestNoteToBottom[lane]].note = null;
            closestNoteToBottom[lane]++;
        }
    }

    public GameObject spawnAtLocation(GameObject noteToSpawn, Vector2 loc)
    {
        return Instantiate(noteToSpawn, loc, Quaternion.identity);
    }
    private float toRadians(float value)
    {
        return value * (Mathf.PI / 180);
    }

}