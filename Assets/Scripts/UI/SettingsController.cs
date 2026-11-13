using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance { get; private set; }

    private bool isOpen;
    [SerializeField] public GameObject settings;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                CloseSettings();
            }
        }
    }

    public void OpenSettings()
    {
        isOpen = true;
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        isOpen= false;
        settings.SetActive(false);
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }
}
