using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.Hierarchy.Editor;
using System.Collections;
using UnityEditor.AssetImporters;
using NUnit.Framework;

[RequireComponent(typeof(TMP_Text))]
public class Text : MonoBehaviour
{
    private TMP_Text _textBox;

    [Header("Test String")]
    [SerializeField] private string testText;
    private int _currentVisibleCharacterIndex;
    private Coroutine _typewriterCoroutine;
    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _interpunctuationDelay;

    [Header("Text settings")]
    [SerializeField] private float charactersPerSecond = 20;
    [SerializeField] private float interpunctuationDelay = 0.5f;

    //Events or something

    private void Awake()
    {
        _textBox = GetComponent<TMP_Text>();
        _simpleDelay = new WaitForSeconds(1/charactersPerSecond);
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetText(testText);
    }

    public void SetText(string text)
    {
        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }
        _textBox.text = text;
        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        _typewriterCoroutine = StartCoroutine(Typewriter());
    }

    private IEnumerator Typewriter()
    {
        TMP_TextInfo textInfo = _textBox.textInfo;
        while (_currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;
            _textBox.maxVisibleCharacters++;

            if (character == '?' || character == '.' || character == ',' || character == ':'
            || character == ';' || character == '!' || character == '-')
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                yield return _simpleDelay;
            }

            _currentVisibleCharacterIndex++;
        }
    }
}