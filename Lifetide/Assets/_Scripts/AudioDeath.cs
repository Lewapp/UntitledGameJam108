using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDeath : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource.isPlaying && audioSource.time > 0f)
        {
            Destroy(gameObject);
        }
    }
}
