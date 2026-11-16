using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DancePhone : MonoBehaviour
{
    [SerializeField] Sprite phoneSpr;
    public static DancePhone Instance { get; private set; }

    private float bobbingSpeed;

    public float bobbingAmp;
    public float kickOfs; // beat of offset
    public float bobTimer;
    public float clampMin;
    private bool isBobbing;
    private Vector3 basePos;

    private float baseAngle;
    [SerializeField] private float angleTilt;

    [SerializeField] public GameObject phone;
    [SerializeField] public float bpm;
    [SerializeField] private SpriteRenderer phoneSR;

    [SerializeField] private GameObject leftText;
    [SerializeField] private GameObject rightText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //targetSpr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        basePos = new Vector3(-0.24f, -1.6f, 0f);
        phone.SetActive(false);
        //StartBobbing();
        baseAngle = transform.eulerAngles.z;
        angleTilt = 12f;

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
            phone.transform.position = newPos;

            // TILT LEFT AND RIGHT
            float newAngle = baseAngle + angleTilt * Mathf.Sin(bobTimer * bobbingSpeed) + kickOfs;
            phone.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
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

    public void SetText()
    {
        leftText.GetComponent<TextMeshProUGUI>().text = "CHARM";
        rightText.GetComponent<TextMeshProUGUI>().text = "REASON";
    }

}