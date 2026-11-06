using UnityEngine;
using UnityEngine.UI;

public class CharacterScoreBar : MonoBehaviour
{


    public Image blueFill;
    public float maxScore = 50;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float targetFill = Mathf.InverseLerp(-maxScore, maxScore, GameManager.Instance.getCharacterScore());
        blueFill.fillAmount = Mathf.Lerp(blueFill.fillAmount, targetFill, Time.deltaTime * 5f);


    }
}
