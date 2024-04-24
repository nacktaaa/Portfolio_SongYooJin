using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionCanvas : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider SFXSlider;
    public Toggle musicToggle;
    public Toggle SFXToggle;

    private void OnEnable()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        musicToggle.onValueChanged.AddListener(ToggleMusic);
        SFXToggle.onValueChanged.AddListener(ToggleSFX);

        Managers.Sound.audioMixer.GetFloat("Master", out float masterVol);
        Managers.Sound.audioMixer.GetFloat("Master", out float musicVol);
        Managers.Sound.audioMixer.GetFloat("Master", out float SFXVol);
    }

    private void OnDisable()
    {
        masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);    
        SFXSlider.onValueChanged.RemoveListener(SetSFXVolume);
        musicToggle.onValueChanged.RemoveListener(ToggleMusic);
        SFXToggle.onValueChanged.RemoveListener(ToggleSFX);
    }  

    public void SetMasterVolume(float _volume)
    {
        Managers.Sound.audioMixer.SetFloat("Master", Mathf.Log10(_volume)*20);
    }

    public void SetMusicVolume(float _volume)
    {
        Managers.Sound.audioMixer.SetFloat("Music", Mathf.Log10(_volume)*20);
    }

    public void SetSFXVolume(float _volume)
    {
        Managers.Sound.audioMixer.SetFloat("SFX", Mathf.Log10(_volume)*20);
    }

    public void ToggleMusic(bool _on)
    {
        if(_on)
            Managers.Sound.audioMixer.SetFloat("Music", 0);
        else
            Managers.Sound.audioMixer.SetFloat("Music", -50);
    }

    public void ToggleSFX(bool _on)
    {
        if(_on)
            Managers.Sound.audioMixer.SetFloat("SFX", 0);
        else
            Managers.Sound.audioMixer.SetFloat("SFX", -50);
    }
}
