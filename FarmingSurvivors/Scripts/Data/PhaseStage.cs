using System.Collections;
using System.Collections.Generic;
using StageMap;
using UnityEngine;

[System.Serializable]
public class PhaseStage 
{
    public string StageID;
    public StageType StageType;
    public int SpawnMax;
    public int StageGold;
    public string PhasePool;
    public string PhasePer;

    public List<string> phasePools;
    public List<string> phasePers;
    

}


