using UnityEngine;

public class Note : MonoBehaviour
{
    public double fallSpd;

    protected double initialTime;
    public double startTime;
    public double endTime;
    protected double totalTravelTime;

    protected Vector3 originalLocation;
    public Vector3 endLocation;

    protected Vector3 totalDistance;

    //protected float originalHeight;
    //public float endHeight;
    //protected float totalHeight;

    [SerializeField] Material disableMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTime = AudioSettings.dspTime;
        //originalHeight = transform.position.y;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        //totalHeight = originalHeight - endHeight;
        totalDistance = originalLocation - endLocation;
    }

    // Update is called once per frame
    void Update()
    {
        double currentTime = AudioSettings.dspTime;

        float percentageTravelled = (float)((currentTime - startTime) / totalTravelTime);

        //transform.position = new Vector3(transform.position.x,
        //                                 originalHeight - (totalHeight * percentageTravelled),
        //                                 transform.position.z);

        transform.position = new Vector3(originalLocation.x - (totalDistance.x * percentageTravelled),
                                         originalLocation.y - (totalDistance.y * percentageTravelled),
                                         originalLocation.z - (totalDistance.z * percentageTravelled));
    }

    public void disableNote()
    {
        GetComponent<Renderer>().material = disableMat;
    }

}
