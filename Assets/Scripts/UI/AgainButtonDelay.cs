using UnityEngine;
using System.Collections;

public class AgainButtonDelay : MonoBehaviour
{
    [SerializeField] public GameObject b;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DelayShowButton());
        b.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DelayShowButton()
    {
        yield return new WaitForSeconds(1.5f);
        b.SetActive(true);

    }
}
