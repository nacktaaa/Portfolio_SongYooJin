using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGMSounds
{
    Intro, Ready, Normal, Boss
}
public enum TriggerSound
{
    Clear, GameOver, Heal, LevelUp
}
public enum EffectSound
{
    DropItem
}

public class SoundManager : SingletoneBehaviour<SoundManager>
{
    // 오디오플레이어를 용도에 따라 들고 있음
    public AudioMixer audioMixer;
    public AudioSource BGMplayer;
    public AudioSource TriggerPlayer;
    public AudioSource EffectPlayer;

    // 오디오 파일 모두 들고 있기
    // 용도에 따라 딕셔너리 새로 파기(재생할 오디오플레이어의 구분을 위함)
    // 오디오 파일명과 열거형 이름이 같도록 (딕셔너리 Key가 클립 이름이 되도록. 새 클립은 열거형에 추가)
    public Dictionary<string, AudioClip> BGMSoundsDict = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> triggerSoundsDict = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> effectSoundsDict = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject go = GameObject.Find("@Sounds");
        if(go == null)
        {
            go = new GameObject("@Sounds");
            Object.DontDestroyOnLoad(go);
        }
        audioMixer = Resources.Load<AudioMixer>("Sounds/AudioMixer");
        GameObject bgmSound = new GameObject("BGMPlayer");
        GameObject triggerSound = new GameObject("TriggerPlayer");
        GameObject effectSound = new GameObject("EffectPlayer");
        bgmSound.transform.SetParent(go.transform);
        triggerSound.transform.SetParent(go.transform);
        effectSound.transform.SetParent(go.transform);

        AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups("Master");
        
        BGMplayer = bgmSound.AddComponent<AudioSource>();
        BGMplayer.outputAudioMixerGroup = audioMixerGroups[1];
        BGMplayer.loop = true;
        TriggerPlayer = triggerSound.AddComponent<AudioSource>();
        TriggerPlayer.outputAudioMixerGroup = audioMixerGroups[2];
        EffectPlayer = effectSound.AddComponent<AudioSource>();
        EffectPlayer.outputAudioMixerGroup = audioMixerGroups[2];
        EffectPlayer.ignoreListenerPause = true;

        AudioClip[] BGMSounds = Resources.LoadAll<AudioClip>("Sounds/BGM");
        
        AudioClip[] triggerSounds = Resources.LoadAll<AudioClip>("Sounds/TriggerSound");
        AudioClip[] effectSounds = Resources.LoadAll<AudioClip>("Sounds/Effect");


        foreach(AudioClip clip in BGMSounds)
            BGMSoundsDict.Add(clip.name, clip);
        foreach(AudioClip clip in triggerSounds)
            triggerSoundsDict.Add(clip.name, clip);
        foreach(AudioClip clip in effectSounds)
            effectSoundsDict.Add(clip.name, clip);
        
    }

    // 오디오플레이어 별로 재생 함수가 다름
    public void PlayBGM(BGMSounds bgm)
    {
        if (BGMplayer == null){
            BGMplayer = GameObject.Find("BGMPlayer").GetComponent<AudioSource>();
        }
        string key = bgm.ToString();
        if(BGMplayer.clip == null || BGMplayer.clip != BGMSoundsDict[key])
        {
            BGMplayer.clip = BGMSoundsDict[key];
            BGMplayer.Play();
        }

        if(!BGMplayer.isPlaying)
            BGMplayer.Play();
    }

    public void PlayTriggerSound(TriggerSound trigger)
    {
        if (TriggerPlayer == null){
            TriggerPlayer = GameObject.Find("TriggerPlayer").GetComponent<AudioSource>();
        }
        string key = trigger.ToString();
        TriggerPlayer.PlayOneShot(triggerSoundsDict[key]);
    }

    public void PlayEffectSound(EffectSound effect)
    {
        if (EffectPlayer == null){
            EffectPlayer = GameObject.Find("EffectPlayer").GetComponent<AudioSource>();
        }
        if(EffectPlayer.isPlaying)
            EffectPlayer.Stop();

        string key = effect.ToString();
        EffectPlayer.PlayOneShot(effectSoundsDict[key]);
    }

    public void Clear()
    {
        BGMSoundsDict.Clear();
        triggerSoundsDict.Clear();
        effectSoundsDict.Clear();
    }
}