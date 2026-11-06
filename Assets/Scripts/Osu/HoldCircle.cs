using UnityEngine;

public class HoldCircle : OsuNote
{
    [Header("Slider shape")]
    [SerializeField] private float unitsPerSecond = 3f; // path length per second of hold
    [SerializeField] private float minLength = 1f;
    [SerializeField] private float maxLength = 4f;
    [SerializeField, Range(0f, 1f)] private float pathAlpha = 0.35f;
    [SerializeField, Range(0.1f, 1.5f)] private float pathWidthScale = 0.9f; // relative to circle diameter

    [Header("Slider ball")]
    [Tooltip("Sprite for the ball. Falls back to the circle sprite.")]
    [SerializeField] private Sprite ballSprite;
    [SerializeField] private float ballScale = 0.6f;
    [SerializeField] private float followRingScale = 1.6f; // relative to the circle, shown while held

    private Vector2 direction = Vector2.up;
    private double endTime; // dsp time
    private Vector3 pathEnd; // local space
    private bool holding;

    private LineRenderer path;
    private SpriteRenderer tail;
    private SpriteRenderer ball;
    private SpriteRenderer followRing;

    // called by NoteSpawner before setup()
    public void setSlider(Vector2 dir, double endDsp)
    {
        direction = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.up;
        endTime = endDsp;
    }

    protected override void build()
    {
        base.build();

        float duration = (float)System.Math.Max(0.0, endTime - hitTime);
        float length = Mathf.Clamp(duration * unitsPerSecond, minLength, maxLength);
        pathEnd = bodyLocalPos + transform.InverseTransformVector((Vector3)(direction * length));

        float diameter = body.bounds.size.x;

        // slider body
        GameObject pathObj = new GameObject("SliderPath");
        pathObj.transform.SetParent(transform, false);
        path = pathObj.AddComponent<LineRenderer>();
        path.useWorldSpace = false;
        path.positionCount = 2;
        path.SetPosition(0, bodyLocalPos);
        path.SetPosition(1, pathEnd);
        path.widthMultiplier = diameter * pathWidthScale;
        path.numCapVertices = 12;
        path.material = new Material(Shader.Find("Sprites/Default"));
        Color pc = body.color;
        pc.a *= pathAlpha;
        path.startColor = pc;
        path.endColor = pc;
        path.sortingLayerID = body.sortingLayerID;
        path.sortingOrder = body.sortingOrder - 2;

        // tail circle
        tail = createSprite("SliderTail", body.sprite, transform, -1);
        tail.transform.localPosition = pathEnd;

        // ball + follow ring (hidden until the head time)
        ball = createSprite("SliderBall", ballSprite != null ? ballSprite : body.sprite, transform, 3);
        ball.transform.localScale = bodyLocalScale * ballScale;
        ball.enabled = false;

        followRing = createSprite("FollowRing", ringSprite != null ? ringSprite : body.sprite, ball.transform, 3);
        followRing.transform.localScale = Vector3.one * (followRingScale / ballScale) * sizeRatio(followRing.sprite, ball.sprite);
        followRing.enabled = false;
    }

    protected override void tick(double now)
    {
        if (now < hitTime) return; // full slider shows until it's time to hit

        // the far end of the slider pulls in toward the circle over the hold
        float p = Mathf.Clamp01((float)((now - hitTime) / System.Math.Max(0.0001, endTime - hitTime)));
        Vector3 currentEnd = Vector3.Lerp(pathEnd, bodyLocalPos, p);

        path.SetPosition(1, currentEnd);
        tail.transform.localPosition = currentEnd;

        ball.enabled = true;
        ball.transform.localPosition = currentEnd;
        followRing.enabled = holding;
    }

    // head was hit: don't disappear, start tracking the hold
    public override void onHit()
    {
        holding = true;
        if (approachRing != null) approachRing.enabled = false;
    }
}