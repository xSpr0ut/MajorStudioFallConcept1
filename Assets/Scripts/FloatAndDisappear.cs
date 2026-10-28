using System.Collections;
using TMPro;
using UnityEngine;

public class FloatAndDisappear : MonoBehaviour
{
    float xdir;
    float zdir;
    float yspd = 0.2f;

    public string text;
    public Color color;

    public TextMeshProUGUI textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro.text = text;
        textMeshPro.color = color;

        xdir = Random.value * 0.6f - 0.3f;
        zdir = Random.value * 0.6f - 0.3f;

        StartCoroutine(destroyAfterSecs());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(xdir, yspd, zdir) * Time.deltaTime;
    }

    IEnumerator destroyAfterSecs()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
