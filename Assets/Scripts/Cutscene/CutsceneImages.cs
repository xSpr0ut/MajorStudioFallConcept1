using UnityEngine;

public class CutsceneImages : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private GameObject typewriterObject;
    private AudioSource audioSource;

    [Header("Objects")]
    [SerializeField] private GameObject[] objects;
    [SerializeField] private float[] startTimes;
    [SerializeField] private float[] endTimes;

    [Header("Fade")]
    [SerializeField] private float fadeTime = 0.5f;

    private CanvasGroup[] canvasGroups;
    private SpriteRenderer[] spriteRenderers;
    private float[] alphas;

    void Start()
    {
        canvasGroups = new CanvasGroup[objects.Length];
        spriteRenderers = new SpriteRenderer[objects.Length];
        alphas = new float[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            spriteRenderers[i] = objects[i].GetComponent<SpriteRenderer>();

            // if its not a sprite, its UI, so add a canvas group so we can fade it
            if (spriteRenderers[i] == null)
            {
                canvasGroups[i] = objects[i].GetComponent<CanvasGroup>();
                if (canvasGroups[i] == null)
                {
                    canvasGroups[i] = objects[i].AddComponent<CanvasGroup>();
                }
            }

            // make it invisible at the start
            alphas[i] = 0;
            SetAlpha(i);
        }

        audioSource = typewriterObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (audioSource.isPlaying)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (audioSource.time >= startTimes[i] && audioSource.time < endTimes[i])
                {
                    // fade in
                    alphas[i] += Time.deltaTime / fadeTime;
                }
                else
                {
                    // fade out
                    alphas[i] -= Time.deltaTime / fadeTime;
                }

                alphas[i] = Mathf.Clamp(alphas[i], 0, 1);
                SetAlpha(i);
            }
        }
    }

    void SetAlpha(int i)
    {
        if (spriteRenderers[i] != null)
        {
            Color color = spriteRenderers[i].color;
            color.a = alphas[i];
            spriteRenderers[i].color = color;
        }
        else
        {
            canvasGroups[i].alpha = alphas[i];
        }
    }
}