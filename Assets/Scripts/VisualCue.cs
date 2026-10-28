using System;
using UnityEngine;

public class VisualCue : IComparable<VisualCue>
{
    public double beatPos;
    public int lane;
    public AudioClip clip;
    public double spawnTime;
    public double actualTime;
    public string name;

    public bool hold;
    public double topSpawnTime;
    public double topEndTime;
    public bool hit;

    public double evaluationTime;

    public Note note;

    public int CompareTo(VisualCue v)
    {
        if (beatPos > v.beatPos)
        {
            return 1;
        }
        else if (beatPos < v.beatPos)
        {
            return -1;
        }

        // if measures are the same, sort by channel
        if (lane > v.lane)
        {
            return 1;
        }
        else if (lane < v.lane)
        {
            return -1;
        }

        return 0;
    }
}
