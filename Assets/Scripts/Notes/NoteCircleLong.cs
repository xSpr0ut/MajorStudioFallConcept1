/* NoteCircleLong.cs
 * Osu style held note that inherits from NoteCircle
 * Jessica this is the file you should change
 * Does not do what it's supposed to do at all. Currently, it behaves like a traditional rhythmplus note
 */

using UnityEngine;

public class NoteCircleLong : NoteCircle
{

    [SerializeField] private GameObject outerRing;
    [SerializeField] private GameObject innerRing;

    private Color holdColor;

    // used for calculating percentage
    private float numTotalDistance;
    private void Start()
    {
        initialTime = AudioSettings.dspTime;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;
        numTotalDistance = Vector3.Distance(originalLocation, endLocation);
        outerRing.GetComponent<SpriteRenderer>().color = color;

        //lol. i hate unity color space 
        holdColor = new Color((color.r * 255 + 30) / 255, (color.g * 255 + 30) / 255, (color.b * 255 + 30) / 255);
    }

    // Hi Jessica feel free to delete this method and replace it with your own
    // Reference NoteCircle.move() for expanding note movement
    // Current method stretches note out so it appears to be falling to correct location, but doesn't work
    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        // manual lerp because I'm stupid. 
        float percentageTravelledOuter = (float)((currentTime - startTime) / totalTravelTime);
        float percentageTravelledInner = (float)((currentTime - topStartTime) / totalTravelTime);

        // because of the way notes still exist after the hit time is over to give leniency for player, the mask interacts weird with other long notes
        // for this reason im doing a manual resetting back to 0 when note is over to ensure the mask doesnt overlap weird
        if (percentageTravelledInner > 1.0f)
        {
            percentageTravelledOuter = 0;
            percentageTravelledInner = 0;
            GetComponent<SpriteRenderer>().enabled = false;
        }


        // scale the scale by % traveled and multiply by 5 because the outer ring is scaled by 5
        // they are initially the same size, so whatever scale the other ring is just change this number
        float scaleOuter = Mathf.Clamp01(percentageTravelledOuter);
        float scaleInner = Mathf.Clamp01(percentageTravelledInner);

        

        // set the scale
        outerRing.transform.localScale = new Vector3(scaleOuter, scaleOuter, 1);
        innerRing.transform.localScale = new Vector3(scaleInner, scaleInner, 1);
    }

    public override void onHold()
    {
        outerRing.GetComponent<SpriteRenderer>().color = holdColor;
    }
    public override void disableNote()
    {
        base.disableNote();
        outerRing.GetComponent<SpriteRenderer>().color = disableColor;
    }
}
