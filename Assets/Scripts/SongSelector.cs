using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class SongSelector : MonoBehaviour
{
    [SerializeField] private TextAsset listOfSongs;

    [Header("ui")]
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject songPrefab;
    [SerializeField] private GameObject selectedSongFrame;
    [SerializeField] private TextMeshProUGUI selectedSongName;

    private RectTransform cc;
    private int songPrefabHeight = 50;

    private int selectedSong;
    private bool canStart = false;

    private float minScroll = -125f;
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
    //private Song GRFBMS = new Song("Green Rock Festival", false, "Audio/GRF/", "Charts/Green Rock Festival", "SongImages/GRFEST TITLE", "Charts/traj_angle_green_rock_festival_with_cue", 0.7888, 184.0, "Green_rock_festival.wav"); // BMS shares the CSV's raw timeline, so same offset to match the music
    //private Song GRFCSV = new Song("Green_rock_festival_8keys", true, "Green_rock_festival", "Charts/Green_rock_festival_8keys", "SongImages/GRFEST_1.avi_000000000", "Charts/traj_angle_green_rock_festival_with_cue", 0.7888, 184.0, "Green_rock_festival.wav"); // backing wav at Resources root; chart offset 0.7888 s; 184 bpm

    //private Song GRFCSVEZ = new Song("GREEN ROCK FESTIVAL EASY", true, "Green_rock_festival", "Charts/green_rock_festival_easy_bs_8key", "SongImages/GRFEST_1.avi_000000000", "Charts/traj_angle_green_rock_festival_with_cue", 0.7888, 184.0, "Green_rock_festival.wav"); // backing wav at Reso

    
    private void Awake()
    {
        selectedSong = 0;
        readCSV();

        //listOfTracks.Add(GRFCSV);
        //listOfTracks.Add(GRFCSVEZ);

        cc = contentContainer.gameObject.GetComponent<RectTransform>();
        cc.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, listOfTracks.Count * songPrefabHeight);

        originalPos = cc.anchoredPosition.y;
        moveTarget = originalPos;

        maxScroll = minScroll + listOfTracks.Count * songPrefabHeight - 50;

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

            if (Input.GetKeyDown(KeyCode.D))
            {
                GameManager.Instance.startGame(listOfTracks[selectedSong]);
                gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedSong = Mathf.Clamp(selectedSong - 1, 0, listOfTracks.Count - 1);
            moveTarget = Mathf.Clamp(moveTarget - songPrefabHeight, minScroll, maxScroll);

        }
        else if (Input.GetKeyDown(KeyCode.S))
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
