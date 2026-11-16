using UnityEngine;
using UnityEngine.Video;
public class EndScene : MonoBehaviour
{
    public static EndScene Instance { get; private set; }

    [SerializeField] private GameObject text;
    [SerializeField] private GameObject art;

    private VideoPlayer artPlayer;
    private Text twText;

    [SerializeField] private VideoClip success;
    [SerializeField] private VideoClip failure;
    [SerializeField] private VideoClip notFired;
    [SerializeField] private VideoClip fired;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        artPlayer = art.GetComponent<VideoPlayer>();
        twText = text.GetComponent<Text>();
    }

    void Start()
    {
        SetEnding("notfired");
    }

    public void SetEnding(string ending)
    {
        string endingText;

        switch (ending)
        {
            case "success":
                artPlayer.clip = success;
                endingText = "MISSION SUCCESSFUL.";
                break;

            case "failure":
                artPlayer.clip = failure;
                endingText = "MISSION UNSUCCESSFUL.";
                break;

            case "notfired":
                artPlayer.clip = notFired;
                endingText = "ANOTHER DAY EMPLOYED!";
                break;

            default:
                artPlayer.clip = fired;
                endingText = "HOT AND HUNGRY.";
                break;
        }

        twText.SetText(endingText);
        artPlayer.Play();
    }
}

