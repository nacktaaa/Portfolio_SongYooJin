
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using DG.Tweening;
using StageMap;
using System.Linq;

public class BattleStage : MonoBehaviour
{
    public StageType stageType = StageType.Normal;
    public BattleStageUI battleStageUI;    
    public PlayerController player;
    public Dictionary<int, List<MonsterStatus>> respawnMonsterDic = new Dictionary<int, List<MonsterStatus>>();
    public int totalEnemyAmount = 30;
    public int respawnAmount = 10;
    protected List<float> phaseValues = new List<float>();
    public float enemyRespawnTime;
    public Tilemap[] tilemaps;
    
    protected float respawnCount = 0;
    protected int curPhase = 0;
    [SerializeField]
    protected int enemyCount = 0;
    int rewardGold = 0;
    int killCount;
    public int KillCount
    {
        get { return killCount;}
        set
        {
            killCount = value;
            if(stageType == StageType.Normal)
            {
                battleStageUI.SetMonsterCountText(totalEnemyAmount - killCount);
                
                if(killCount >= totalEnemyAmount)
                {
                    killCount = totalEnemyAmount;
                    battleStageUI.SetMonsterCountText(totalEnemyAmount - killCount);
                    GameManager.Instance.StageClear();
                }
            }
        }
    }

    private void Start()
    {
        if (player.currentWeapons.Count == 0 && stageType == StageType.Normal)
        {
            if(GameManager.Instance.isDialog)
            {
                StopAllCoroutines();
                StartCoroutine(WaitLevelUpCoroutine());
            }
            else
                GameManager.Instance.OnLevelUp();

        }
    }

    IEnumerator WaitLevelUpCoroutine()
    {
        while(GameManager.Instance.isDialog)
        {
            yield return null;
        }
        GameManager.Instance.OnLevelUp();
    }

    public virtual void Init()
    {
        enemyCount = 0;
        respawnCount = 0;
        curPhase = 0;
        KillCount = 0;
        
        player.transform.SetParent(this.transform);
        Camera.main.GetComponent<FollowCam>().Init(player.transform, stageType);
        
        if(stageType == StageType.Normal)
        {
            tilemaps = GetComponentsInChildren<Tilemap>();
            foreach(var tile in tilemaps)
            {
                Reposition rePos = tile.GetComponent<Reposition>();
                tile.transform.position = rePos.startPos;
                
            }
        }
        
        // 몬스터 풀에서 소환할 몬스터 리스트와 페이즈 정보 받아오기
        string id = GameManager.Instance.GetStageID(stageType);
        // 스테이지 ID로 스테이지 정보 찾기
        if(DataManager.Instance.phaseStageDic.TryGetValue(id, out PhaseStage phaseStage))
        {
            totalEnemyAmount = phaseStage.SpawnMax;
            battleStageUI.SetMonsterCountText(totalEnemyAmount);
            rewardGold = phaseStage.StageGold;

            int phaseIdx = 0;
            // 각 페이즈에 등장할 몬스터풀ID를 순회 하고..
            foreach(var poolID in phaseStage.phasePools)
            {
                // 해당하는 몬스터풀ID를 몬스터풀 딕셔너리에서 찾아 몬스터 이름들을 가져온다
                if (DataManager.Instance.monsterPoolDic.TryGetValue(poolID, out List<string> monsterNames))
                {
                    List<MonsterStatus> newMonsterList = new List<MonsterStatus>();
                    // 몬스터 이름으로 해당하는 MonsterStatus를 찾아 몬스터 리스트에 저장
                    foreach(var name in monsterNames)
                        newMonsterList.Add(DataManager.Instance.minorMonsterStatuses.Single( m => m.name == name));

                    // 저장한 몬스터 리스트를 딕셔너리로 저장 (Key : int 페이즈 순서 Value : List<MonsterStatus>)
                    respawnMonsterDic.TryAdd(phaseIdx, newMonsterList);             
                    phaseIdx ++;
                }
            }

            for (int i = 0; i < phaseStage.phasePers.Count; i++)
            {
                // 각 페이즈 값(float)을 리스트로 저장
                phaseValues.Add(float.Parse(phaseStage.phasePers[i]));
            }
        }
    }

    public void Clear()
    {
        respawnMonsterDic.Clear();
        phaseValues.Clear();
        respawnCount = 0;
        enemyCount = 0;
    }

    protected virtual void Update()
    {
        if(!player.isDead)
        {
            respawnCount += Time.deltaTime;
            if(respawnCount > enemyRespawnTime)
            {
                RespawnEnemy();
                respawnCount = 0;
            }
        }
    }

    protected virtual void RespawnEnemy()
    {
        // 대화씬 진행 중이라면 리턴
        if(GameManager.Instance.isDialog)
            return;

        if(GameManager.Instance.isGameOver || GameManager.Instance.isClear )
            return;

        // 한번에 리스폰할 수만큼 진행 
        for(int i = 0; i < respawnAmount; i++)
        {
            // 지금까지 스폰된 몬스터 수가 스폰할 총 몬스터 수 보다 적다면 스폰
            if(enemyCount < totalEnemyAmount)
            {
                // 현재 페이즈에 해당하는 몬스터를 오브젝트풀에서 가져옴
                GameObject enemy = GetMonster();
                if(enemy != null)
                {
                    // 페이즈에 따른 몬스터 리스트에서 랜덤으로 MonsterStatus를 가져와 몬스터 초기화
                    if(respawnMonsterDic.TryGetValue(curPhase, out List<MonsterStatus> monsterList))
                    {
                        int rnd = Random.Range(0, monsterList.Count);
                        enemy.GetComponent<MinorMonster>().Init(monsterList[rnd], player.transform);
                    }
                    else // TryGetValue에 실패했다면 기본 몬스터정보로 초기화 
                        enemy.GetComponent<MinorMonster>().Init(DataManager.Instance.minorMonsterStatuses[0], player.transform);
        
                    enemy.transform.position = player.GetRespawnPoint();
                    enemy.SetActive(true);
                    enemyCount++;

                    CheckCurPhase();
                }
            }
        }
    }

    protected void CheckCurPhase()
    {
        if(phaseValues.Count != 0)
        {
            // 현재까지 생성된 몬스터 수로 현재 페이즈를 설정
            int count = enemyCount;
            if(curPhase != 0)
            {
                for (int j = 0; j < curPhase; j ++)
                    count -= (int)(totalEnemyAmount * phaseValues[j]);
            }

            if(count >= totalEnemyAmount * phaseValues[curPhase])
                curPhase = (curPhase + 1) >= phaseValues.Count ? curPhase : curPhase + 1;
        }
    }


    // 오브젝트풀에서 몬스터를 가져오는 메서드
    protected GameObject GetMonster()
    {
        GameObject go = null;
        go = PoolManager.Instance.Pop(DataManager.Instance.minorMonsterPrefab).gameObject;
    
        return go;
    }
    

}
