using UnityEngine;
using UnityEngine.UI;

public class BackButtonSettings : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Back);
    }

    private void Back()
    {
        SettingsController.Instance.CloseSettings();
    }

    void Update()
    {

    }
}
