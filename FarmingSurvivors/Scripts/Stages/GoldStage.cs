
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Unity.VisualScripting;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using StageMap;
using DG.Tweening;
using Unity.Mathematics;
using System.Linq;

public class GoldStage : MonoBehaviour
{
    public Action<int> GetGoldEventHandler;
    public StageType stageType = StageType.Gold;
    public GoldStageUI goldStageUI;
    public PlayerController player;
    public GameObject respawnEffect;
    public Tilemap[] tilemaps;
    public int killCount = 0;
    int totalEnemyAmount = 100;

    [SerializeField]
    float playTime;
    [SerializeField]
    int initMonsterAmount;
    [SerializeField]
    float respawnCoolTime = 10f;
    [SerializeField]
    int respawnAmount = 10;
    List<MonsterStatus> monsters = new List<MonsterStatus>();
    List<GameObject> effectPool = new List<GameObject>();
    WaitForSeconds respawnDelay;
    int earnedGold = 0;
    float respawnCount = 0;
    float timeCount = 0;

    public int EarnedGold
    {
        get { return earnedGold;}
        set 
        {
            earnedGold = value;
            GetGoldEventHandler?.Invoke(earnedGold);
        }
    }

    public void Init()
    {
        timeCount = 0;
        respawnCount = 0;
        earnedGold = 0;
        respawnDelay = new WaitForSeconds(1f/respawnAmount);
        goldStageUI.SetGoldCount(earnedGold);
        Camera.main.GetComponent<FollowCam>().Init(player.transform, stageType);
        goldStageUI.gameObject.SetActive(true);

        foreach(var tile in tilemaps)
        {
            Reposition rePos = tile.GetComponent<Reposition>();
            tile.transform.position = rePos.startPos;
        }

        // 아이디부여
        string id = GameManager.Instance.GetStageID(stageType);
        // 몬스터 풀에서 몬스터 정보 받아오기
        if(DataManager.Instance.randomStageDic.TryGetValue(id, out RandomStage randomStage))
        {
            totalEnemyAmount = randomStage.SpawnMax;

            if(DataManager.Instance.monsterPoolDic.TryGetValue(randomStage.SpawnPool, out List<string> monsterNames))
            {
                foreach(var name in monsterNames)
                    monsters.Add(DataManager.Instance.minorMonsterStatuses.Where( m => m.name == name).Last());
            }
        }

        for(int i = 0; i < initMonsterAmount; i ++)
        {
            float sign = i < initMonsterAmount/2 ? 1 : -1;
            Vector3 spawnPos = player.transform.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 2f * sign, 0);
            GameObject go = PoolManager.Instance.Pop(DataManager.Instance.runAwayMonsterPrefab).gameObject;
            go.transform.position = spawnPos;
            go.GetComponent<Monster>().Init(monsters[UnityEngine.Random.Range(0, monsters.Count)], player.transform);
        }
    }


    private void Update()
    {
        if(!player.isDead)
        {
            timeCount += Time.deltaTime;
            if(timeCount >= playTime)
            {
                timeCount = playTime;
                GameManager.Instance.StageClear();
            }
            goldStageUI.SetTimeCount(timeCount);

            respawnCount += Time.deltaTime;
            if(respawnCount >= respawnCoolTime)
            {
                StartCoroutine(RespawnMonsterCoroutine());
                respawnCount = 0;
            }
        }
    }

    IEnumerator RespawnMonsterCoroutine()
    {
        List<Vector3> spawnPoses = new List<Vector3>();
        List<GameObject> tempEffects = new List<GameObject>();
        
        for(int i = 0; i < respawnAmount; i ++)
        {
            Vector3 newPos = new Vector3(UnityEngine.Random.Range(-3f, 3f), player.transform.position.y + UnityEngine.Random.Range(-5f, 5f), 0);
            spawnPoses.Add(newPos);
        }    

        foreach(var pos in spawnPoses)
        {
            GameObject effect = GetEffect();
            effect.transform.position = pos;
            effect.SetActive(true);
            tempEffects.Add(effect);
            yield return respawnDelay;
        }

        for (int i = 0; i < respawnAmount; i ++)
        {
            GameObject monster = GetMonster();
            monster.transform.position = spawnPoses[i];
            monster.SetActive(true);
            tempEffects[i].SetActive(false);
            yield return respawnDelay;
        }    
    }

    // 재화방은 페이즈 랜덤 
    private GameObject GetMonster()
    {
        GameObject go = null;
        go = PoolManager.Instance.Pop(DataManager.Instance.runAwayMonsterPrefab).gameObject;
        
        int rnd = UnityEngine.Random.Range(0, monsters.Count);
        go.GetComponent<RunAwayMonster>().Init(monsters[rnd], player.transform);
        
        return go;
    }

    private GameObject GetEffect()
    {
        GameObject go = null;
        foreach(var effect in effectPool)
        {
            if(!effect.activeSelf)
            {
                go = effect;
                break;
            }
        }
        if(go == null)
        {
            go = Instantiate(respawnEffect, transform);
            go.SetActive(false);
            effectPool.Add(go);
        }
        return go;
    }
    public void Clear()
    {
        foreach(var effect in effectPool)
            Destroy(effect);
        effectPool.Clear();
        monsters.Clear();
    }
}
