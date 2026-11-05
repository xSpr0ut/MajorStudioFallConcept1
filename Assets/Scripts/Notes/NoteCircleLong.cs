/* NoteCircleLong.cs
 * Osu style held note that inherits from NoteCircle
 * Jessica this is the file you should change
 * Does not do what it's supposed to do at all. Currently, it behaves like a traditional rhythmplus note
 */

using UnityEngine;

public class NoteCircleLong : NoteCircle
{
    // used for calculating percentage
    private float numTotalDistance;
    private void Start()
    {
        initialTime = AudioSettings.dspTime;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;
        numTotalDistance = Vector3.Distance(originalLocation, endLocation);
    }

    // Hi Jessica feel free to delete this method and replace it with your own
    // Reference NoteCircle.move() for expanding note movement
    // Current method stretches note out so it appears to be falling to correct location, but doesn't work
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        // closer to player
        float percentageTravelledBottom = Mathf.Clamp01((float)((currentTime - startTime) / totalTravelTime));
        Vector3 bottomCoord = new Vector3(originalLocation.x,
                                          originalLocation.y - (totalDistance.y * percentageTravelledBottom),
                                          originalLocation.z);

        // further from player 
        float percentageTravelledTop = Mathf.Clamp01((float)((currentTime - topStartTime) / totalTravelTime));
        Vector3 topCoord = new Vector3(originalLocation.x,
                                       originalLocation.y - (totalDistance.y * percentageTravelledTop),
                                       originalLocation.z);

        Vector3 midPoint = (topCoord + bottomCoord) / 2;
        float scale = Mathf.Clamp(Vector3.Distance(topCoord, bottomCoord), 0, Vector3.Distance(originalLocation, endLocation));

        Debug.Log(percentageTravelledBottom + "  " + percentageTravelledTop);

        transform.position = midPoint;

        GetComponent<SpriteRenderer>().size = new Vector2(1, scale);
        //transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }
}
