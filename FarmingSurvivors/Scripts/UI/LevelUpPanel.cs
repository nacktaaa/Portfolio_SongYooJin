using System;
using System.Collections.Generic;
using Util;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    public int maxSkill = 4;
    public Sprite[] icons;

    public void ShowRandomItem(List<Weapon.Weapon> selectedSkills)
    {
        if (selectedSkills.Count == 0){
            GameManager.Instance.Resume();
            gameObject.SetActive(false);
            return;
        }
        SoundManager.Instance.PlayTriggerSound(TriggerSound.LevelUp);
        SoundManager.Instance.BGMplayer.Pause();
        
        SkillSelectSlot[] arr = GetComponentsInChildren<SkillSelectSlot>();
        foreach (var b in arr){
            b.gameObject.SetActive(false);
        }

        for (int i = 0; i < selectedSkills.Count; i++)
        {   
            arr[i].Init(selectedSkills[i]);
            arr[i].gameObject.SetActive(true);
        }
    }
}
