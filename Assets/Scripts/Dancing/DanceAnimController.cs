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

    private Sprite currentFrame;
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
        currentFrame = player.GetComponent<Sprite>();
    }

    public void StartDancing()
    {
        //bpm = get song bpm
        bpm = 131f;

        StartCoroutine(Dance());
    }

    public void StopDancing()
    {
        StopCoroutine(Dance());
    }

    public void TriggerFailFrame()
    {
        
    }

    public IEnumerator Dance()
    {
        float secondsPer4Frames = ( 60f / bpm ) * 16f;
        float outerFrameHold = secondsPer4Frames * 0.15f;
        float innerFrameHold = secondsPer4Frames * 0.1f;

        while (true)
        {
            for (int _i = 0; _i < danceRoutine.Count; _i++)
            {
                for (int _j = 0; _j < 2; _j++)
                {
                    currentFrame = danceRoutine[_i].danceFrames[1];
                    yield return new WaitForSeconds(outerFrameHold);

                    currentFrame = danceRoutine[_i].danceFrames[2];
                    yield return new WaitForSeconds(innerFrameHold);

                    currentFrame = danceRoutine[_i].danceFrames[3];
                    yield return new WaitForSeconds(outerFrameHold);

                    currentFrame = danceRoutine[_i].danceFrames[2];
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
