using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    public Slider BGMSlider;
    public Slider SFXSlider;

    private void OnEnable()
    {
        BGMSlider.onValueChanged.AddListener(SetBGMVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);

        SoundManager.Instance.audioMixer.GetFloat("BGM", out float BGMVol);
        SoundManager.Instance.audioMixer.GetFloat("SFX", out float SFXVol);
        BGMSlider.value = BGMVol;
        SFXSlider.value = SFXVol;
    }

    private void OnDisable()
    {
        BGMSlider.onValueChanged.RemoveListener(SetBGMVolume);    
        SFXSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }  

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.audioMixer.SetFloat("BGM", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.audioMixer.SetFloat("SFX", volume);
    }

}
