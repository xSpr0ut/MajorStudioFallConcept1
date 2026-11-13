using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    private bool isOpen;
    [SerializeField] public GameObject pauseMenu;

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
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen)
            {
                OpenMenu();
            }
            else if (!SettingsController.Instance.GetIsOpen())
            {
                CloseMenu();
            }
        }
    }

    public void OpenMenu()
    {
        isOpen = true;
        pauseMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        isOpen = false;
        pauseMenu.SetActive(false);
    }
}
