using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] public string mainMenuScene;

    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Play);
    }

    private void Play()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    void Update()
    {

    }
}
