using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class DataManager
{
    //모든 데이터를 다 들고 있는 클래스 
    // 아이템 데이터
    public List<Item> ItemDB {get; private set;} = new List<Item>();
    public Dictionary<int, Food> FoodDict {get; private set;} = new Dictionary<int, Food>();
    public Dictionary<int, Weapon> WeaponDict {get; private set;} = new Dictionary<int, Weapon>();
    public Dictionary<int, Cure> CureDict {get; private set;} = new Dictionary<int, Cure>();
    public Dictionary<int, Tools> ToolDict {get; private set;} = new Dictionary<int, Tools>();
    
    string savePath;

    public void Init()
    {
        savePath = Managers.saveDataPath;
        // json 파일을 TextAsset 으로 로드하기
        TextAsset textAsset = Resources.Load<TextAsset>("Data/Items");
        ItemData itemData = JsonConvert.DeserializeObject<ItemData>(textAsset.text);

        itemData.Init();
        FoodDict = itemData.MakeDict(itemData.foods);
        WeaponDict = itemData.MakeDict(itemData.weapons);
        CureDict = itemData.MakeDict(itemData.cures);
        ToolDict = itemData.MakeDict(itemData.tools);
    }

    public List<string> LoadSaveFiles()
    {
        List<string> newList = new List<string>();
        DirectoryInfo diInfo = new DirectoryInfo(Managers.saveDataPath);

        foreach(FileInfo file in diInfo.GetFiles("*.json"))
            newList.Add(file.Name);

        return newList;
    }


    public void SaveData(string _name, string _date)
    {
        SaveData saveData = new SaveData();
        saveData.Save();
        if(!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        string saveJson = JsonConvert.SerializeObject(saveData);
        Debug.Log("string : " + saveJson);
        
        string saveFileName = _date + " " + _name;
        string saveFilePath = savePath + saveFileName + ".json";
        File.WriteAllText(saveFilePath, saveJson);
        Debug.Log("Save Success : " + saveFilePath + " => " + saveFileName);
    }

    public SaveData LoadData(string saveFileName)
    {
        string saveFilePath = savePath + saveFileName;
        //Debug.Log("LoadData Path : " + saveFilePath);
        if(!File.Exists(saveFilePath))
        {
            //Debug.Log("No Save File");
            return null;
        }

        string saveFile = File.ReadAllText(saveFilePath);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(saveFile);
        return saveData;
    }

    public void Clear()
    {
        ItemDB.Clear();
        FoodDict.Clear();
        WeaponDict.Clear();
        CureDict.Clear();
        ToolDict.Clear();
    }
}


[Serializable]
public class SaveData
{
    //현재 플래그
    public Dictionary<Define.FlagType, int> curflags = new Dictionary<Define.FlagType, int>();
    public string characterName;

    //현재 시간 (게임이 시작되고 흐른 총 시간)
    public float totalGameTime = 0f;    //게임매니저

#region 현재 플레이어 스탯
    //현재 위치
    public float posX;
    public float posY;
    public float posZ;

    public int curHp;            // 체력
    public int curMaxHp;
    public float hunger = 0;      // 허기
    public float thirst = 0;      // 목마름
    public float sleepiness = 0;  // 졸음
    public float[] curTiredSteps = new float[3];
    public float curWeight = 0;    // 현재 무게
    public float[] curOverWeights = new float[3];
    public float curViewAngle;    
    public float curDetectRange;
    public float injuryPoints = 0; // 부상 입은 정도.  3 이상이면 부상 플래그 발동
    public float sickPoints = 0; // 아픈 정도. 

    // 팩터 변수 
    public float curBasicSpeed;
    public float curRotateSpeed;
    public float curHealSpeed;
    public float curActionSpeed;
    public float curAttack;
    public float curInjuryRate = 70;          // 부상 확률(%)
    public float curSickRate = 50;            // 병에 걸릴 확률(%)
    public float[] curExhaustedSteps = new float[3];
    public bool isInfected = false;
    
#endregion

    //현재 플레이어 아이템리스트
    public List<int> playerItemIdx = new List<int>();
    public int curMagazine;
    public int curAmmoCount;
    public int? equipedItem = null;

    //현재 스토리지 아이템 리스트
    public Dictionary<int, List<int>> storageItems = new Dictionary<int, List<int>>();

    //현재 좀비 킬수
    public int killcount;

    public void Save()
    {
        characterName = Managers.Game.player.name;
        totalGameTime = Managers.Game.totalGameTime;
        posX = Managers.Game.player.transform.position.x;
        posY = Managers.Game.player.transform.position.y;
        posZ = Managers.Game.player.transform.position.z;

        curflags = Managers.Game.flag.SaveCurFlags();
        
        List<Item> playerItems = Managers.Inven.playerItems;
        curMagazine = Managers.Inven.curMagazine;
        curAmmoCount = Managers.Inven.AmmoCount;

        killcount = Managers.Game.killcount;

        foreach(Item item in playerItems)
        {
            playerItemIdx.Add(item.idx);
            if(item is Weapon)
            {
                Weapon temp = (Weapon)item;
                if(temp.isEquiped)
                    equipedItem = temp.idx;
            }
        }

        for(int j = 0; j < Managers.Game.storages.Count; j ++)
        {
            if(Managers.Game.storages[j].isOpened)
            {
                List<int> templist = new List<int>();
                foreach(Item item in Managers.Game.storages[j].items)
                    templist.Add(item.idx);
                
                storageItems.Add(j, templist);
            }
        }
        
        SavePlayerStat();
    }

    void SavePlayerStat()
    {
        curHp = Managers.Game.player.stat.HP;            
        curMaxHp = Managers.Game.player.stat.curMaxHp;
        hunger = Managers.Game.player.stat.Hunger;      // 허기
        thirst = Managers.Game.player.stat.Thirst;      // 목마름
        sleepiness = Managers.Game.player.stat.Sleepiness;  // 졸음
        for(int i = 0; i < curTiredSteps.Length; i ++)
        {
            curTiredSteps[i] = Managers.Game.player.stat.curTiredSteps[i];
            curOverWeights[i] = Managers.Game.player.stat.curOverWeights[i];
            curExhaustedSteps[i] = Managers.Game.player.stat.curExhaustedSteps[i]; 
        }

        curWeight = Managers.Game.player.stat.CurWeight;    // 현재 무게
        curViewAngle = Managers.Game.player.stat.curViewAngle;    
        curDetectRange = Managers.Game.player.stat.curDetectRange;
        injuryPoints = Managers.Game.player.stat.InjuryPoints; // 부상 입은 정도.  3 이상이면 부상 플래그 발동
        sickPoints = Managers.Game.player.stat.SickPoints; // 아픈 정도. 

        // 팩터 변수 
        curBasicSpeed = Managers.Game.player.stat.curBasicSpeed;
        curRotateSpeed = Managers.Game.player.stat.curRotateSpeed;
        curActionSpeed = Managers.Game.player.stat.CurActionSpeed;
        curAttack = Managers.Game.player.stat.curAttack;
        curInjuryRate = Managers.Game.player.stat.CurInjuryRate;          // 부상 확률(%)
        curSickRate = Managers.Game.player.stat.CurSickRate;            // 병에 걸릴 확률(%)
        isInfected = Managers.Game.player.stat.isInfected;
    }

}