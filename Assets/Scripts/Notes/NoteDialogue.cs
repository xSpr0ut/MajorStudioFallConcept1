using TMPro;
using UnityEngine;

public class NoteDialogue : Note
{
    private string textToDisplay;

    [SerializeField] private TextMeshProUGUI textToChange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTime = AudioSettings.dspTime;

        originalLocation = transform.position;
        //textToChange.text = textToDisplay;
        //Debug.Log("text in start is " + textToChange.text);
    }


    override
    protected void move()
    {
        //double currentTime = AudioSettings.dspTime;

        //if (currentTime >= topStartTime)
        //{

        //}
    }

    public void changeText(string text)
    {
        textToChange.text = text;
        textToDisplay = text;
        //Debug.Log("text in  method is " + textToChange.text);
    }
}
