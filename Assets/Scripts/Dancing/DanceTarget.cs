using UnityEngine;

public class DanceTarget : MonoBehaviour
{
    [SerializeField] Sprite neutral;
    [SerializeField] Sprite threatened;
    [SerializeField] Sprite seduced;
    public static DanceTarget Instance { get; private set; }

    private float bobbingSpeed;

    public float bobbingAmp;
    public float kickOfs; // beat of offset
    public float bobTimer;
    public float clampMin;
    private bool isBobbing;
    private Vector3 basePos;

    [SerializeField] public GameObject target;
    [SerializeField] public float bpm;
    private Sprite targetSpr;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        basePos = new Vector3(-0.24f, -1.6f, 0f);
        target.SetActive(false);
        //StartBobbing();
        targetSpr = GetComponent<Sprite>();
    }

    void Update()
    {
        if (isBobbing)
        {
            bobTimer += Time.deltaTime;

            bobbingSpeed = (bpm / 60f) * Mathf.PI / 2;

            float bobWave = Mathf.Abs(Mathf.Cos(bobTimer * bobbingSpeed)) + kickOfs;
            float yOffset = (bobWave * bobbingAmp);
            float newY = basePos.y + yOffset;
            newY = Mathf.Max(newY, clampMin);
            Vector3 newPos = new Vector3(basePos.x, newY, basePos.z);
            target.transform.position = newPos;
        }
    }

    public void StartBobbing()
    {
        isBobbing = true;
    }
    public void StopBobbing()
    {
        isBobbing = false;
    }

    public void SetThreatened()
    {
        targetSpr = threatened;
    }

    public void SetNeutral()
    {
        targetSpr = neutral;
    }

    public void SetSeduced()
    {
        targetSpr = seduced;
    }
}
