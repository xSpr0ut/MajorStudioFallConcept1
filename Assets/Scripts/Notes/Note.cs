/* Note.cs
 * Abstract class that all notes inherit from.
 * Aka, every note shares the same values (ex: start location, end location, when user should press),
 * but their movement is different across notes (ex: static, moving down screen, expanding).
 * Our current goal is to create more Note objects that inherit from this base class
 * 
 * !!!!!!!!!!!!!!! DO NOT TOUCH THIS CLASS !!!!!!!!!!!!!!!
 */

using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Note : MonoBehaviour
{
    // defined in gameManager in the inspector
    protected double fallSpd;

    // time that note spawns, assigned in start()
    protected double initialTime;

    // set by noteSpawner, relevant for all notes
    protected double startTime;
    protected double endTime;
    protected Vector3 endLocation;
    protected Color color;

    // set by noteSpawner for long notes only
    protected double topStartTime;

    // calculated in start(), effectively
    protected Vector3 originalLocation;
    protected double totalTravelTime;
    protected Vector3 totalDistance;
    private Color disableColor;

    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // called in noteSpawner immediately after Instantiate()
    public void setVars(double startTime, double endTime, double fallSpd, Vector3 endLocation, Color color)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.fallSpd = fallSpd;
        this.endLocation = endLocation;
        this.color = color;

        disableColor = new Color(color.r, color.g, color.b, 0.5f);
    }

    // called in noteSpawner immediately after Instantiate()
    public void setLongNoteVars(double topStartTime)
    {
        this.topStartTime = topStartTime;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AudioSettings.dspTime is extremely accurate for timing related events
        initialTime = AudioSettings.dspTime;

        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;
    }

    // DO NOT ADD AN UPDATE FUNCTION INTO ANY INHERITED OBJECTS
    // EVERY SINGLE NOTE SHOULD ONLY CALL move() EVERY FRAME
    void Update()
    {
        move();
    }

    // All child objects override this method
    // This is the note's behavior (ex: expanding, dropping from sky, etc)
    // ONLY method that you will have to change across Note objects
    protected abstract void move();

    // only for long notes that player is currently holding
    public void onHold()
    {
        spriteRenderer.color = Color.greenYellow;
    }

    // only for long notes when player hits the initial note but releases too early
    public void disableNote()
    {
        spriteRenderer.color = disableColor;
    }

    
}
