using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossStage : BattleStage
{
    public BossStageUI bossStageUI;
    public GameObject bossPrefab;
    public GameObject spawnEffect;
    public float bossSpawnTime = 60f;
    public float playTime = 120f;
    public float respawnDelay = 10f;
    public Transform[] respawnPoints;
    private BossMonster bossMonster;
    public float timeCount = 0;
    private float delayCount = 0;
    bool DoRespawn = true;
    public bool isBossSpawn = false;

    public override void Init()
    {
        base.Init();
        isBossSpawn = false;
        DoRespawn = true;
        timeCount = 0;
        delayCount = 0;
        bossStageUI.transform.GetChild(0).gameObject.SetActive(false);
        bossPrefab = DataManager.Instance.bossDict[int.Parse(GameManager.Instance.curChapter)];
    }

    protected override void Update()
    {
        if(!player.isDead || !GameManager.Instance.isGameOver)
        {
            timeCount += Time.deltaTime;
            if(timeCount >= playTime)
            {
                timeCount = playTime;
                GameManager.Instance.GameOver();
            }
            
            bossStageUI.SetTimeCount(playTime - timeCount);

            if(timeCount >= bossSpawnTime)
            {
                StartCoroutine(SpawnBoss());
            }

            if(DoRespawn)
            {
                respawnCount += Time.deltaTime;
                if(respawnCount > enemyRespawnTime)
                {
                    RespawnEnemy();
                    respawnCount = 0;
                }
            }
            else if(!DoRespawn)
            {
                delayCount += Time.deltaTime;
                if(delayCount > respawnDelay)
                {
                    DoRespawn = true;
                    delayCount = 0;
                    respawnCount = 0;
                }
            }
        }   
    }

    protected override void RespawnEnemy()
    {
        if(GameManager.Instance.isGameOver || GameManager.Instance.isClear)
            return;

        if(enemyCount < totalEnemyAmount)
        {
            GameObject enemy = GetMonster();
            if(enemy != null)
            {
                enemy.transform.position = Util.GetRandom.PickRandom(respawnPoints).position;
                enemy.SetActive(true);
                enemyCount++;

                if(phaseValues.Count != 0)
                {
                    int count = enemyCount;
                    if(curPhase != 0)
                    {
                        for (int i = 0; i < curPhase; i ++)
                            count -= (int)(totalEnemyAmount * phaseValues[i]);
                    }

                    if(count >= totalEnemyAmount * phaseValues[curPhase])
                    {
                        DoRespawn = false;
                        delayCount = 0;
                        curPhase = (curPhase + 1) >= phaseValues.Count ? curPhase : curPhase + 1;
                    }
                }
            }
        }
    }

    IEnumerator SpawnBoss()
    {
        if(!isBossSpawn)
        {
            isBossSpawn = true;
            GameObject effect = Instantiate(spawnEffect, Vector3.zero, Quaternion.identity);
            yield return null;
            GameObject go = PoolManager.Instance.Pop(bossPrefab).gameObject;
            go.transform.position = Vector3.zero;
            go.transform.DOShakePosition(0.5f, 0.2f, 3, 30); 
            bossMonster = go.GetComponent<BossMonster>();
            bossMonster.Init(bossMonster.GetComponent<BossMonster>().status, player.transform);

            bossMonster.BossDieHandler -= ChapterClear;
            bossMonster.BossDieHandler += ChapterClear;

            bossStageUI.SetBossHPSlider(bossMonster.GetHPbarValue());
            bossStageUI.bossNameText.text = bossMonster.status.monsterName;
            bossStageUI.transform.GetChild(0).gameObject.SetActive(true);

            Destroy(effect, 0.2f);
        }
    }
    
    // 챕터 클리어 코루틴 
    private void ChapterClear()
    {
        GameManager.Instance.StageClear();
    }

    private void OnDisable() {
        if(bossMonster != null)
            bossMonster.BossDieHandler -= ChapterClear;
    }
}
