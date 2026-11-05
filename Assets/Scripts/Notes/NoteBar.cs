/* NoteBar.cs
 * RhythmPlus style note that inherits from Note
 * Falls from predefined location to final location
 */
using UnityEngine;

public class NoteBar : Note
{
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        // manual lerp because im stupid. 
        float percentageTravelled = (float)((currentTime - startTime) / totalTravelTime);

        // change position based on % traveled, aka time passed/total time from top to bottom
        // in this case, the note falls from top of the screen to bottom of the screen
        transform.position = new Vector3(originalLocation.x - (totalDistance.x * percentageTravelled),
                                         originalLocation.y - (totalDistance.y * percentageTravelled),
                                         originalLocation.z - (totalDistance.z * percentageTravelled));
    }
}
