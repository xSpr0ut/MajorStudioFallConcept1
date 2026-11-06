using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour
{
    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Quit);
    }

    private void Quit()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void Update()
    {

    }
}
