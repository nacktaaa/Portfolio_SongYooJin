using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Equipment;
using Newtonsoft.Json;
using StageMap;
using Unity.VisualScripting;
using UnityEngine;

public enum FarmRewardType
{
    Gold, Equipment
}
public class FarmManager : SingletoneBehaviour<FarmManager>
{
    // 씨앗 생성
    // 소지 씨앗 리스트 (테스트)
    // 스테이지 클리어하여 밭 성장도 누적 
    public Action ChangeSeedsHandler;
    public Action ChangeRewardHandler;
    public FarmSlot[] farmSlots;
    public List<Crop> playerSeeds = new List<Crop>();
    public List<int> rewardGolds = new List<int>();
    public List<Equipment.Equipment> rewardEquips = new List<Equipment.Equipment>();
    public FarmInvenSlot selectInvenSlot = null;
    public CompostSlot selectCompost = null;
    public FarmSlot selectSlot = null;
    public int growthPoint = 0;
    public bool isAutoPlant = false;


    public void Init()
    {
        selectInvenSlot = null;
        farmSlots = GameObject.FindObjectsOfType<FarmSlot>();
        LoadFarm();

        if(!PlayerPrefs.HasKey("seedInventory"))
        {
            foreach(SeedType seed in Enum.GetValues(typeof(SeedType)))
            {
                FarmManager.Instance.CreateSeed(seed, SeedGrade.Normal);
            } 
        }
        UIManager.Instance.farmUI.SetInvenSlots();

        foreach(var s in farmSlots)
            s.Init();

        GrowFarm();
    }

    public void LoadGrowthPoint(int point)
    {
        growthPoint = point;
    }
    public void EnableFarm()
    {
        foreach(var slot in farmSlots)
            slot.EnableCol();
    }

    public void DisableFarm()
    {
        foreach(var slot in farmSlots)
            slot.DisableCol();
    }

    public void HidePriceSlots()
    {
        selectSlot = null;
        foreach(var slot in farmSlots)
            slot.HidePrice();
    }

    public void CreateSeed(SeedType seedType, SeedGrade grade, int count = 1)
    {
        if(selectInvenSlot != null)
        {
            selectInvenSlot.UnSelected();
            selectInvenSlot = null;
        }

        bool hasSeed = false;
        foreach(var item in playerSeeds)
        {
            if(item.cropSetting.seed != seedType)
                continue;
            else if(item.cropSetting.seed == seedType)
            {
                if(item.gradeSetting.grade == grade)
                {
                    item.count += count;
                    hasSeed = true;
                    break;
                }
            }
        }

        if(!hasSeed)
        {
            Crop seed = new Crop(seedType, grade, count);
            playerSeeds.Add(seed);
        }
        ChangeSeedsHandler?.Invoke();
    }
    public List<Crop> CreateSeeds(int amount, SeedGrade seedGrade){
        List<Crop> crops = new List<Crop>();
        for (int i = 0; i < amount; i++){
            SeedType newSeedType = Util.GetRandom.RandomEnum<SeedType>();
            int search = crops.FindIndex(x => x.cropSetting.seed == newSeedType);
            if (search == -1){
                var sp = new Crop(newSeedType, seedGrade, 1);
                crops.Add(sp);
            }
            else {
                crops[search].count += 1;
            }
        }

        foreach (var crop in crops){
            FarmManager.Instance.CreateSeed(crop.cropSetting.seed, crop.gradeSetting.grade, crop.count);
        }
        return crops;
    }

    public Crop PopSeedHighestGrade()
    {
        if(playerSeeds.Count == 0)
            return null;

        Crop crop = null;

        foreach(var s in playerSeeds)
        {
            if(s.gradeSetting.grade == SeedGrade.Unique)
            {
                crop = s;
                break;
            }
        }
        if(crop == null)
        {
            foreach(var s in playerSeeds)
            {
                if(s.gradeSetting.grade == SeedGrade.SuperRare)
                {
                    crop = s;
                    break;
                }
            }
        }
        if(crop == null)
        {
            foreach(var s in playerSeeds)
            {
                if(s.gradeSetting.grade == SeedGrade.Rare)
                {
                    crop = s;
                    break;
                }
            }
        }
        if(crop == null)
        {
            foreach(var s in playerSeeds)
            {
                if(s.gradeSetting.grade == SeedGrade.Normal)
                {
                    crop = s;
                    break;
                }
            }
        }
        return crop;
    }

    public void RemoveSeeds(Crop crop)
    {
        if(playerSeeds.Contains(crop))
        {
            crop.count --;
            if(crop.count == 0)
            {
                playerSeeds.Remove(crop);
                if(selectInvenSlot != null)
                {
                    selectInvenSlot.UnSelected();
                    selectInvenSlot = null;
                }
            }
            
            ChangeSeedsHandler?.Invoke();
        }
    }

    public void GrowFarm()
    {
        if(growthPoint > 0)
        {
            foreach(var s in farmSlots)
            {
                if(!s.isEmpty)
                {
                    s.CurGrowthPoint += growthPoint;
                }
            }
            growthPoint = 0;
        }
    }

    public void MakeReward(Crop crop)
    {
        int goldRate = UnityEngine.Random.Range(0, 10);
        if(goldRate > 6)
        {
            // 골드 지급
            int goldValue = MakeRandomGold(crop.gradeSetting.minGold, crop.gradeSetting.maxGold);
            rewardGolds.Add(goldValue);
        }
        else 
        {
            // 장비 지급 
            Equipment.Equipment newEquip = MakeRandomEquipByRate(crop);
            rewardEquips.Add(newEquip);
        }
        ChangeRewardHandler?.Invoke();  // 수확 가능 상태를 확인하고 수확버튼 새로고침
    }

    public void ClearReward()
    {
        rewardGolds.Clear();
        rewardEquips.Clear();
        ChangeRewardHandler?.Invoke();
    }

    private Equipment.Equipment MakeRandomEquipByRate(Crop crop)
    {

        Equipment.Grade rewardGrade;
        Equipment.Parts rewardParts;

        float seedRate = 100 * crop.gradeSetting.rewardRates[Equipment.Grade.SEED];
        float grassRate = 100 * crop.gradeSetting.rewardRates[Equipment.Grade.GRASS];
        float flowerRate = 100 * crop.gradeSetting.rewardRates[Equipment.Grade.FLOWER];
        float fruitRate = 100 * crop.gradeSetting.rewardRates[Equipment.Grade.FRUIT];

        float rnd = UnityEngine.Random.Range(0, 100);
        if(rnd < seedRate)
            rewardGrade = Equipment.Grade.SEED;
        else if (rnd < seedRate + grassRate)
            rewardGrade = Equipment.Grade.GRASS;
        else if (rnd < seedRate + grassRate + flowerRate)
            rewardGrade = Equipment.Grade.FLOWER;
        else
            rewardGrade = Equipment.Grade.FRUIT;
        
        rnd = UnityEngine.Random.Range(0, 100); 
        if(rnd < 100/Enum.GetValues(typeof(Equipment.Parts)).Length)
            rewardParts = crop.cropSetting.rewardParts;
        else
            rewardParts = Util.GetRandom.RandomEnum<Equipment.Parts>();        

        Equipment.Equipment newEquip = EquipManager.Instance.MakeNewEquipment(rewardParts, rewardGrade);
        return newEquip;
    }

    public (SeedType seedType, SeedGrade SeedGrade) RewardRandomSeedbyRate(int curChapter, StageType stageType)
    {
        SeedGrade seedGrade;
        SeedType seedType;

        float normalRate = 0;
        float rareRate = 0;
        float superRareRate = 0;

        if(stageType == StageType.Normal || stageType == StageType.Gold)
        {
            normalRate = 100 * GameManager.Instance.mapConfig.normalRewardRates[SeedGrade.Normal];
            rareRate = 100 * GameManager.Instance.mapConfig.normalRewardRates[SeedGrade.Rare];
            superRareRate = 100 * GameManager.Instance.mapConfig.normalRewardRates[SeedGrade.SuperRare];
        }
        else if(stageType == StageType.Boss)
        {
            normalRate = 100 * GameManager.Instance.mapConfig.bossRewardRates[SeedGrade.Normal];
            rareRate = 100 * GameManager.Instance.mapConfig.bossRewardRates[SeedGrade.Rare];
            superRareRate = 100 * GameManager.Instance.mapConfig.bossRewardRates[SeedGrade.SuperRare];
        }

        float rnd = UnityEngine.Random.Range(0, 100);
        if(rnd < normalRate)
            seedGrade = SeedGrade.Normal;
        else if (rnd < normalRate + rareRate)
            seedGrade = SeedGrade.Rare;
        else if (rnd < normalRate + rareRate + superRareRate)
            seedGrade = SeedGrade.SuperRare;
        else
            seedGrade = SeedGrade.Unique;
        
        seedType = Util.GetRandom.RandomEnum<SeedType>();        
        CreateSeed(seedType, seedGrade);
        return (seedType, seedGrade);
    }

    private int MakeRandomGold(int min, int max)
    {
        List<int> golds = new List<int>();
        int value = min;
        while(value <= max)
        {
            golds.Add(value);
            value += 100;
        }

        return Util.GetRandom.PickRandom(golds);
    }

    public void LoadFarm()
    {
        if(!PlayerPrefs.HasKey("Farm"))
            return;

        var farmJson = PlayerPrefs.GetString("Farm");
        var farm = JsonConvert.DeserializeObject<FarmSaveData>(farmJson, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			});
        foreach(var s in farmSlots)
        {
            foreach(var data in farm.slotDatas)
            {
                if(s.slotIdx == data.slotIdx)
                {
                    s.isEnable = data.isEnable;
                    s.isEmpty = data.isEmpty;
                    if(!s.isEmpty)
                        s.crop = new Crop(data.seed, data.grade);
                    s.CurGrowthPoint = data.CurGrowthPoint;
                    break;
                }
            }
        }
        isAutoPlant = farm.isAuto;
    }

    public void LoadSeeds(string json)
    {
        var seeds = JsonConvert.DeserializeObject<FarmInvenSaveData>(json, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			});
        
            
        foreach(var s in seeds.farmInvenDatas)
            playerSeeds.Add(new Crop(s.seed, s.grade, s.count));
        
    }

    public int GetFarmPrice()
    {
        int count = 0;
        foreach(var slot in farmSlots)
        {
            if (slot.isEnable)
                count ++;
        }

        if ( count >= 12)
            return 4500;
        else if ( count >= 11)
            return 4000;
        else if ( count >= 7 )
            return 2000;
        else if ( count >= 5)
            return 1000;
        else if ( count == 4)
            return 500;
        else
            return 0;

    }
}
