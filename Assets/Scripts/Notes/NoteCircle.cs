using UnityEngine;

public class NoteCircle : Note
{
    [SerializeField] protected Sprite circleSprite;
    [SerializeField] protected Sprite approachSprite;
    [SerializeField] protected float circleSize = 1f;
    [SerializeField] protected float approachSize = 3f;

    protected Transform approachRing;
    protected SpriteRenderer approachRenderer;
    protected float rootScale = 1f;

    private static Sprite solidCircle;
    private static Sprite ringCircle;
    private static Sprite square;

    protected virtual void Start()
    {
        initialTime = AudioSettings.dspTime;
        originalLocation = transform.position;
        totalTravelTime = endTime - startTime;
        totalDistance = originalLocation - endLocation;

        transform.position = endLocation;
        transform.rotation = Quaternion.identity;

        spriteRenderer.sprite = circleSprite != null ? circleSprite : getSolidCircle();
        rootScale = circleSize / spriteRenderer.sprite.bounds.size.x;
        transform.localScale = new Vector3(rootScale, rootScale, 1);

        approachRing = makeChild("Approach", approachSprite != null ? approachSprite : getRingCircle(), spriteRenderer.sortingOrder + 2, out approachRenderer);

        move();
    }

    override
    protected void move()
    {
        double currentTime = AudioSettings.dspTime;

        float percentageTravelled = Mathf.Clamp01((float)((currentTime - startTime) / totalTravelTime));

        float size = Mathf.Lerp(approachSize, 1f, percentageTravelled) * circleSize;
        setWorldSize(approachRing, approachRenderer.sprite, size, size);

        approachRenderer.color = new Color(1, 1, 1, Mathf.Clamp01(percentageTravelled * 3));
        approachRenderer.enabled = currentTime < endTime;
    }

    protected Transform makeChild(string childName, Sprite sprite, int order, out SpriteRenderer renderer)
    {
        GameObject child = new GameObject(childName);
        child.transform.SetParent(transform, false);

        renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingLayerID = spriteRenderer.sortingLayerID;
        renderer.sortingOrder = order;

        return child.transform;
    }

    protected void setWorldSize(Transform t, Sprite sprite, float width, float height)
    {
        Vector3 spriteSize = sprite.bounds.size;
        t.localScale = new Vector3(width / spriteSize.x / rootScale, height / spriteSize.y / rootScale, 1);
    }

    protected static Sprite getSolidCircle()
    {
        if (solidCircle == null) solidCircle = makeCircle(128, 0f);
        return solidCircle;
    }

    protected static Sprite getRingCircle()
    {
        if (ringCircle == null) ringCircle = makeCircle(128, 0.1f);
        return ringCircle;
    }

    protected static Sprite getSquare()
    {
        if (square == null)
        {
            Texture2D tex = Texture2D.whiteTexture;
            square = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
        }
        return square;
    }

    private static Sprite makeCircle(int size, float thickness)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        float radius = size / 2f;
        float innerRadius = radius * (1 - thickness);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(radius, radius));

                float alpha = Mathf.Clamp01(radius - dist);
                if (thickness > 0)
                {
                    alpha *= Mathf.Clamp01(dist - innerRadius);
                }

                pixels[y * size + x] = new Color(1, 1, 1, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}