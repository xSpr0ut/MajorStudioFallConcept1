using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct DanceRoutine
{
    public int danceID;
    public List<Sprite> danceFrames;
}

public class DanceAnimController : MonoBehaviour
{
    public static DanceAnimController Instance { get; private set; }

    [SerializeField] public GameObject player;
    [SerializeField] private List<DanceRoutine> danceRoutine;
    [SerializeField] private Sprite danceFailFrame;

    private SpriteRenderer spriteRend;

    // the sprite that should be displayed ONLY if plyaer doesnt fail
    private Sprite currentDanceFrame;

    private Coroutine danceCoroutine;
    private Coroutine failCoroutine;

    private float bpm = 131f;
    private bool isFailing = false;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Start()
    {
        spriteRend = player.GetComponent<SpriteRenderer>();
        currentDanceFrame = spriteRend.sprite;

        player.SetActive(false);
    }

    public void StartDancing()
    {
        player.SetActive(true);

        if (danceCoroutine != null)
        {
            StopCoroutine(danceCoroutine);
        }

        if (failCoroutine != null)
        {
            StopCoroutine(failCoroutine);
            failCoroutine = null;
        }

        isFailing = false;

        danceCoroutine = StartCoroutine(Dance());
    }

    public void StopDancing()
    {
        if (danceCoroutine != null)
        {
            StopCoroutine(danceCoroutine);
            danceCoroutine = null;
        }

        if (failCoroutine != null)
        {
            StopCoroutine(failCoroutine);
            failCoroutine = null;
        }

        isFailing = false;

        player.SetActive(false);
    }

    public void TriggerFailFrame()
    {
        if (danceCoroutine == null)
            return;

        if (failCoroutine != null)
        {
            StopCoroutine(failCoroutine);
        }

        failCoroutine = StartCoroutine(ShowFailFrame());
    }

    private IEnumerator ShowFailFrame()
    {
        isFailing = true;
        spriteRend.sprite = danceFailFrame;
        yield return new WaitForSeconds(0.5f);

        isFailing = false;
        spriteRend.sprite = currentDanceFrame;
        failCoroutine = null;
    }

    private void SetDanceFrame(Sprite newFrame)
    {
        currentDanceFrame = newFrame;

        if (!isFailing)
        {
            spriteRend.sprite = currentDanceFrame;
        }
    }
    public void SetBPM(string songName)
    {
        if (songName == "No Broke Boys" || songName == "Telephone")
        {
            bpm = 122;
        }
        else if (songName == "Hot and Cold")
        {
            bpm = 132;
        }
    }

    public IEnumerator Dance()
    {
        float secondsPer4Frames = (60f / bpm) * 4f;
        float outerFrameHold = secondsPer4Frames * 0.18f;
        float innerFrameHold = secondsPer4Frames * 0.07f;

        while (true)
        {
            for (int i = 0; i < danceRoutine.Count; i++)
            {

                for (int j = 0; j < 4; j++)
                {
                    SetDanceFrame(danceRoutine[i].danceFrames[0]);
                    yield return new WaitForSeconds(outerFrameHold);

                    SetDanceFrame(danceRoutine[i].danceFrames[1]);
                    yield return new WaitForSeconds(innerFrameHold);

                    SetDanceFrame(danceRoutine[i].danceFrames[2]);
                    yield return new WaitForSeconds(outerFrameHold);

                    SetDanceFrame(danceRoutine[i].danceFrames[1]);
                    yield return new WaitForSeconds(innerFrameHold);
                }
            }

            if (danceRoutine.Count == 0)
                yield return null;
        }
    }
}
