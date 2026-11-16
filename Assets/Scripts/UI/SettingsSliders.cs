using UnityEngine;
using UnityEngine.UI;

public class SettingsSliders : MonoBehaviour
{
    [SerializeField] AudioSource audio;
    [SerializeField] Slider slider;
    [SerializeField] PulsingCamera cam;
    [SerializeField] Slider slider2;

    void Start()
    {
        //slider.value = 1f;
        //slider2.value = 1f;
    }

    void Update()
    {
        audio.volume = slider.value * 0.7f;
        cam.pulsingAmp = slider2.value;
    }
}
