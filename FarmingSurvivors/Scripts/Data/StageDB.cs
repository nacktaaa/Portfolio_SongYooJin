using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class StageDB : ScriptableObject
{
	public List<RandomStage> RandomStage; 
	public List<PhaseStage> PhaseStage; 
	public List<MonsterPool> MonsterPool; 
	
	public Dictionary<string, RandomStage> MakeRandomStageDic()
	{
		Dictionary<string, RandomStage> newDic = new Dictionary<string, RandomStage>();
		foreach(var item in RandomStage)
		{
			newDic.Add(item.StageID, item);
		}
		
		return newDic;
	}

	public Dictionary<string, PhaseStage> MakePhaseStageDic()
	{
		Dictionary<string, PhaseStage> newDic = new Dictionary<string, PhaseStage>();
		foreach(var item in PhaseStage)
		{
			item.phasePools = SplitString(item.PhasePool);
			item.phasePers = SplitString(item.PhasePer);
			newDic.Add(item.StageID, item);
		}
		return newDic;
	}

	public Dictionary<string, List<string>> MakeMonsterPoolDic()
	{
		Dictionary<string, List<string>> newDic = new Dictionary<string, List<string>>();
		foreach(var item in MonsterPool)
		{
			item.monsters = SplitString(item.Monsters);
			newDic.Add(item.PoolID, item.monsters);
		}
		return newDic;
	}

	public List<string> SplitString(string str)
	{
		string[] arr = str.Split(',');
        for(int i = 0; i < arr.Length; i ++)
            arr[i] = arr[i].Trim();
		
		List<string> newList = new List<string>();
		foreach(var c in arr)
			newList.Add(c);

        return newList;
	}	

}
