using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] public string startScene;

    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Play);
    }

    private void Play()
    {
        SceneManager.LoadScene(startScene);
    }

    void Update()
    {
        
    }
}
