using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assemblies;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// unbastardized documentation https://docs.unity3d.com/ScriptReference/AudioSettings-dspTime.html
public class GameManager : MonoBehaviour
{
    #region vars and such
    
    private SongSelector.Song selectedSong;

    [Header("Note info")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject longNotePrefab;

    [SerializeField] private double noteSpeed = 5; // in units/second, player controlled

    [SerializeField] private float initialHeight = 5f;
    [SerializeField] private float endHeight = 0f;

    [Header("Audio stuff")]
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
    
    private int[] currentNoteSpawning = new int[5]; // keeping track of which position each lane is in
    private int[] closestNoteToBottom = new int[5]; // keeping track of the lowest note in each lane

    //list of all notes
    List<VisualCue>[] allLanes = new List<VisualCue>[5];
    List<GameObject> keyLines = new List<GameObject>();

    // game variables
    private ChartParser chartParser;
    private NoteSpawner noteSpawner;
    [SerializeField] private Scoring scorer;
    private bool started = false;

    // list of keys for easy looping
    private List<KeyControl> keys = new List<KeyControl>();

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

        noteSpawner = GetComponent<NoteSpawner>();  
    }
    void Start()
    {
        keys.Add(Keyboard.current.dKey);
        keys.Add(Keyboard.current.fKey);
        keys.Add(Keyboard.current.jKey);
        keys.Add(Keyboard.current.kKey);
        keys.Add(Keyboard.current.spaceKey);

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

            spawnVisualCue(currentTime);

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
        for (int i = 0; i < 4; i++)
        {
            var k = spawnAtLocation(keyPrefab, new Vector3(-3 + 2 * i, -4, 0));

            //Image keyRect = k.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            //keyRect.sprite = arrows[i];
            ////keyRect.rotation = Quaternion.Euler(0, 90, i * 45);
            //k.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = i.ToString();
            keyLines.Add(k);
            noteSpawner.level4locs.Add(k.transform);
        }

        scorer.gameObject.SetActive(true);

        //lol
        string filePath = "Audio/" + selectedSong.musicFilePath + audioCues[currentAudioCue].name.Replace(".wav", "").Replace(".mp3", "");
        AudioClip a = Resources.Load<AudioClip>(filePath);

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
        double songTime = currentTime - startTime; 

        while (currentAudioCue < audioCues.Count && audioCues[currentAudioCue].actualTime + startTime <= currentTime)
        {
            AudioClip a = Resources.Load<AudioClip>("Audio/" + selectedSong.musicFilePath + audioCues[currentAudioCue].name.Replace(".wav", "").Replace(".mp3", ""));
            if (a != null) AudioSource.PlayClipAtPoint(a, new Vector3(0, 0, 0));
            currentAudioCue++;
        }
    }

    private void spawnVisualCue(double currentTime)
    {
        // spawning notes
        while (currentVisualCue < visualCues.Count && visualCues[currentVisualCue].spawnTime + startTime <= currentTime)
        {
            Note n = noteSpawner.spawnVisualCue(visualCues[currentVisualCue], startTime, noteSpeed);

            // lot of shuffling but you assign the note object to the Note value in one of the VisualCues in the current lane
            int currentLane = visualCues[currentVisualCue].lane;

            allLanes[currentLane][currentNoteSpawning[currentLane]].note = n;

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
        foreach (KeyControl key in keys)
        {
            if (key.wasPressedThisFrame)
            {
                resolveHit(keys.IndexOf(key));
            }

            if (key.wasReleasedThisFrame)
            {
                resolveRelease(keys.IndexOf(key));
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

            // put this back when we want sound effects with our music ig
            //if (!String.IsNullOrEmpty(allLanes[lane][closestNoteToBottom[lane]].name))
            //{
            //    AudioClip clip = defaultClip;
            //    clip = Resources.Load<AudioClip>(selectedSong.musicFilePath + allLanes[lane][closestNoteToBottom[lane]].name.Replace(".wav", ""));
            //    audioSrc.PlayOneShot(clip);
            //}

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