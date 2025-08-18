using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider audioSlider;

    private void Start()
    {
        if(PlayerPrefs.HasKey("Audio"))
        {
            loadVolume();
        }
        else
        { 
            setAudioVolume(); 
        }
    }
    public void setAudioVolume()
    {
        float volume = audioSlider.value;
        audioMixer.SetFloat("Audio", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Audio", volume);
    }

    private void loadVolume()
    {
        audioSlider.value = PlayerPrefs.GetFloat("Audio");
        setAudioVolume();
    }
}
