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
        double currentTime = AudioSettings.dspTime;

        // closer to player
        float percentageTravelledBottom = Mathf.Clamp01((float)((currentTime - startTime) / totalTravelTime));
        Vector3 bottomCoord = new Vector3(originalLocation.x - (totalDistance.x * percentageTravelledBottom),
                                          originalLocation.y,
                                          originalLocation.z - (totalDistance.z * percentageTravelledBottom));

        // further from player 
        float percentageTravelledTop = Mathf.Clamp01((float)((currentTime - topStartTime) / totalTravelTime));
        Vector3 topCoord = new Vector3(originalLocation.x - (totalDistance.x * percentageTravelledTop),
                                       originalLocation.y,
                                       originalLocation.z - (totalDistance.z * percentageTravelledTop));

        Vector3 midPoint = (topCoord + bottomCoord) / 2;
        float scale = Mathf.Clamp(Vector3.Distance(topCoord, bottomCoord), 0, Vector3.Distance(originalLocation, endLocation));
        if (originalLocation.x < -14)
        {
            Debug.Log(currentTime + "    " + topStartTime + "     " + startTime);
            Debug.Log(currentTime + "    " + bottomCoord.x + "     " + topCoord.x + "     " + midPoint.x);
            Debug.Log(currentTime + "    " + percentageTravelledBottom + "     " + percentageTravelledTop + "     " + scale);
        }

        transform.position = midPoint;
        transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);

        //float percentageTravelledBottom = (float)((currentTime - startTime) / totalTravelTime);
        //float bottomCoord = Mathf.Clamp(originalHeight - (totalHeight * percentageTravelledBottom), endHeight, originalHeight);

        //float percentageTravelledTop = (float)((currentTime - topStartTime) / totalTravelTime);
        //float topCoord = Mathf.Clamp(originalHeight - (totalHeight * percentageTravelledTop), endHeight, originalHeight);

        //float midPoint = (topCoord + bottomCoord) / 2;
        //float scale = Mathf.Clamp(topCoord - bottomCoord, 0, totalHeight);

        //transform.position = new Vector3(transform.position.x, midPoint, transform.position.z);
        //transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }

    public void onHold()
    {
    }
}
