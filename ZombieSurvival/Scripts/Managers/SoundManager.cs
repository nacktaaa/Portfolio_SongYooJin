using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    // 전역으로 관리할 오디오 소스 (플레이어와 좀비는 각자 들고 있음)
    // : BGM, Triggered, Effect 

    public AudioMixer audioMixer;
    public AudioSource BGMplayer;
    AudioSource TriggerPlayer;
    AudioSource EffectPlayer;

    // 오디오 클립 모두 들고 있기

    // 좀비 사운드 딕셔너리 Dictionarry<string, AudioClip>
    // FIdle(1~N) , FDeath(1~N), FEating, MIdle(1~N), MDeath(1~N), MEating, Bite1, Bite2, 
    // Thump, Smash1, Smash2
    public Dictionary<string, AudioClip> zombieSoundsDict = new Dictionary<string, AudioClip>();

    // 플레이어 사운드 딕셔너리 Dictionarry<string, AudioClip>
    // FootStepNary, FootStepSusan, FootStepBob, FHeavyBreath, MHeavyBreath, Heartbeat
    public Dictionary<string, AudioClip> playerSoundsDict = new Dictionary<string, AudioClip>();

    // BGM 딕셔너리 Dictionarry<string, AudioClip>
    // Intro, Ambient, Death, FindZombie, Chase1, Chase2
    public Dictionary<string, AudioClip> BGMSoundsDict = new Dictionary<string, AudioClip>();

    // Trigger 딕셔너리
    // BreakDoor, Death, Save, Stab1, Stab2, Stab3
    public Dictionary<string, AudioClip> triggerSoundsDict = new Dictionary<string, AudioClip>();

    // Effect 딕셔너리
    // FireRifle, CloseDoor, OpenDoor, Mount, CloseBag, TakeMedicine, DrinkWater
    // Bandage, Eating, OpenBag, PutInBag, Click, Reload, OpenStorage, Ticking
    public Dictionary<string, AudioClip> effectSoundsDict = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject go = GameObject.Find("@Sounds");
        if(go == null)
        {
            go = new GameObject("@Sounds");
            Object.DontDestroyOnLoad(go);
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
        }

        AudioClip[] zombieSounds = Resources.LoadAll<AudioClip>("Sounds/Zombie");
        AudioClip[] playerSounds = Resources.LoadAll<AudioClip>("Sounds/Player");
        AudioClip[] BGMSounds = Resources.LoadAll<AudioClip>("Sounds/BGM");
        AudioClip[] triggerSounds = Resources.LoadAll<AudioClip>("Sounds/TriggerMusic");
        AudioClip[] effectSounds = Resources.LoadAll<AudioClip>("Sounds/Effect");

        foreach(AudioClip clip in zombieSounds)
        {
            zombieSoundsDict.Add(clip.name, clip);
            //Debug.Log(zombieSoundsDict[clip.name]);
        }
        
        foreach(AudioClip clip in playerSounds)
            playerSoundsDict.Add(clip.name, clip);

        foreach(AudioClip clip in BGMSounds)
            BGMSoundsDict.Add(clip.name, clip);    

        foreach(AudioClip clip in triggerSounds)
            triggerSoundsDict.Add(clip.name, clip);    

        foreach(AudioClip clip in effectSounds)
            effectSoundsDict.Add(clip.name, clip);            
        
    }

    
    public void PlayBGM(Define.BGM bgm)
    {
        switch(bgm)
        {
            case Define.BGM.Intro :
            case Define.BGM.Death :
            case Define.BGM.Ambient :
                BGMplayer.loop = true;
                break;
            case Define.BGM.Chase1 :
            case Define.BGM.Chase2 :
            case Define.BGM.FindZombie :
                BGMplayer.loop = false;
                break;
        }
        string key = bgm.ToString();
        if(BGMplayer.clip != BGMSoundsDict[key])
        {
            BGMplayer.clip = BGMSoundsDict[key];
            BGMplayer.Play();
        }
    }

    public void PlayTriggerMusic(Define.TriggerSound triggerMusic)
    {
        string key = triggerMusic.ToString();
        TriggerPlayer.PlayOneShot(triggerSoundsDict[key]);
    }

    public void PlayEffectSound(Define.EffectSound effectSound)
    {
        string key = effectSound.ToString();
        EffectPlayer.PlayOneShot(effectSoundsDict[key]);
    }

    public void StopEffectSound()
    {
        EffectPlayer.Stop();
    }

    public void Clear()
    {
        //Debug.Log("SoundManager Clear");
        zombieSoundsDict.Clear();
        playerSoundsDict.Clear();
        BGMSoundsDict.Clear();
        triggerSoundsDict.Clear();
        effectSoundsDict.Clear();
    }

}
