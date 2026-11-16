using UnityEngine;

public class DanceTarget : MonoBehaviour
{
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
        StartBobbing();
    }

    void Update()
    {
        if (isBobbing)
        {
            bobbingSpeed = bpm / 60f * Mathf.PI;

            float bobWave = Mathf.Abs(Mathf.Cos(bobTimer * bobbingSpeed));
            float cam_ofs = bobWave * bobbingAmp + kickOfs;
            Vector3 camofsv = new Vector3(basePos.x, cam_ofs, basePos.z);
            camofsv.y = Mathf.Max(camofsv.y, clampMin);

            transform.position = camofsv;
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
}
