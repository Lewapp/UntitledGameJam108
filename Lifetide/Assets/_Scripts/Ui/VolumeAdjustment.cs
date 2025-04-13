using UnityEngine;
using UnityEngine.UI;

public class VolumeAdjustment : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        musicSlider.value = 0.5f;
        sfxSlider.value = 0.5f;
    }

    private void Update()
    {
        if (GlobalData.playerInfo)
        {
            GlobalData.playerInfo.musicVolume = musicSlider.value;
            GlobalData.playerInfo.sfxVolume = sfxSlider.value;
        }

    }
}
