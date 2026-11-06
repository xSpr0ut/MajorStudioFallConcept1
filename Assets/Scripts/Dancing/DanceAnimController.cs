using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct DanceRoutine {
    public int danceID;
    public List<Sprite> danceFrames;
}
public class DanceAnimController : MonoBehaviour
{
    public static DanceAnimController Instance { get; private set; }

    [SerializeField] public GameObject player;
    [SerializeField] List<DanceRoutine> danceRoutine;
    [SerializeField] public Sprite danceFailFrame;

    private SpriteRenderer spriteRend;
    private Sprite currentFrame;
    private Coroutine danceCoroutine;
    private float bpm;

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
        currentFrame = spriteRend.sprite;
        player.SetActive(false);
    }

    public void StartDancing()
    {
        player.SetActive(true);

        //bpm = get song bpm
        bpm = 131f;

        if (danceCoroutine != null) { 
            StopCoroutine(danceCoroutine);  
        }

        danceCoroutine = StartCoroutine(Dance());
    }

    public void StopDancing()
    {
        if (danceCoroutine != null) { 
            //StopCoroutine(danceCoroutine);
            danceCoroutine = null;
        }

        player.SetActive(false);

    }

    public void TriggerFailFrame()
    {
        
    }

    public IEnumerator Dance()
    {
        float secondsPer4Frames = (60f / bpm) * 4f;
        float outerFrameHold = secondsPer4Frames * 0.15f;
        float innerFrameHold = secondsPer4Frames * 0.1f;

        Debug.Log("dance routine" + danceRoutine.Count) ;

        while (true)
        {
            for (int _i = 0; _i < danceRoutine.Count; _i++)
            {
                for (int _j = 0; _j < 4; _j++)
                {
                    Debug.Log("move");
                    spriteRend.sprite = danceRoutine[_i].danceFrames[0];
                    yield return new WaitForSeconds(outerFrameHold);

                    spriteRend.sprite = danceRoutine[_i].danceFrames[1];
                    yield return new WaitForSeconds(innerFrameHold);

                    spriteRend.sprite = danceRoutine[_i].danceFrames[2];
                    yield return new WaitForSeconds(outerFrameHold);

                    spriteRend.sprite = danceRoutine[_i].danceFrames[1];
                    yield return new WaitForSeconds(innerFrameHold);
                }
            }
        }

        yield return null;
    }

    public void Update()
    {
        
    }
}
