using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongSelectionButton : MonoBehaviour
{
    [SerializeField] public string selectionScene;

    Button b;
    void Start()
    {
        b = this.GetComponent<Button>();
        b.onClick.AddListener(Play);
    }

    private void Play()
    {
        SceneManager.LoadScene(selectionScene);
    }

    void Update()
    {

    }
}
