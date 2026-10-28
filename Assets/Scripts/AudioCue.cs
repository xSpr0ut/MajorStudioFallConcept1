using System;
using UnityEngine;

public class AudioCue : IComparable<AudioCue>
{
    public double beatPos;
    public int channel;
    public double scheduleTime;
    public double actualTime;
    public string name;

    public int CompareTo(AudioCue a)
    {
        if (beatPos > a.beatPos)
        {
            return 1;
        }
        else if (beatPos < a.beatPos)
        {
            return -1;
        }

        // if measures are the same, sort by channel
        // usually, bms files have all the bgm on the same channel. however, we are splitting it into 8
        // the actual implementation and modified bms file will come later (for now, this check does nothing)
        if (channel > a.channel)
        {
            return 1;
        }
        else if (channel < a.channel)
        {
            return -1;
        }

        return 0;
    }
}
