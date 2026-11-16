using UnityEngine;
using UnityEngine.UI;

public class SettingsSliders : MonoBehaviour
{
    [SerializeField] AudioSource audio;
    [SerializeField] Slider slider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        audio.volume = slider.value * 0.7f;
    }
}
