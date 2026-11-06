using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OsuNote : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("The main circle. If empty, the first SpriteRenderer on this object or its children is used.")]
    [SerializeField] protected SpriteRenderer body;
    [Tooltip("Ring that shrinks onto the circle. If empty, one is made at runtime from Ring Sprite.")]
    [SerializeField] protected SpriteRenderer approachRing;
    [Tooltip("Outline sprite used for the approach ring / follow ring (e.g. the NoteCircleOutline sprite). Falls back to the body sprite.")]
    [SerializeField] protected Sprite ringSprite;

    [Header("Drawing order")]
    [Tooltip("Force the note to draw above everything else (speakers, outlines)")]
    [SerializeField] protected bool drawOnTop = true;
    [SerializeField] protected int topSortingOrder = 100;

    [Header("Timing feel")]
    [SerializeField] protected float approachStartScale = 3f;   // ring size when the note appears
    [SerializeField, Range(0f, 1f)] protected float fadeInPortion = 0.4f; // part of the approach spent fading in
    [SerializeField] protected float hitFadeTime = 0.12f;
    [SerializeField] protected float missFadeTime = 0.25f;
    [SerializeField] protected float hitGrowScale = 1.3f;
    [SerializeField] protected Color missTint = new Color(0.5f, 0.5f, 0.5f, 1f);

    protected Transform target;
    private float approachStartWidth = 0f; // world width the ring starts at (0 = use approachStartScale)
    protected double spawnTime; // dsp time
    protected double hitTime;   // dsp time
    protected bool ready;
    protected bool finishing;

    protected Vector3 bodyLocalScale = Vector3.one;
    protected Vector3 bodyLocalPos = Vector3.zero;
    private Vector3 ringBaseScale;

    private readonly List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private readonly List<Color> spriteColors = new List<Color>();
    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    private readonly List<Color> lineColors = new List<Color>();
    private Color tint = Color.white;
    protected float alpha = 0f;

    // used by NoteSpawner to hand over the NoteCircleOutline image when Ring Sprite is empty
    public void setRingSpriteIfEmpty(Sprite s)
    {
        if (ringSprite == null && s != null) ringSprite = s;
    }

    // a plain white open ring, made once and shared by every note
    private static Sprite generatedRing;
    private static Sprite generatedDisc;

    protected static Sprite getGeneratedRing()
    {
        if (generatedRing == null) generatedRing = makeCircleSprite(0.08f); // thickness as a fraction of the radius
        return generatedRing;
    }

    protected static Sprite getGeneratedDisc()
    {
        if (generatedDisc == null) generatedDisc = makeCircleSprite(1f); // 1 = filled all the way in
        return generatedDisc;
    }

    private static Sprite makeCircleSprite(float thickness)
    {
        const int size = 256;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        float r = size / 2f;
        Color[] px = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r)) / r; // 0 center, 1 edge
                float outer = Mathf.Clamp01((1f - d) * size * 0.5f);              // soft outer edge
                float inner = thickness >= 1f ? 1f : Mathf.Clamp01((d - (1f - thickness)) * size * 0.5f); // soft inner edge
                px[y * size + x] = new Color(1f, 1f, 1f, Mathf.Min(outer, inner));
            }
        }
        tex.SetPixels(px);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public void setApproachStartWidth(float worldWidth)
    {
        approachStartWidth = worldWidth;
    }

    public void setup(Transform followTarget, double spawnDsp, double hitDsp)
    {
        target = followTarget;
        spawnTime = spawnDsp;
        hitTime = hitDsp;

        if (body == null) body = GetComponentInChildren<SpriteRenderer>();
        if (body == null)
        {
            Debug.LogError(name + " has no SpriteRenderer to use as the circle");
            return;
        }

        // the circle must be solid: if the prefab's image is missing or is the same ring image, draw a solid circle
        if (body.sprite == null || body.sprite == ringSprite)
        {
            body.sprite = getGeneratedDisc();
        }

        // draw above the speaker art so the solid circle isn't hidden behind it
        if (drawOnTop)
        {
            body.sortingOrder = topSortingOrder;
        }

        if (body.transform != transform)
        {
            bodyLocalScale = body.transform.localScale;
            bodyLocalPos = body.transform.localPosition;
        }

        build();

        // start the ring at the outline's size: ring ends at the closed circle's width, so the ratio is the start scale
        float circleWidth = visibleWidth(body.sprite) * Mathf.Abs(body.transform.lossyScale.x);
        if (approachStartWidth > 0f && circleWidth > 0f)
        {
            approachStartScale = approachStartWidth / circleWidth;
        }

        collectRenderers();
        setAlpha(0f);

        ready = true;
        Update();
    }

    // create the extra visual pieces; subclasses add theirs after calling base
    protected virtual void build()
    {
        // never fall back to the solid circle: use a generated open ring if nothing was given
        if (ringSprite == null || ringSprite == body.sprite)
        {
            ringSprite = getGeneratedRing();
        }

        if (approachRing == null)
        {
            approachRing = createSprite("ApproachRing", ringSprite, transform, 2);
            // size the ring so that at the end of the approach it lines up exactly with the closed circle
            approachRing.transform.localScale = bodyLocalScale * sizeRatio(approachRing.sprite, body.sprite);
        }
        ringBaseScale = approachRing.transform.localScale;
    }

    protected virtual void Update()
    {
        if (!ready) return;

        // follow the speaker spot (position only, so the circle doesn't spin with the speaker)
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }

        if (finishing) return;

        double now = AudioSettings.dspTime;
        float approach = (float)((now - spawnTime) / System.Math.Max(0.0001, hitTime - spawnTime));

        approachRing.transform.localScale = ringBaseScale * Mathf.Lerp(approachStartScale, 1f, Mathf.Clamp01(approach));
        setAlpha(fadeInPortion <= 0f ? 1f : Mathf.Clamp01(approach / fadeInPortion));

        tick(now);
    }

    protected virtual void tick(double now) { }

    public virtual void onHit()
    {
        finish(hitFadeTime, hitGrowScale, false);
    }
    public virtual void onMiss()
    {
        finish(missFadeTime, 1f, true);
    }

    public virtual void onComplete()
    {
        finish(hitFadeTime, hitGrowScale, false);
    }

    protected void finish(float time, float grow, bool missed)
    {
        if (finishing) return;
        finishing = true;

        if (approachRing != null) approachRing.enabled = false;
        if (missed) tint = missTint;

        StartCoroutine(fadeOut(time, grow));
    }

    private IEnumerator fadeOut(float time, float grow)
    {
        float startAlpha = alpha;
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / time);
            setAlpha(Mathf.Lerp(startAlpha, 0f, p));
            transform.localScale = startScale * Mathf.Lerp(1f, grow, p);
            yield return null;
        }

        Destroy(gameObject);
    }

    protected float sizeRatio(Sprite a, Sprite b)
    {
        float wa = visibleWidth(a);
        float wb = visibleWidth(b);
        if (wa <= 0f || wb <= 0f) return 1f;
        return wb / wa;
    }

    // width of the drawn part of a sprite, ignoring empty transparent margins
    // (uses the sprite's Tight mesh, which hugs the visible pixels)
    public static float visibleWidth(Sprite s)
    {
        if (s == null) return 0f;

        Vector2[] v = s.vertices;
        if (v == null || v.Length == 0) return s.bounds.size.x;

        float min = float.MaxValue, max = float.MinValue;
        foreach (Vector2 p in v)
        {
            if (p.x < min) min = p.x;
            if (p.x > max) max = p.x;
        }
        return max - min;
    }

    protected SpriteRenderer createSprite(string objName, Sprite sprite, Transform parent, int orderOffset)
    {
        GameObject go = new GameObject(objName);
        go.transform.SetParent(parent, false);
        go.transform.localScale = bodyLocalScale;
        go.transform.localPosition = parent == transform ? bodyLocalPos : Vector3.zero;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = body.color;
        sr.sortingLayerID = body.sortingLayerID;
        sr.sortingOrder = body.sortingOrder + orderOffset;
        return sr;
    }

    private void collectRenderers()
    {
        sprites.Clear();
        spriteColors.Clear();
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true))
        {
            sprites.Add(sr);
            spriteColors.Add(sr.color);
        }

        lines.Clear();
        lineColors.Clear();
        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>(true))
        {
            lines.Add(lr);
            lineColors.Add(lr.startColor);
        }
    }

    protected void setAlpha(float a)
    {
        alpha = a;

        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] == null) continue;
            Color c = spriteColors[i];
            sprites[i].color = new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, c.a * a);
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == null) continue;
            Color c = lineColors[i];
            Color col = new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, c.a * a);
            lines[i].startColor = col;
            lines[i].endColor = col;
        }
    }
}