using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentStats : MonoBehaviour
{
    public static PersistentStats Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string ending;


    public void CalculateEnding()
    {
        if (GameManager.Instance.selectedSong.chartFilePath == "level2Achart")
        {
            if (GameManager.Instance.getCharacterScore() < 20 && GameManager.Instance.getCharacterScore() > -20 && GameManager.Instance.scorer.getScore() == 120000)
            {

                ending = "success";
            }
            else
            {
                ending = "failure";
            }
        }
        else
        {
            if (GameManager.Instance.getCharacterScore() < 20 && GameManager.Instance.getCharacterScore() > -20 && GameManager.Instance.scorer.getScore() == 180000)
            {

                ending =  "notfired";
            }
            else
            {
                ending =  "fired";
            }
        }
        Debug.Log(ending);
        Debug.Log("points" + GameManager.Instance.scorer.getScore());
        Debug.Log("bar points" + GameManager.Instance.getCharacterScore());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Ending")
        {
            EndScene.Instance.SetEnding(ending);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
