using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FlagUI : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprBads = new Sprite[3];
    [SerializeField]
    Sprite[] sprGoods = new Sprite[3];
    [SerializeField]
    Sprite[] sprState;
    Color transparentColor = new Color(0,0,0,0);

    FlagSlot[] flagSlots;

    public void Init()
    {
        Sprite[] sprs = Resources.LoadAll<Sprite>("Images/Flags");
        sprState = new Sprite[sprs.Length - sprBads.Length - sprGoods.Length];

        int badIdx = 0;
        int goodIdx = 0;
        int stateIdx = 0;

        for (int i =0; i < sprs.Length; i++)
        {
            if (sprs[i].name.Contains("bad"))
            {
                sprBads[badIdx++] = sprs[i];
                continue;
            }
            else if(sprs[i].name.Contains("good"))
            {
                sprGoods[goodIdx++] = sprs[i];
                continue;
            }

            sprState[stateIdx++] = sprs[i];
        }
    
        flagSlots = GetComponentsInChildren<FlagSlot>();
    }

    public void AddFlagIcon(Define.FlagType statusEffect)
    {
        foreach (FlagSlot slot in flagSlots)
        {
            if (slot.isEmpty)
            {
                if (statusEffect == Define.FlagType.Full)
                    slot.backImage.sprite = sprGoods[0];
                else
                    slot.backImage.sprite = sprBads[0]; 
                
                slot.backImage.color = Color.white;

                string statusEffectName = statusEffect.ToString();
                if(statusEffectName == "Full")
                    statusEffectName = "Hunger";

                foreach(Sprite sprite in sprState)
                {
                    if(statusEffectName == sprite.name)
                    {
                        slot.stateImage.sprite = sprite; 
                        slot.stateImage.color = Color.white;
                        slot.gameObject.name = sprite.name;
                        slot.isEmpty = false;
                        return;
                    }
                }   
            }
        }
    }

    public void RemoveFlagIcon(Define.FlagType statusEffect)
    {
        string statusEffectName = statusEffect.ToString();
        if(statusEffectName == "Full")
            statusEffectName = "Hunger";

        foreach (FlagSlot slot in flagSlots)
        {
            if(slot.isEmpty)
                continue;
            
            if(slot.gameObject.name == statusEffectName)
            {
                ClearFlagIcon(slot);
                SortFlagIcon();
                return;
            }
        }
    }

    public void RemoveAllFlagIcon()
    {
        foreach(FlagSlot slot in flagSlots)
        {
            ClearFlagIcon(slot);
        }
    }

    public void ChangeFlagIcon(Define.FlagType statusEffect, int step)
    {
        foreach (FlagSlot slot in flagSlots)
        {
            if(slot.isEmpty)
                continue;

            if(slot.gameObject.name == statusEffect.ToString())
            {
                slot.backImage.sprite = sprBads[step];
                return;
            }
        }
    }
    void SortFlagIcon()
    {
        for(int i = 0; i < flagSlots.Length; i++)
        {
            if(i == 0 || flagSlots[i].isEmpty)
                continue;
            
            if(flagSlots[i-1].isEmpty && !flagSlots[i].isEmpty)
            {
                FlagSlot prevSlot = flagSlots[i-1];
                prevSlot.backImage.sprite = flagSlots[i].backImage.sprite;
                prevSlot.stateImage.sprite = flagSlots[i].stateImage.sprite;
                prevSlot.backImage.color = Color.white;
                prevSlot.stateImage.color = Color.white; 
                prevSlot.gameObject.name = flagSlots[i].gameObject.name;
                prevSlot.isEmpty = false;

                ClearFlagIcon(flagSlots[i]);
            }
        }
    }

    void ClearFlagIcon(FlagSlot flagSlot)
    {
        flagSlot.backImage.sprite = null;
        flagSlot.stateImage.sprite = null;
        flagSlot.stateImage.color = transparentColor;
        flagSlot.backImage.color = transparentColor;
        flagSlot.gameObject.name = Define.FlagType.None.ToString();
        flagSlot.isEmpty = true;
    }
}
