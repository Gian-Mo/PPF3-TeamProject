using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class volume_settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;

    private void Start()
    {
        Load();
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

    public void Save()
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("SFX", effectsSlider.value);
    }
    public void Load()
    {
       musicSlider.value =  PlayerPrefs.GetFloat("Music");
       effectsSlider.value = PlayerPrefs.GetFloat("SFX");
       audioMixer.SetFloat("music", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
       audioMixer.SetFloat("sfx", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);


    }
}
