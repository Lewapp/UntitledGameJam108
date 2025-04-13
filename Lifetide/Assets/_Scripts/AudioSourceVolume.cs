using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceVolume : MonoBehaviour
{
    public bool isMusic;
    public float maxVolume;

    private AudioSource volumeSource { get => GetComponent<AudioSource>(); }

    private void Update()
    {
        if (!GlobalData.playerInfo)
            return;

        if (isMusic)
            volumeSource.volume = maxVolume * GlobalData.playerInfo.musicVolume;
        else
            volumeSource.volume = maxVolume * GlobalData.playerInfo.sfxVolume;
    }
}
