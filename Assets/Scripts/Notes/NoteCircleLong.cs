using UnityEngine;

public class NoteCircleLong : NoteCircle
{
    [SerializeField] private float maxSliderLength = 4f;
    [SerializeField] private float trackWidth = 0.9f;
    [SerializeField] private float ballSize = 1.3f;
    [SerializeField] private float trackAlpha = 0.5f;

    private double releaseTime;

    private Vector3 sliderStart;
    private Vector3 sliderEnd;

    private Transform track;
    private Transform endCircle;
    private Transform ball;
    private SpriteRenderer trackRenderer;
    private SpriteRenderer endRenderer;
    private SpriteRenderer ballRenderer;

    protected override void Start()
    {
        spriteRenderer.sortingOrder += 2;

        base.Start();

        releaseTime = topStartTime + totalTravelTime;

        Vector3 dir = originalLocation - endLocation;
        dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.down;

        float length = Mathf.Min((float)((releaseTime - endTime) * fallSpd), maxSliderLength);

        sliderStart = endLocation;
        sliderEnd = endLocation + dir * length;

        int order = spriteRenderer.sortingOrder;

        track = makeChild("Track", getSquare(), order - 2, out trackRenderer);
        track.position = (sliderStart + sliderEnd) / 2;
        track.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        setWorldSize(track, trackRenderer.sprite, circleSize * trackWidth, length);

        endCircle = makeChild("End", getSolidCircle(), order - 1, out endRenderer);
        endCircle.position = sliderEnd;
        setWorldSize(endCircle, endRenderer.sprite, circleSize * trackWidth, circleSize * trackWidth);

        ball = makeChild("Ball", getRingCircle(), order + 1, out ballRenderer);
        setWorldSize(ball, ballRenderer.sprite, circleSize * ballSize, circleSize * ballSize);

        move();
    }

    override
    protected void move()
    {
        base.move();

        if (track == null) return;

        double currentTime = AudioSettings.dspTime;

        double holdTime = System.Math.Max(releaseTime - endTime, 0.0001);
        float percentageHeld = Mathf.Clamp01((float)((currentTime - endTime) / holdTime));

        Vector3 farEnd = Vector3.Lerp(sliderEnd, sliderStart, percentageHeld);
        float length = Vector3.Distance(farEnd, sliderStart);

        track.position = (farEnd + sliderStart) / 2;
        setWorldSize(track, trackRenderer.sprite, circleSize * trackWidth, length);

        endCircle.position = farEnd;
        ball.position = farEnd;
        ballRenderer.enabled = currentTime >= endTime;

        Color c = spriteRenderer.color;
        trackRenderer.color = new Color(c.r, c.g, c.b, c.a * trackAlpha);
        endRenderer.color = c;
        ballRenderer.color = c;

        bool done = currentTime > releaseTime;
        spriteRenderer.enabled = !done;
        trackRenderer.enabled = !done;
        endRenderer.enabled = !done;
        ballRenderer.enabled = ballRenderer.enabled && !done;
    }
}