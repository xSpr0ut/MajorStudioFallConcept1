using UnityEngine;

public class PulsingCamera : MonoBehaviour
{
    public static PulsingCamera Instance { get; private set; }

    private float pulsingSpeed;

    public float pulsingAmp;
    public float camBase;
    public float pulseTimer;
    public float kickOfs; // close camera
    public float clampMin; // RAISE THIS FOR LONGER PAUSE
    private bool isPulsing;
    public float beatOfs; // time the beat start with song start

    [SerializeField] public Camera cam;
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
        cam = GetComponent<Camera>();
        camBase = cam.orthographicSize;

        //StartPusling();
    }

    void Update()
    {
        if (isPulsing)
        {
            pulseTimer += Time.deltaTime;

            pulsingSpeed = bpm / 60f * Mathf.PI;
            //float pulseWave = Mathf.Abs(Mathf.Sin(pulseTimer * pulsingSpeed) + beatOfs);
            float pulseWave = Mathf.Abs(Mathf.Cos(pulseTimer * pulsingSpeed) + beatOfs);
            float cam_ofs = pulseWave * pulsingAmp + kickOfs;
            cam.orthographicSize = Mathf.Clamp(camBase - cam_ofs, clampMin, Mathf.Infinity);

            //Debug.Log("cam.orth")
        }
    }


    public void StartPulsing()
    {
        isPulsing = true;
    }
    public void StopPulsing()
    {
        isPulsing = false;
    }
}
