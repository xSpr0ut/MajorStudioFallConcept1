using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEngine.InputSystem;

public class SongSelector : MonoBehaviour
{
    [SerializeField] private TextAsset listOfSongs;

    [Header("ui")]
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject songPrefab;
    [SerializeField] private GameObject selectedSongFrame;
    [SerializeField] private TextMeshProUGUI selectedSongName;

    private RectTransform cc;
    private int songPrefabHeight = 100;

    private int selectedSong;
    private bool canStart = false;

    private float minScroll = -250f;
    private float maxScroll;

    private float originalPos;
    private float moveTarget;
    private float moveSpeed = 3;
    public struct Song
    {
        public string songName;
        public string chartFilePath;
        public string musicFilePath;
        public string image;

        public Song(string sn, string cfp, string mfp, string img)
        {
            songName = sn;
            chartFilePath = cfp;
            musicFilePath = mfp;
            image = img;
        }
    }

    private List<Song> listOfTracks = new List<Song>();
    
    private void Awake()
    {
        selectedSong = 0;
        readCSV();

        cc = contentContainer.gameObject.GetComponent<RectTransform>();
        cc.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, listOfTracks.Count * songPrefabHeight);

        originalPos = cc.anchoredPosition.y;
        moveTarget = originalPos;

        maxScroll = minScroll + listOfTracks.Count * songPrefabHeight - 100;

        populateList();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(cc.anchoredPosition.y - moveTarget) > moveSpeed)
        {
            cc.anchoredPosition = new Vector2(cc.anchoredPosition.x, cc.anchoredPosition.y + moveSpeed * -Mathf.Sign(cc.anchoredPosition.y - moveTarget));
            selectedSongFrame.SetActive(false);
            canStart = false;
        }
        else
        {
            cc.anchoredPosition = new Vector2(cc.anchoredPosition.x, moveTarget);
            selectedSongFrame.SetActive(true);
            selectedSongFrame.GetComponent<Image>().sprite = Resources.Load<Sprite>("SongImages/" + listOfTracks[selectedSong].image);
            selectedSongName.text = listOfTracks[selectedSong].songName;
            canStart = true;

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                GameManager.Instance.startGame(listOfTracks[selectedSong]);
                gameObject.SetActive(false);
            }
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            selectedSong = Mathf.Clamp(selectedSong - 1, 0, listOfTracks.Count - 1);
            moveTarget = Mathf.Clamp(moveTarget - songPrefabHeight, minScroll, maxScroll);

        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            selectedSong = Mathf.Clamp(selectedSong + 1, 0, listOfTracks.Count - 1);
            moveTarget = Mathf.Clamp(moveTarget + songPrefabHeight, minScroll, maxScroll);
        }
    }

    private void readCSV()
    {
        string assetPath = AssetDatabase.GetAssetPath(listOfSongs);

        foreach (string line in File.ReadLines(assetPath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Split line into columns based on commas
            string[] columns = line.Split(',');
            
            if (columns[0].Equals("NAME")) // ignore first line
            {
                continue;
            }

            string songName;
            string chartFilePath;
            string musicFilePath;
            string imageFilePath;

            songName = columns[0].Trim();
            chartFilePath = columns[1].Trim();
            musicFilePath = columns[2].Trim();
            imageFilePath = columns[3].Trim();

            Song song = new Song(songName, chartFilePath, musicFilePath, imageFilePath);
            listOfTracks.Add(song);
        }
    }
    private void populateList()
    {
        foreach (Song song in listOfTracks)
        {
            GameObject placedSong = Instantiate(songPrefab, contentContainer);

            TextMeshProUGUI textComp = placedSong.GetComponentInChildren<TextMeshProUGUI>();
            textComp.text = song.songName;

            // can you BELIEVE getcomponentinchildren includes the calling gameobject!!?
            Image[] i = placedSong.GetComponentsInChildren<Image>();
            i[1].sprite = Resources.Load<Sprite>("SongImages/" + song.image);
        }
    }
}
