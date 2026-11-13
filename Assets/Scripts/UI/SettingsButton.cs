using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Settings);
    }

    private void Settings()
    {
        SettingsController.Instance.OpenSettings();
    }

    void Update()
    {

    }
}
