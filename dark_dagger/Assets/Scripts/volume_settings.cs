using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volume_settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    private void Start()
    {
        SetMusicVolume();
        SetSFXVolume();
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume()
    {
        float volume = effectsSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }
}
