using UnityEngine;

public class NoteBarLong : NoteBar
{
    public double topStartTime;

    // used for calculating percentage
    private float numTotalDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created 
    void Start()
    {
        initialTime = AudioSettings.dspTime;
        //originalHeight = transform.position.y;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        //totalHeight = originalHeight - endHeight;
        totalDistance = originalLocation - endLocation;
        numTotalDistance = Vector3.Distance(originalLocation, endLocation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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

        transform.position = midPoint;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }

    public void onHold()
    {

    }
}
