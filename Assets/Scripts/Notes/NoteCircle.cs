using UnityEngine;

public class NoteCircle : Note
{
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        float percentageTravelled = (float)((currentTime - startTime) / totalTravelTime);

        float scale = Mathf.Clamp01(percentageTravelled) * 5;

        transform.localScale = new Vector3(scale, scale, 1);
    }
}
