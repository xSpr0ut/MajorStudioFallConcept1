/* NoteBarLong.cs
 * RhythmPlus style long note that inherits from NoteBar (doesn't need to to be honest)
 * Falls from predefined location to final location, stretching along the way based on difference between hold and release time
 */
using UnityEngine;

public class NoteBarLong : NoteBar
{
    // used for calculating percentage
    private float numTotalDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
        initialTime = AudioSettings.dspTime;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;
        numTotalDistance = Vector3.Distance(originalLocation, endLocation);
    }

    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        // gets the coordinate of the Vector3 that is closer to the bottom
        float percentageTravelledBottom = Mathf.Clamp01((float)((currentTime - startTime) / totalTravelTime));
        Vector3 bottomCoord = new Vector3(originalLocation.x,
                                          originalLocation.y - (totalDistance.y * percentageTravelledBottom),
                                          originalLocation.z);

        // gets the coordinate of the Vector3 that is closer to the top
        float percentageTravelledTop = Mathf.Clamp01((float)((currentTime - topStartTime) / totalTravelTime));
        Vector3 topCoord = new Vector3(originalLocation.x,
                                       originalLocation.y - (totalDistance.y * percentageTravelledTop),
                                       originalLocation.z);

        // get where the middle of the note should be
        Vector3 midPoint = (topCoord + bottomCoord) / 2;

        // stretch out the note so that it appears long; this is the difference between the top and bottom
        float scale = Mathf.Clamp(Vector3.Distance(topCoord, bottomCoord), 0, Vector3.Distance(originalLocation, endLocation));

        Debug.Log(percentageTravelledBottom + "  " + percentageTravelledTop);

        // apply position and scaling changes
        transform.position = midPoint;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);

        /* May be confusing so I will explain it again
         * Think of a long note as two regular notes; one when you start holding, and one when you release.
         * These notes fall at the same time, so while theyre both falling, 
         * we can get in the middle of the two and scale the long note so that the ends touch the short notes.
         * 
         * However, the two short notes will not always be the same distance from each other. 
         * Sometimes, the later note will not have spawned yet while the earlier note is already spawning.
         * Or, the earlier note already moved past the "hit" line.
         * 
         * Either way, that's what the final Clamp() is for.
         * To prevent the note from not knowing what to do when either side is missing, 
         * we clamp the values to 0 (very close to bottom) or the total length of where the notes fall (when release is MUCH later than hold)
         */
    }
}
