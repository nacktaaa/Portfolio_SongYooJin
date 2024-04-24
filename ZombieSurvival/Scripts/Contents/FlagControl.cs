using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagControl 
{
    public Action<Define.FlagType, int> FlagAddOrChangeEvent;
    public Action<Define.FlagType> FlagRemoveEvent;
    Define.FlagType curflag = Define.FlagType.None;
    public Define.FlagType CurrentFlag
    {
        get { return curflag;}
        set 
        { 
            curflag = value;
        }
    }
    Dictionary<Define.FlagType, int> curFlagDict = new Dictionary<Define.FlagType, int>();

    public bool HasFlag(Define.FlagType flagType)
    {
        if(CurrentFlag.HasFlag(flagType))
            return true;
        else
            return false;
    }

    public int GetFlagStep(Define.FlagType flagType)
    {
        if(curFlagDict.ContainsKey(flagType))
            return curFlagDict[flagType];

        return 0;
    }

    public void AddFlag(Define.FlagType flagType)
    {
        if(!curFlagDict.ContainsKey(flagType))
        {
            CurrentFlag |= flagType;
            curFlagDict.Add(flagType, 1);
            
            if(flagType == Define.FlagType.Zombie)
                RemoveAllFlag();
            
            Managers.UI.AddFlagUI(flagType);
            FlagAddOrChangeEvent.Invoke(flagType, curFlagDict[flagType]);
        }
        else
        {
            ChangeFlagStep(flagType, isPlus: true);
        }
    }

    public void ChangeFlagStep(Define.FlagType flagType, bool isPlus)
    {
        if(!curFlagDict.ContainsKey(flagType))
            return;

        if(isPlus)
        {
            curFlagDict[flagType]++;
            if(curFlagDict[flagType] > 3)
                curFlagDict[flagType] = 3;
        }
        else
        {
            curFlagDict[flagType]--;
            if(curFlagDict[flagType]<= 0)
                curFlagDict[flagType] = 0;
        }

        switch(curFlagDict[flagType])
        {
            case 0 :
            {
                RemoveFlag(flagType);
            }
                break;
            case 1 :
            {
                Managers.UI.ChangeFlagUI(flagType, curFlagDict[flagType]);
                FlagAddOrChangeEvent.Invoke(flagType, curFlagDict[flagType]);
            }
                break;
            case 2 :
            {
                Managers.UI.ChangeFlagUI(flagType, curFlagDict[flagType]);
                FlagAddOrChangeEvent.Invoke(flagType, curFlagDict[flagType]);
            }
                break;
            case 3 :
            {
                RemoveAllFlag();
                AddFlag(Define.FlagType.Death);
                Managers.Game.causeOfDeath = flagType.ToString();
            }
                break;
            default :
                break;
        }
    }

    public void RemoveFlag(Define.FlagType flagType)
    {
        if(CurrentFlag.HasFlag(flagType))
        {
            CurrentFlag &= ~flagType;
            curFlagDict.Remove(flagType);
            Managers.UI.RemoveFlagUI(flagType);
            FlagRemoveEvent.Invoke(flagType);
        }
    }
    
    public void RemoveAllFlag()
    {
        Managers.UI.RemoveAllFlagUI();
        CurrentFlag = Define.FlagType.None;
        FlagRemoveEvent.Invoke(Define.FlagType.None);
    }

    public Dictionary<Define.FlagType, int> SaveCurFlags()
    {
        Dictionary<Define.FlagType, int> newDict = new Dictionary<Define.FlagType, int>();
        foreach(KeyValuePair<Define.FlagType, int> item in curFlagDict)
        {
            newDict.Add(item.Key, item.Value);
        }
        return newDict;
    }

    public void LoadCurFlags(Dictionary<Define.FlagType, int> savedFlagDict)
    {
        foreach(KeyValuePair<Define.FlagType, int> saveFlag in savedFlagDict)
        {
            CurrentFlag |= saveFlag.Key;
            curFlagDict.Add(saveFlag.Key, saveFlag.Value);
            Managers.UI.AddFlagUI(saveFlag.Key);
            Managers.UI.ChangeFlagUI(saveFlag.Key, saveFlag.Value);
        }
    }
}

