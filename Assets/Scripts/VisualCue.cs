using System;
using UnityEngine;

public class VisualCue
{
    public double beatPos;
    public int lane;
    public AudioClip clip;
    public double spawnTime;
    public double actualTime;

    public bool hold;
    public double topSpawnTime;
    public double topEndTime;
    public bool hit;

    public double evaluationTime;

    public Note note;

    public enum NoteTypes
    {
        BAR,
        LONGBAR,
        CIRCLE,
        LONGCIRCLE
    }


    public NoteTypes type;
}
