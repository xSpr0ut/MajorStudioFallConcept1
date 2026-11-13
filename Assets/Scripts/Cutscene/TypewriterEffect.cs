using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    private TMP_Text _textBox;

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    private AudioSource audioSource;

    [Header("Dialogue Lines")]
    [SerializeField] private string[] lines;
    [SerializeField] private float[] startTimes; 
    [SerializeField] private float[] endTimes; 

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;
    // the time (in seconds into the audio) when the next scene loads
    [SerializeField] private float nextSceneTime = 10f;

    private int _currentLine;
    private int _currentVisibleCharacterIndex;
    private bool _isTyping;
    private WaitForSeconds _simpleDelay;

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        _textBox.text = "";
        audioSource.Play();
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        for (_currentLine = 0; _currentLine < lines.Length; _currentLine++)
        {
            while (audioSource.time < startTimes[_currentLine] && audioSource.isPlaying)
            {
                yield return null;
            }

            SetText(lines[_currentLine]);

            while (_isTyping)
            {
                yield return null;
            }
        }

        while (audioSource.time < nextSceneTime && audioSource.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    public void SetText(string text)
    {
        _textBox.text = text;
        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;
        _textBox.ForceMeshUpdate();

        _isTyping = true;
        StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;

        float lineLength = endTimes[_currentLine] - startTimes[_currentLine];
        float delay = lineLength / textInfo.characterCount;
        _simpleDelay = new WaitForSeconds(delay);

        while (_currentVisibleCharacterIndex < textInfo.characterCount)
        {
            if (audioSource.time >= endTimes[_currentLine] || !audioSource.isPlaying)
            {
                _textBox.maxVisibleCharacters = textInfo.characterCount;
                break;
            }

            _textBox.maxVisibleCharacters++;
            _currentVisibleCharacterIndex++;

            yield return _simpleDelay;
        }

        _isTyping = false;
    }
}