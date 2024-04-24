using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmSaveData 
{
    public List<SlotData> slotDatas = new List<SlotData>();  
    public bool isAuto;

    public void SaveFarmData()
    {
        if(FarmManager.Instance.farmSlots == null)
            return;
            
        foreach(var s in FarmManager.Instance.farmSlots)
        {
            SlotData data = new SlotData(s.slotIdx, s.isEnable, s.isEmpty, s.CurGrowthPoint, s.crop);
            slotDatas.Add(data);
        }

        isAuto = UIManager.Instance.farmUI.autoPlantToggle.isOn;
    }
}

public class FarmInvenSaveData
{
    public List<FarmInvenData> farmInvenDatas = new List<FarmInvenData>();
    public void SaveFarmInvenData()
    {
        foreach(var s in FarmManager.Instance.playerSeeds)
        {
            FarmInvenData seed = new FarmInvenData(s.cropSetting.seed, s.gradeSetting.grade, s.count);
            farmInvenDatas.Add(seed);
        }
    }
}

public class FarmInvenData
{
    public SeedType seed;
    public SeedGrade grade;
    public int count;

    public FarmInvenData(SeedType seed, SeedGrade grade, int count)
    {
        this.seed = seed;
        this.grade = grade;
        this.count = count;
    }
}

public class SlotData
{
    public int slotIdx;
    public bool isEnable;
    public bool isEmpty;
    public int CurGrowthPoint;
    public SeedType seed;
    public SeedGrade grade;

    public SlotData(int slotIdx, bool isEnable, bool isEmpty, int curGrowthPoint, Crop crop)
    {
        this.slotIdx = slotIdx;
        this.isEnable = isEnable;
        this.isEmpty = isEmpty;
        this.CurGrowthPoint = curGrowthPoint;
        if(!this.isEmpty && crop != null)
        {
            seed = crop.cropSetting.seed;
            grade = crop.gradeSetting.grade;
        }
    }
}


