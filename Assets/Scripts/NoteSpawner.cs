using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Note prefabs here")]
    [SerializeField] private GameObject barNote;
    [SerializeField] private GameObject longBarNote;
    [SerializeField] private GameObject circleNote;     // needs a HitCircle component
    [SerializeField] private GameObject longCircleNote; // needs a HoldCircle component

    [Header("Note locations here")]
    [Tooltip("The D, F, J, K transforms under speakersL / speakersR, in that order")]
    [SerializeField] private List<Transform> level1locs = new List<Transform>();

    [Header("Sliders")]
    [Tooltip("Optional direction per lane (D, F, J, K) for slider paths. Leave empty / zero to hang straight down.")]
    [SerializeField] private List<Vector2> sliderDirections = new List<Vector2>();

    [Header("Approach ring")]
    [Tooltip("Start each approach ring at the size of the NoteCircleOutline under that key, so it looks like the outline shrinks onto the circle")]
    [SerializeField] private bool startRingAtOutlineSize = true;

    public List<Transform> level4locs = new List<Transform>();

    private static readonly string[] laneNames = { "D", "F", "J", "K" };

    private void Awake()
    {
        resolveLanes(true);
    }

    private void resolveLanes(bool logErrors)
    {
        while (level1locs.Count < laneNames.Length) level1locs.Add(null);

        Transform[] all = null;

        for (int i = 0; i < laneNames.Length; i++)
        {
            if (level1locs[i] != null) continue;

            if (all == null) all = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (Transform t in all)
            {
                if (t.gameObject.scene.IsValid() && t.name.Trim() == laneNames[i])
                {
                    level1locs[i] = t;
                    break;
                }
            }

            if (level1locs[i] == null && logErrors)
            {
                Debug.LogError("NoteSpawner: Level1locs slot " + i + " is empty and no object named '" + laneNames[i] + "' is in the scene. Drag it into Level1locs.");
            }
        }
    }

    public OsuNote spawnVisualCue(VisualCue cue, double startTime)
    {
        if (level1locs.Count <= cue.lane || level1locs[cue.lane] == null) resolveLanes(false);
        Transform target = level1locs[cue.lane];
        if (target == null) return null;
        GameObject notePrefab = cue.hold ? longCircleNote : circleNote;

        GameObject n = GameManager.Instance.spawnAtLocation(notePrefab, target.position);

        OsuNote note = n.GetComponent<OsuNote>();
        if (note == null)
        {
            Debug.LogError(notePrefab.name + " is missing a HitCircle / HoldCircle component");
            Destroy(n);
            return null;
        }

        if (cue.hold)
        {
            HoldCircle hold = note as HoldCircle;
            if (hold != null)
            {
                hold.setSlider(getSliderDirection(cue.lane), cue.topEndTime + startTime);
            }
        }

        // if the prefab's Ring Sprite is empty, use this key's NoteCircleOutline image
        SpriteRenderer outline = target.GetComponentInChildren<SpriteRenderer>(true);
        if (outline != null) note.setRingSpriteIfEmpty(outline.sprite);

        if (startRingAtOutlineSize)
        {
            float outlineWidth = getOutlineWidth(target);
            if (outlineWidth > 0f) note.setApproachStartWidth(outlineWidth);
        }

        note.setup(target, cue.spawnTime + startTime, cue.actualTime + startTime);

        return note;
    }

    // world width of the NoteCircleOutline under a key, 0 if there isn't one
    private float getOutlineWidth(Transform key)
    {
        SpriteRenderer outline = key.GetComponentInChildren<SpriteRenderer>(true);
        if (outline == null || outline.sprite == null) return 0f;
        return OsuNote.visibleWidth(outline.sprite) * Mathf.Abs(outline.transform.lossyScale.x);
    }

    public Vector3 getLanePosition(int lane)
    {
        if (lane < 0 || lane >= level1locs.Count || level1locs[lane] == null) return Vector3.zero;
        return level1locs[lane].position;
    }

    private Vector2 getSliderDirection(int lane)
    {
        if (lane < sliderDirections.Count && sliderDirections[lane] != Vector2.zero)
        {
            return sliderDirections[lane].normalized;
        }

        // default: the slider hangs straight down from the circle
        return Vector2.down;
    }
}