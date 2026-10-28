using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class ChartParser
{
    private string title;
    private string artist;
    private double bpm;
    private string playLevel;

    // mappings
    Dictionary<string, string> wavs = new Dictionary<string, string>(); // for converting pairs to file name (parsing)
    Dictionary<string, double> bpms = new Dictionary<string, double>(); // for converting pairs to bpms

    private double prescheduleTime = 1;
    public double chartTimeOffset = 0; // global shift (s) added to ALL note times (BMS and CSV) to align the chart to the music

    private List<AudioCue> audioCues = new List<AudioCue>();
    private List<VisualCue> visualCues = new List<VisualCue>();

    private List<VisualCue>[] allLanes = new List<VisualCue>[8];

    public ChartParser()
    {
        // instantiate lanes
        for (int i = 0; i < 8; i++)
        {
            allLanes[i] = new List<VisualCue>();
        }
    }
    public void parseChart(SongSelector.Song song, double noteSpeed, float initialHeight)
    {
        parseCSV(song);
        addDspTimeCSV(noteSpeed, initialHeight);
    }

    void parseCSV(SongSelector.Song song)
    {
        string assetPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("Charts/" + song.chartFilePath));

        foreach (string line in File.ReadLines(assetPath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split line into columns based on commas
            string[] columns = line.Split(',');

            double timeToPress = 0;
            if (double.TryParse(columns[1].Trim(), out double result))
            {
                timeToPress = result;

                for (int i = 2; i < columns.Length; i++)
                {
                    if (int.TryParse(columns[i].Trim(), out int value))
                    {
                        int pressed = value;
                        if (pressed != 0)
                        {
                            VisualCue visualCue = new VisualCue();
                            visualCue.actualTime = timeToPress + chartTimeOffset;
                            visualCue.evaluationTime = timeToPress + chartTimeOffset;
                            visualCue.lane = i - 2;

                            if (pressed == 2)
                            {
                                visualCue.hold = true;
                                visualCue.topEndTime = visualCue.actualTime + 0.5;
                                visualCue.evaluationTime = visualCue.topEndTime;
                            }

                            visualCues.Add(visualCue);
                        }
                    }
                }
            }
        }

        string key = ".wav";

        wavs[key] = song.musicFilePath;

        AudioCue cue = new AudioCue();
        cue.actualTime = 0;
        cue.scheduleTime = 0 - prescheduleTime;
        cue.name = key;

        audioCues.Add(cue);
    }

    void addDspTimeCSV(double noteSpeed, float initialHeight)
    {
        for (int i = 0; i < visualCues.Count; i++)
        {
            visualCues[i].spawnTime = visualCues[i].actualTime - (initialHeight / noteSpeed); // this MIGHT be a negative value but don't worry
            if (visualCues[i].hold)
            {
                visualCues[i].topSpawnTime = visualCues[i].topEndTime - (initialHeight / noteSpeed);
            }
            allLanes[visualCues[i].lane].Add(visualCues[i]); // add to lane for easy player input
        }

        //Debug.Log
    }

    public List<AudioCue> getAudioCues() { return audioCues; }

    public List<VisualCue> getVisualCues() { return visualCues; }

    public List<VisualCue>[] getAllLanes() { return allLanes; }
}
