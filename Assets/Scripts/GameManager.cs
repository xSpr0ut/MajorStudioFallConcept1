using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assemblies;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameManager : MonoBehaviour
{
    #region vars and such
    
    private SongSelector.Song selectedSong;

    [Header("Note info")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private GameObject longNotePrefab;

    [SerializeField] private double noteSpeed = 5; // in units/second, player controlled
    [SerializeField] private double approachTime = 0.8; // seconds a circle is visible before it should be hit (osu "approach rate")

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
        chartParser.parseChart(selectedSong, approachTime);

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

        // no key lines anymore: notes appear on the D/F/J/K speaker circles (NoteSpawner.level1locs)

        scorer.gameObject.SetActive(true);

        //lol
        string filePath = "Audio/" + selectedSong.musicFilePath + audioCues[currentAudioCue].name.Replace(".wav", "").Replace(".mp3", "");
        AudioClip a = Resources.Load<AudioClip>(filePath);

        started = true;
        startTime = AudioSettings.dspTime + startDelay;

        gameEndTime = Math.Max(a.length, visualCues[visualCues.Count - 1].evaluationTime + (scorer.getMissRange())/1000) + startTime + 1f;
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
            OsuNote n = noteSpawner.spawnVisualCue(visualCues[currentVisualCue], startTime);

            // lot of shuffling but you assign the note object to the Note value in one of the VisualCues in the current lane
            int currentLane = visualCues[currentVisualCue].lane;

            allLanes[currentLane][currentNoteSpawning[currentLane]].note = n;

            currentVisualCue++;
            currentNoteSpawning[currentLane]++;
        }
    }

    private void checkMissedNotes(double currentTime)
    {
        double missWindow = (double)scorer.getMissRange() / 1000.0;

        for (int i = 0; i < allLanes.Length; i++)
        {
            while (closestNoteToBottom[i] < allLanes[i].Count)
            {
                VisualCue cue = allLanes[i][closestNoteToBottom[i]];

                // circle (or slider head) was never pressed and its window has passed -> miss
                // (judged on the head, so a missed slider no longer blocks the lane until its tail)
                if (!cue.hit && cue.actualTime + startTime + missWindow <= currentTime)
                {
                    scorer.inputScore(scorer.getMiss(), i);
                    if (cue.note != null) cue.note.onMiss();
                    cue.note = null;
                    closestNoteToBottom[i]++;
                    continue;
                }

                // slider held all the way to the end -> full hit, like osu
                if (cue.hold && cue.hit && cue.topEndTime + startTime <= currentTime)
                {
                    scorer.inputScore(0.0, i);
                    if (cue.note != null) cue.note.onComplete();
                    cue.note = null;
                    closestNoteToBottom[i]++;
                    continue;
                }

                break;
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
        if (closestNoteToBottom[lane] >= allLanes[lane].Count) return; // no notes left in the lane

        VisualCue cue = allLanes[lane][closestNoteToBottom[lane]];

        // already holding this slider, a second press shouldn't score it again
        if (cue.hit) return;

        // time difference = time labelled on note - current time
        double timeDifference = Math.Abs((cue.actualTime + startTime) - AudioSettings.dspTime);

        // if the note is nowhere near the player, don't accept input
        if (timeDifference > (double)scorer.getMissRange() / 1000)
        {
            return;
        }

        scorer.inputScore(timeDifference, lane);
        cue.hit = true;

        if (cue.hold)
        {
            // slider: head is hit, ball starts rolling, player keeps holding
            if (cue.note != null) cue.note.onHit();
        }
        else
        {
            if (cue.note != null) cue.note.onHit();
            cue.note = null;
            closestNoteToBottom[lane]++;
        }
    }

    public void resolveRelease(int lane)
    {
        if (closestNoteToBottom[lane] >= allLanes[lane].Count) return;

        VisualCue cue = allLanes[lane][closestNoteToBottom[lane]];

        if (!cue.hold || !cue.hit) return;

        double timeDifference = Math.Abs((cue.topEndTime + startTime) - AudioSettings.dspTime);

        if (timeDifference > (double)scorer.getMissRange() / 1000)
        {
            // let go way too early -> slider break
            scorer.inputScore(scorer.getMiss(), lane);
            if (cue.note != null) cue.note.onMiss();
        }
        else
        {
            scorer.inputScore(timeDifference, lane);
            if (cue.note != null) cue.note.onComplete();
        }

        cue.note = null;
        closestNoteToBottom[lane]++;
    }
    
    public Vector3 getLanePosition(int lane)
    {
        return noteSpawner.getLanePosition(lane);
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