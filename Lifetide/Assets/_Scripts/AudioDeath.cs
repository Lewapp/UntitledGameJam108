using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDeath : MonoBehaviour
{
    private AudioSource audioSource;
    private bool audioStarted;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (audioSource.time > 0f)
        {
            audioStarted = true;
        }

        if (!audioSource.isPlaying && audioStarted)
        {
            Destroy(gameObject);
        }
    }
}
