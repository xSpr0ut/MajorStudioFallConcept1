using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Continue);
    }

    private void Continue()
    {
        PauseController.Instance.CloseMenu();
    }

    void Update()
    {

    }
}
