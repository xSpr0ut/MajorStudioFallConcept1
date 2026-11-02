using UnityEngine;

public class NoteBar : Note
{
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        float percentageTravelled = (float)((currentTime - startTime) / totalTravelTime);

        transform.position = new Vector3(originalLocation.x - (totalDistance.x * percentageTravelled),
                                         originalLocation.y - (totalDistance.y * percentageTravelled),
                                         originalLocation.z - (totalDistance.z * percentageTravelled));
    }
}
