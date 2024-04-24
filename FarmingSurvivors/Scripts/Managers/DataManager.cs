using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StageMap;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Equipment;

/// <summary>
/// 게임에 필요한 모든 데이터를 들고 있는 클래스
/// </summary>
public class DataManager : SingletoneBehaviour<DataManager>
{
    public MapConfig[] ChapterConfigs;
    public GameObject[] playerPrefabs;
    public GameObject[] weaponPrefabs;
    public GameObject minorMonsterPrefab;
    public GameObject runAwayMonsterPrefab;
    public GameObject invenSlot;
    public MonsterStatus[] minorMonsterStatuses;
    public Equipment.PartsSetting[] equipicons;
    public CropSetting[] cropSettings;
    public CropGradeSetting[] cropGradeSettings;
    public Dictionary<int, GameObject> bossDict = new Dictionary<int, GameObject>();
    private StageDB stageDB;
    private DialogDB dialogDB;
    public Dictionary<string, List<string>> monsterPoolDic = new Dictionary<string, List<string>>();
    public Dictionary<string, PhaseStage> phaseStageDic = new Dictionary<string, PhaseStage>();
    public Dictionary<string, RandomStage> randomStageDic = new Dictionary<string, RandomStage>();
    public Dictionary<string, List<DialogContents>> dialogDict = new Dictionary<string, List<DialogContents>>();

    public void Init()
    {
        ChapterConfigs = Resources.LoadAll<MapConfig>("SO/ChapterConfig");
        playerPrefabs = Resources.LoadAll<GameObject>("Prefabs/Characters");
        weaponPrefabs = Resources.LoadAll<GameObject>("Prefabs/Battle/Weapons");
        
        minorMonsterPrefab = Resources.Load<GameObject>("Prefabs/Monsters/MinorMonster");
        runAwayMonsterPrefab = Resources.Load<GameObject>("Prefabs/Monsters/RunAwayMonster");

        minorMonsterStatuses = Resources.LoadAll<MonsterStatus>("SO/Monsters");
        invenSlot = Resources.Load<GameObject>("Prefabs/UI/InvenSlot");
        equipicons = Resources.LoadAll<PartsSetting>("SO/Equipment/PartsSettings");
        cropSettings = Resources.LoadAll<CropSetting>("SO/Farm/Crops");
        cropGradeSettings = Resources.LoadAll<CropGradeSetting>("SO/Farm/Grade");
        
        dialogDB = Resources.Load<DialogDB>("Data/DialogDB");
        stageDB = Resources.Load<StageDB>("Data/StageDB");
        dialogDict = dialogDB.MakeDialogDict();
        monsterPoolDic = stageDB.MakeMonsterPoolDic();
        phaseStageDic = stageDB.MakePhaseStageDic();
        randomStageDic = stageDB.MakeRandomStageDic();

        GameObject[] ch1Monsters = Resources.LoadAll<GameObject>("Prefabs/Monsters/Chapter1");
        GameObject[] ch2Monsters = Resources.LoadAll<GameObject>("Prefabs/Monsters/Chapter2");
        GameObject[] ch3Monsters = Resources.LoadAll<GameObject>("Prefabs/Monsters/Chapter3");
        
        int chapter = 1;
        foreach(var m in ch1Monsters)
        {
            if(m.TryGetComponent<BossMonster>(out BossMonster boss))
            {
                bossDict.Add(chapter, m);
                break;
            }
        }
        chapter++;
        foreach(var m in ch2Monsters)
        {
            if(m.TryGetComponent<BossMonster>(out BossMonster boss))
            {
                bossDict.Add(chapter, m);
                break;
            }
        }
        chapter++;
        foreach(var m in ch3Monsters)
        {
            if(m.TryGetComponent<BossMonster>(out BossMonster boss))
            {
                bossDict.Add(chapter, m);
                break;
            }
        }

    }

    
	
}

