/* NoteCircle.cs
 * Osu style note that inherits from Note
 * It does not move, but instead grows from 0 to 500% scale the closer the timing gets to when player should hit
 * There is one magic number in here, we will have to change it manually
 */

using UnityEngine;

public class NoteCircle : Note
{
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        // manual lerp because I'm stupid. 
        float percentageTravelled = (float)((currentTime - startTime) / totalTravelTime);

        // scale the scale by % traveled and multiply by 5 because the outer ring is scaled by 5
        // they are initially the same size, so whatever scale the other ring is just change this number
        float scale = Mathf.Clamp01(percentageTravelled) * 5;

        // set the scale
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
