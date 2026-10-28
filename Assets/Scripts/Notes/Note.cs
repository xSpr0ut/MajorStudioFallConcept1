using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Note : MonoBehaviour
{
    protected double fallSpd;

    protected double initialTime;
    protected double startTime;
    protected double endTime;
    protected double totalTravelTime;

    protected Vector3 originalLocation;
    protected Vector3 endLocation;

    protected Vector3 totalDistance;

    protected Color color;

    private Color disableColor;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void setVars(double startTime, double endTime, double fallSpd, Vector3 endLocation, Color color)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.fallSpd = fallSpd;
        this.endLocation = endLocation;
        this.color = color;

        disableColor = new Color(color.r, color.g, color.b, 0.5f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTime = AudioSettings.dspTime;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    protected abstract void move();

    public void disableNote()
    {
        spriteRenderer.color = disableColor;
    }

}
