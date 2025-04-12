using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolume : MonoBehaviour
{
    public bool isMusic;
    public PlayerInfo playerInfo;
    public float maxVolume;

    private AudioSource volumeSource { get => GetComponent<AudioSource>(); }

    private void Update()
    {
        if (isMusic)
            volumeSource.volume = maxVolume * playerInfo.musicVolume;
        else
            volumeSource.volume = maxVolume * playerInfo.sfxVolume;
    }
}
