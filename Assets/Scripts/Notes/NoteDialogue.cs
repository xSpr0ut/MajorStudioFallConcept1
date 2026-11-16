using TMPro;
using UnityEngine;

public class NoteDialogue : Note
{
    public string textToDisplay;

    [SerializeField] private TextMeshProUGUI textToChange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialTime = AudioSettings.dspTime;

        originalLocation = transform.position;
        textToChange.text = textToDisplay;
    }


    override
    protected void move()
    {
        //double currentTime = AudioSettings.dspTime;

        //if (currentTime >= topStartTime)
        //{

        //}
    }
}
