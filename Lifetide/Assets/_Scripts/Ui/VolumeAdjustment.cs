using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjustment : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;
    }

    private void Update()
    {
        playerInfo.musicVolume = musicSlider.value;
        playerInfo.sfxVolume = sfxSlider.value;
    }
}
