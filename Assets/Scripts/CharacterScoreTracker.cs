using UnityEngine;

public class CharacterScoreTracker : MonoBehaviour
{

    private float movementUnit = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        float x = GameManager.Instance.getCharacterScore() * movementUnit;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

    }
}
