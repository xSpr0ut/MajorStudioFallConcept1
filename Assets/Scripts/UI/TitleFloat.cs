using UnityEngine;

public class TitleFloat : MonoBehaviour
{
    /* Allows the bottle service sign to float like a sin wave~~~
     * Modify values in editor */

    [SerializeField] private float amp = 40f;
    [SerializeField] private float speed = 4f;
    private RectTransform rectTransform;
    private Vector2 startPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float targY = Mathf.Sin(Time.unscaledTime * speed) * amp;
        rectTransform.anchoredPosition = startPosition + new Vector2(0, targY);
    }
}
