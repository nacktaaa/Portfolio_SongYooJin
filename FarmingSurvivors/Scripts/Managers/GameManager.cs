using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StageMap;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이 씬을 관리하는 매니저
/// 한 챕터의 흐름 관리
/// </summary>
public class GameManager : SingletoneBehaviour<GameManager>
{
    public Character curCharacter1 = Character.Pyotr;
    public string curCharacter;
    public PlayerController player;
    public MapConfig mapConfig;
    public bool NowPlaying = false;
    public bool isGameOver = false;
    public bool isClear = false;
    public bool useRevival = false;
    public bool isEnterStage = false;
    public bool isDialog = false;
    public bool isContinue = false;

    public WaitForSeconds transitionDelay = new WaitForSeconds(1.5f);
    public StageUICanvas stageUI;
    public BattleStage battleStage;
    public BossStage bossStage;
    public GoldStage goldStage;
    public StageType curStage;
    public int curFloor = 1;
    public string curChapter;
    public MapManager mapManager;
    DialogSystem dialogSystem;
    Coroutine stageClearCoroutine = null;

    public void Init()
    {
        NowPlaying = false;
        isGameOver = false;
        isClear = false;
        useRevival = false;
        isEnterStage = false;
        isDialog = false;
        curFloor = 1;

        mapManager = GameObject.FindObjectOfType<MapManager>();

        if(isContinue)
        {
            if(PlayerPrefs.HasKey("Map"))
            {
                curCharacter = mapManager.CurrentMap.characterName;
                mapConfig = DataManager.Instance.ChapterConfigs.Single(c => c.name == mapManager.CurrentMap.configname);
                curFloor = mapManager.CurrentMap.path.Count + 1;
            }
        }
        // 맵 정보 전달
        mapManager.config = mapConfig;
        curChapter = mapConfig.name.Substring(mapConfig.name.Length - 1, 1);


        // 플레이어 프리팹 생성 
        GameObject playerPrefab = DataManager.Instance.playerPrefabs.Single(p => p.name == curCharacter);
        if (playerPrefab != null)
        {
            player = GameObject.Instantiate(playerPrefab).GetComponent<PlayerController>();
            player.gameObject.SetActive(false);

            if(isContinue)
            {
                // 스킬 로드 
                var mySkills = SaveManager.Instance.LoadSkills();
                if(mySkills != null)
                {
                    Weapon.Weapon curSkill = GameManager.Instance.player.currentWeapons[0];
                    GameObject.Destroy(curSkill.gameObject);
                    GameManager.Instance.player.currentWeapons.Clear();
                    foreach(var s in mySkills)
                    {
                        Weapon.Weapon newWeapon = DataManager.Instance.weaponPrefabs.Single(w =>
                            w.GetComponent<Weapon.Weapon>().stat.id == s.Key).GetComponent<Weapon.Weapon>();
                        LevelUpManager.Instance.CreateWeapon(newWeapon, s.Value);
                    }
                }
            }
        }

        stageUI = GameObject.FindObjectOfType<StageUICanvas>();

        // 스테이지 스크립트 들고 있기
        BattleStage[] battles = GameObject.FindObjectsOfType<BattleStage>();
        foreach (var b in battles)
        {
            if (b.stageType == StageType.Normal)
                battleStage = b;
            else if (b.stageType == StageType.Boss)
                bossStage = b as BossStage;
        }
        goldStage = GameObject.FindObjectOfType<GoldStage>();

        GameObject grid = GameObject.Instantiate(mapConfig.mapGrid, battleStage.transform);
        battleStage.gameObject.SetActive(false);
        bossStage.gameObject.SetActive(false);
        goldStage.gameObject.SetActive(false);

        SoundManager.Instance.PlayBGM(BGMSounds.Ready);

        dialogSystem = GameObject.FindObjectOfType<DialogSystem>();
        if(curChapter == "1" && curFloor == 1)
            dialogSystem.StartDialog("0", curCharacter + curChapter + "0");   // 인트로는 예외. 0
        else if(curChapter == "2" && curFloor == 1)
            dialogSystem.StartDialog("0", curCharacter + curChapter + "0");
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        BGMSounds bgm = BGMSounds.Ready; 
        if(NowPlaying)
            bgm = GameManager.Instance.curStage == StageMap.StageType.Normal ? BGMSounds.Normal : BGMSounds.Boss;

        SoundManager.Instance.PlayBGM(bgm);
    }

    public void OnLevelUp()
    {
        if (player.isDead)
            return;

        Pause();
        stageUI.basicStageUI.levelText.text = (player.status.CurLevel + 1).ToString();
        LevelUpManager.Instance.LevelUp();
    }

    public void StageClear()
    {
        stageClearCoroutine = CoroutineHelper.StartCoroutine(StageClearCoroutine());
    }

    public void GameOver()
    {
        PlayerPrefs.DeleteKey("Map");
        CoroutineHelper.StartCoroutine(GameOverCoroutine());
    }

    IEnumerator StageClearCoroutine()
    {
        if (!isClear)
        {
            isClear = true;
            isEnterStage = false;
            yield return transitionDelay;
            SoundManager.Instance.PlayTriggerSound(TriggerSound.Clear);
            SoundManager.Instance.BGMplayer.Pause();

            if(CheckDialogFloor())
                dialogSystem.StartDialog(curChapter, GetSceneID());

            while(isDialog)
            {
                yield return null;
            }

            Pause();

            if (curStage == StageType.Boss)
            {
                var r = FarmManager.Instance.RewardRandomSeedbyRate(int.Parse(curChapter), curStage);
                stageUI.chapterClearUI.SetRewardSlot(r.seedType, r.SeedGrade);
                stageUI.chapterClearUI.SetNextChapterButton();
                stageUI.ShowChapterClearPanel();
            }
            else
            {
                var r = FarmManager.Instance.RewardRandomSeedbyRate(int.Parse(curChapter), curStage);
                stageUI.stageClearPanel.GetComponent<StageClearUI>().SetRewardSlot(r.seedType, r.SeedGrade);
                stageUI.ShowStageClearPanel();
            }

            if (curStage == StageType.Gold)
            {
                GoldManager.Instance.AddGold(goldStage.EarnedGold);
            }
        }
    }

    IEnumerator GameOverCoroutine()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            SoundManager.Instance.PlayTriggerSound(TriggerSound.GameOver);
            SoundManager.Instance.BGMplayer.Pause();
            yield return transitionDelay;
            Sprite dieSprite = player.GetComponent<SpriteRenderer>().sprite;
            stageUI.ShowGameOverPanel(dieSprite);
            Pause();
        }
    }

    public void ExitStage(StageType stageType)
    {
        Resume();
        NowPlaying = false;
        
        FarmManager.Instance.growthPoint += curFloor * mapConfig.GrowthPoint;

        curFloor++;
        if (curFloor <= 2)
            stageUI.generateButton.gameObject.SetActive(false);

        mapManager.SaveMap();
        SaveManager.Instance.SaveSkills();
        mapManager.ShowMap();
        
        SoundManager.Instance.TriggerPlayer.Stop();
        SoundManager.Instance.PlayBGM(BGMSounds.Ready);

        player.transform.SetParent(null);
        player.transform.position = Vector2.zero;
        player.gameObject.SetActive(false);

        switch (stageType)
        {
            case StageType.Normal:
            case StageType.Gold:
            case StageType.Boss:
            {
                PoolManager.Instance.DisableAllPool();

                foreach (var key in ProjectileManager.Instance.pool.Keys)
                {
                    foreach (var item in ProjectileManager.Instance.pool[key])
                    {
                        if(item.activeSelf)
                            item.SetActive(false);
                    }
                }
            }
                break;
        }

        stageUI.HideAllPanel();
        ClearStageData();
    }

    public void EnterStage(StageType stageType)
    {
        if (stageClearCoroutine != null)
            CoroutineHelper.StopCoroutine(stageClearCoroutine);

        NowPlaying = true;
        isClear = false;
        isGameOver = false;
        useRevival = false;
        isEnterStage = true;
        curStage = stageType;

        if(CheckDialogFloor())
            dialogSystem.StartDialog(curChapter, GetSceneID());

        mapManager.HideMap();

        switch (stageType)
        {
            case StageType.Normal:
                {
                    battleStage.player = player;

                    stageUI.ShowBattleStageUI();
                    player.gameObject.SetActive(true);
                    battleStage.Init();
                    battleStage.gameObject.SetActive(true);

                    SoundManager.Instance.PlayBGM(BGMSounds.Normal);
                }
                break;
            case StageType.Gold:
                {
                    goldStage.player = player;
                    player.transform.SetParent(goldStage.transform);

                    player.gameObject.SetActive(true);
                    goldStage.gameObject.SetActive(true);
                    goldStage.Init();
                    stageUI.ShowGoldStageUI();

                    SoundManager.Instance.PlayBGM(BGMSounds.Ready);
                }
                break;
            case StageType.Store:
                {
                    stageUI.storeStageUI.Init();
                    stageUI.ShowStoreStageUI();

                    SoundManager.Instance.PlayBGM(BGMSounds.Ready);
                }
                break;
            case StageType.Rest:
                {
                    stageUI.ShowRestStageUI();
                    stageUI.restStageUI.Init();
                    SoundManager.Instance.PlayTriggerSound(TriggerSound.Heal);
                    SoundManager.Instance.BGMplayer.Pause();
                }
                break;
            case StageType.Boss:
                {
                    bossStage.player = player;
                    stageUI.ShowBossStageUI();
                    player.gameObject.SetActive(true);
                    bossStage.gameObject.SetActive(true);
                    bossStage.Init();
                    SoundManager.Instance.PlayBGM(BGMSounds.Boss);
                }
                break;
        }
    }

    public void ExitChapter()
    {
        NowPlaying = false;
        Resume();
        curFloor = 0;
        ProjectileManager.Instance.Clear();
        SoundManager.Instance.EffectPlayer.Stop();
        SoundManager.Instance.TriggerPlayer.Stop();
        SceneManager.LoadScene(0);
    }

    private void ClearStageData()
    {
        switch (curStage)
        {
            case StageType.Normal:
                {
                    battleStage.Clear();
                    battleStage.gameObject.SetActive(false);
                }
                break;
            case StageType.Gold:
                {
                    goldStage.Clear();
                    goldStage.gameObject.SetActive(false);
                }
                break;
            case StageType.Boss:
                {
                    battleStage.Clear();
                    bossStage.gameObject.SetActive(false);
                }
                break;
        }

    }

    public string GetStageID(StageType type)
    {
        string stage = type.ToString();
        string newID = stage + curChapter;
        if (curFloor > 15)
            newID = newID + "6";
        else if (curFloor > 12)
            newID = newID + "5";
        else if (curFloor > 9)
            newID = newID + "4";
        else if (curFloor > 6)
            newID = newID + "3";
        else if (curFloor > 3)
            newID = newID + "2";
        else
            newID = newID + "1";

        return newID;
    }

    private string GetSceneID()
    {
        string sceneID = "";
        switch(curChapter)
        {
            case "1":
            case "2":
                sceneID = curCharacter + curFloor.ToString();
                break;
            case "3":
                sceneID = "All" + curFloor.ToString();
                break;
            default :
                break;
        }
        sceneID = isEnterStage ? sceneID + "1" : sceneID + "2";
        return sceneID;
    }

    public void LoadNextChapter()
    {
        int nextIdx = int.Parse(curChapter) + 1 >= 3 ? 3 : int.Parse(curChapter) + 1;
        mapConfig = DataManager.Instance.ChapterConfigs[nextIdx - 1];
        mapManager.config = mapConfig;
        mapManager.GenerateNewMap();
        mapManager.SaveMap();
        GameManager.Instance.player.currentWeapons.Clear();
        SaveManager.Instance.SaveSkills();
        curChapter = nextIdx.ToString();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextCharacter()
    {
        int c = (int)GameManager.Instance.curCharacter1;
        c = (c + 1) % Enum.GetValues(typeof(Character)).Length;
        GameManager.Instance.curCharacter1 = (Character)c;
    }

    public void PreviousCharacter()
    {
        int c = (int)GameManager.Instance.curCharacter1;
        c = (c - 1 + Enum.GetValues(typeof(Character)).Length) % Enum.GetValues(typeof(Character)).Length;
        GameManager.Instance.curCharacter1 = (Character)c;
    }

    private bool CheckDialogFloor()
    {
        switch(curFloor)
        {
            case 1 :
            case 7 :
            case 12 :
            case 17 :
                return true;
            default :
                return false;
        }
    }
    public void ContinueStageWithGold(){
        if(GoldManager.Instance.CurrentGold < 800)
            return;
        
        GoldManager.Instance.SubtractGold(800);
        ContinueStage();
    }
    public void ContinueStage()
    {
        useRevival = true;
        Resume();
        isGameOver = false;
        
        // 플레이어 부활처리 
        player.Revival();
        // 사운드 처리
        SoundManager.Instance.TriggerPlayer.Stop();
        SoundManager.Instance.BGMplayer.Play();
        if(curStage == StageType.Boss)
            bossStage.timeCount = bossStage.isBossSpawn ? bossStage.bossSpawnTime : 0;

        // 게임오버판넬 닫기
        stageUI.revivalPopup.gameObject.SetActive(false);
        stageUI.gameOverPanel.SetActive(false);
    }
}
