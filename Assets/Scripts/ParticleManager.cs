using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem longParticles;
    [SerializeField] private ParticleSystem shortParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        longParticles.Stop();
        shortParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateParticles()
    {
        longParticles.gameObject.SetActive(true);
        shortParticles.gameObject.SetActive(true);
        longParticles.Play();
        shortParticles.Play();
    }

    public void stopParticles()
    {
        longParticles.gameObject.SetActive(false);
        shortParticles.gameObject.SetActive(false);
        longParticles.Stop();
        shortParticles.Stop();
    }

    public bool isPlaying()
    {
        return longParticles.isPlaying;
    }
}
