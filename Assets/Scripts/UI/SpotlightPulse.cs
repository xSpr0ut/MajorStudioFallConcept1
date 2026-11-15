
using UnityEngine;
using UnityEngine.UI;

public class SpotlightPulse : MonoBehaviour
{
    [SerializeField] float bpm = 120f;
    [SerializeField] bool secondHandbeat;

    private float beatLength;
    private Image img;
    private float timer;
    private int beats;
    private Color c;

    void Start()
    {
        beatLength = 60f / Mathf.Max(bpm, 0.01f);
        img = GetComponent<Image>();
        c = img.color;

        timer = 0.4f;
        beats = 0;

        ManageSpotlight();
    }

    void Update()
    {
        ManageTime();
        ManageSpotlight();
    }

    void ManageTime()
    {
        timer += Time.deltaTime;

        while (timer >= beatLength)
        {
            beats++;
            timer -= beatLength;
        }
    }

    void ManageSpotlight()
    {
        if (beats % 2 == 0)
        {
            if (secondHandbeat)
            {
                c.a = 0.4f;
            }
            else
            {
                c.a = 1f;
            }
        }
        else
        {
            if (secondHandbeat)
            {
                c.a = 1f;
            }
            else
            {
                c.a = 0.4f;
            }
        }
        img.color = c;
    }
}
