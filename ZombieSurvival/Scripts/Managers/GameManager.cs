using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager 
{
    // 게임 내 시간
    // 현실 1시간 = 게임 내 24시간 
    public PlayerController player;
    public FlagControl flag;
    public List<Storage> storages = new List<Storage>();

    // 게임 시간 변수
    // 게임 속 하루는 3600초 => 게임 내 1시간 = 150초 / 1분 = 2.5초
    
    float gameDeltaTime = 0;
    //float dayTime = 3600f;
    public float totalGameTime = 0;
    public int killcount = 0;
    public string causeOfDeath;
    float durTime_Full = 120f;
    float countTime_Full = 0;
    public float[] durTime_Bleeding = new float[] {0, 0};
    float countTime_Bleeding = 0;
    float countTime_injury = 0;
    float delayTime_injury = 5f;
    float countTime_HpDown = 0;
    float delayTime_HpDown = 10f;
    float countTime_HpDown2 = 0;
    float countTime_splash = 0;
    float countTime_exhausted = 25f; // 게임 내 시간 10분 = 25f
    float countTime_infection = 0f;
    float durTime_infection = 10f;

    public bool isSleep = false;
    public bool isGameOver = false;
    public bool IsRecognizeZombie = false;
    public bool isPause = false;
    Vector3 startPoint = new Vector3(-20, 0, -11);
    public CameraController virtualCam;
    List<Wall> transparentWalls = new List<Wall>();
    Vector3[] spawnPoints = { new Vector3 (45.5f, 0, -36.5f),
                              new Vector3 (50.5f, 0, 33.5f),
                              new Vector3 (-32f, 0, 60f),
                              new Vector3 (-32.6f, 0, 35.5f) };
    SaveData saveData;
    int day = 1;

    public void Init()
    {
        GameObject playerGo;
        AudioListener.pause = false;
        flag = new FlagControl();
        // 스토리지 초기화. 월드의 모든 스토리지를 찾아 list 에 저장한다.
        Storage[] arr = GameObject.FindObjectsByType<Storage>(FindObjectsSortMode.None);

        if(String.IsNullOrEmpty(Managers.saveDataName))
        {
            playerGo = GameObject.Instantiate(Managers.playerPrefab); 
            playerGo.name = Managers.playerPrefab.name;
            playerGo.transform.position = startPoint; 
            player = playerGo.GetComponent<PlayerController>(); 
            player.Init();
            totalGameTime = 0;
            killcount = 0;
        }
        else
        {
            //Debug.Log("세이브데이터가 있음");
            //세이브 데이터 불러오기
            saveData = Managers.Data.LoadData(Managers.saveDataName);
            totalGameTime = saveData.totalGameTime;
            //플레이어 프리팹 
            Managers.playerPrefab = Resources.Load<GameObject>($"Prefabs/FinalPlayer/{saveData.characterName}");

            playerGo = GameObject.Instantiate(Managers.playerPrefab);
            playerGo.transform.position = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
            player = playerGo.GetComponent<PlayerController>(); 
            player.Init();
            player.stat.LoadStats(saveData);
            Managers.Inven.curMagazine = saveData.curMagazine;
            Managers.Inven.AmmoCount = saveData.curAmmoCount;
            killcount = saveData.killcount;
            
            flag.LoadCurFlags(saveData.curflags);
            
            // 플레이어 아이템 정보 반영 
            bool onEquiped = false;
            foreach(int itemIdx in saveData.playerItemIdx)
            {
                foreach(KeyValuePair<int, Food> item in Managers.Data.FoodDict)
                {
                    if(item.Key == itemIdx)
                        Managers.Inven.playerItems.Add(item.Value);
                }
                foreach(KeyValuePair<int, Weapon> item in Managers.Data.WeaponDict)
                {
                    if(item.Key == itemIdx)
                    {
                        Weapon newWeapon = item.Value.CreateWeapon();
                        if(saveData.equipedItem == item.Key && !onEquiped)
                        {
                            newWeapon.isEquiped = true;
                            Managers.Inven.EquipItem(newWeapon);
                            onEquiped = true;
                        }
                        Managers.Inven.playerItems.Add(newWeapon);
                    }
                }
                foreach(KeyValuePair<int, Cure> item in Managers.Data.CureDict)
                {
                    if(item.Key == itemIdx)
                        Managers.Inven.playerItems.Add(item.Value);
                }
            }
        }

        virtualCam = GameObject.FindObjectOfType<CameraController>();
        virtualCam.Init();
        
        isSleep = false;
        isGameOver = false;
        isPause = false;
        
        int idx = 0;
        foreach(Storage storage in arr)
        {
            storage.idx = idx; 
            storages.Add(storage);
            idx ++;
        }

        // 세이브데이터가 있을 경우 반영하고, 그렇지 않다면 스토리지에 랜덤하게 아이템을 배분한다.
        foreach(Storage storage in storages)
        {
            if(saveData != null)
            {
                if(saveData.storageItems.ContainsKey(storage.idx))
                {
                    List<int> itemIdx = saveData.storageItems[storage.idx];
                    foreach(int num in itemIdx)
                    {
                        foreach(KeyValuePair<int, Food> item in Managers.Data.FoodDict)
                        {
                            if(item.Key == num)
                                storage.items.Add(Managers.Data.FoodDict[num]);
                        }
                        foreach(KeyValuePair<int, Weapon> item in Managers.Data.WeaponDict)
                        {
                            if(item.Key == num)
                            {
                                Weapon newWeapon = item.Value.CreateWeapon();
                                storage.items.Add(newWeapon);
                            }
                        }
                        foreach(KeyValuePair<int, Cure> item in Managers.Data.CureDict)
                        {
                            if(item.Key == num)
                                storage.items.Add(Managers.Data.CureDict[num]);
                        }
                    }
                }
                continue;
            }

            int maxSize = UnityEngine.Random.Range(0, 9);
            int minSize = UnityEngine.Random.Range(0, 5);
            maxSize = Mathf.Clamp((maxSize - minSize), 0, 9);
            
            for (int i = 0; i < maxSize; i++)
            {
                int rand2 = UnityEngine.Random.Range(0, 100);
                Item temp = null;
                int foods = Managers.Data.FoodDict.Count;
                int weapons = Managers.Data.WeaponDict.Count;
                int cures = Managers.Data.CureDict.Count;
                int tools = Managers.Data.ToolDict.Count;
                
                if(rand2 < 60)
                {
                    int rand3 = UnityEngine.Random.Range(0, foods);
                    temp = Managers.Data.FoodDict[rand3];
                    storage.items.Add((Food)temp);
                }
                else if(rand2 < 80)
                {
                    int rand3 = UnityEngine.Random.Range(foods, (foods + weapons));
                    temp = Managers.Data.WeaponDict[rand3];
                    Weapon newWeapon = Managers.Data.WeaponDict[rand3].CreateWeapon();
                    storage.items.Add(newWeapon);
                }
                else if(rand2 < 90)
                {
                    int rand3 = UnityEngine.Random.Range((foods +weapons), (foods + weapons + cures));
                    temp = Managers.Data.CureDict[rand3];
                    storage.items.Add((Cure)temp);
                }
                else
                {
                    int rand3 = UnityEngine.Random.Range((foods + weapons + cures), (foods + weapons + cures+ tools));
                    temp = Managers.Data.ToolDict[rand3];
                    storage.items.Add((Tools)temp);
                }

                float tempWeight = storage.curWeight;
                storage.curWeight += temp.weight;

                if(storage.curWeight > storage.maxWeight)
                {
                    storage.items.Remove(temp);
                    storage.curWeight = tempWeight;
                    break;
                }   
            }
        }
    
        countTime_Bleeding = 0;
        countTime_exhausted = 0;
        countTime_Full = 0;
        countTime_HpDown = 0;
        countTime_HpDown2 = 0;
        countTime_infection = 0;
        countTime_injury = 0;
        countTime_splash = 0;
    }

    public void Clear()
    {
        if(player != null)
            player.stat.Clear();
            
        transparentWalls.Clear();
        virtualCam = null;
        player = null;
        flag = null;
        storages.Clear();
    }

    // 게임 시간 측정
    public void OnUpdate()
    {
        if(isGameOver)
            return;

        if(isPause)
            return;

        FadeOutWall();

        gameDeltaTime = Time.deltaTime;
        totalGameTime += gameDeltaTime;

        player.stat.Thirst += gameDeltaTime;
        player.stat.Sleepiness += gameDeltaTime;

        if(totalGameTime >= 3600f * day)
        {
           // Debug.Log("totalGameTime : " + totalGameTime + " Day : " + day);
            for (int i = 0; i < 10; i++)
            {
                int randomPointIdx = UnityEngine.Random.Range(0, spawnPoints.Length);
                GameObject zombie = Managers.Pool.Pop(Define.PoolableType.Zombie);
                zombie.transform.position = spawnPoints[randomPointIdx];
            }
            day ++;
        }
        

        if(player.stat.isInfected)
        {
            countTime_infection += gameDeltaTime;
            if(countTime_infection > durTime_infection)
                flag.AddFlag(Define.FlagType.Zombie);
        }

        // 시간의 흐름에 따른 플래그 전환 관리 
        if (flag.HasFlag(Define.FlagType.Full))
        {
            countTime_Full += gameDeltaTime;
            if (countTime_Full >= durTime_Full)
            {
                flag.RemoveFlag(Define.FlagType.Full);
                player.stat.Hunger = 0;
                countTime_Full = 0;
            }
        }
        else
        {
            player.stat.Hunger += gameDeltaTime;
        }

        if(flag.HasFlag(Define.FlagType.Bleeding))
        {
            countTime_Bleeding += gameDeltaTime;
            if (flag.GetFlagStep(Define.FlagType.Bleeding) == 1)
            {
                if(countTime_Bleeding >= durTime_Bleeding[0])
                {
                    countTime_Bleeding = 0;
                    flag.RemoveFlag(Define.FlagType.Bleeding);
                }
            }
            else if(flag.GetFlagStep(Define.FlagType.Bleeding) == 2)
            {
                if(countTime_Bleeding >= durTime_Bleeding[1])
                {
                    countTime_Bleeding = 0;
                    flag.RemoveFlag(Define.FlagType.Bleeding);
                    flag.AddFlag(Define.FlagType.Sick);
                }
            }
        }
    
        if(flag.GetFlagStep(Define.FlagType.Overweight) == 2)
        {
            countTime_injury += gameDeltaTime;
            if(countTime_injury > delayTime_injury)
            {
                float rnd = UnityEngine.Random.Range(0f, 100f);
                if(rnd < player.stat.CurInjuryRate)
                    player.stat.InjuryPoints ++;
                
                countTime_injury = 0;
            }
        }

        if(flag.HasFlag(Define.FlagType.Injury))
        {
            countTime_HpDown += gameDeltaTime;
            if(countTime_HpDown > delayTime_HpDown)
            {
                player.stat.HP -= 2;
                countTime_HpDown = 0;
            }
        }
        if(flag.HasFlag(Define.FlagType.Sick))
        {
            countTime_HpDown2 += gameDeltaTime;
            if(countTime_HpDown2 > delayTime_HpDown)
            {
                player.stat.HP -= 2;
                countTime_HpDown2 = 0;
            }
        }

        if(Managers.Input.Splash)
        {
            countTime_splash += gameDeltaTime;
            if(countTime_splash > player.stat.curExhaustedSteps[2])
            {   
                if(flag.GetFlagStep(Define.FlagType.Exhausted) == 2)
                {
                    flag.ChangeFlagStep(Define.FlagType.Exhausted, isPlus : true);
                    countTime_splash = 0;
                }
            }
            else if(countTime_splash > player.stat.curExhaustedSteps[1])
            {
                if(flag.GetFlagStep(Define.FlagType.Exhausted) == 1)
                    flag.ChangeFlagStep(Define.FlagType.Exhausted, isPlus: true);
            }
            else if(countTime_splash > player.stat.curExhaustedSteps[0])
            {
                if(!flag.HasFlag(Define.FlagType.Exhausted))
                    flag.AddFlag(Define.FlagType.Exhausted);
            }
        }
        else
        {
            if(flag.HasFlag(Define.FlagType.Exhausted))
            {
                countTime_exhausted -= gameDeltaTime;
                if(countTime_exhausted < 0)
                {
                    flag.ChangeFlagStep(Define.FlagType.Exhausted, isPlus: false);
                    countTime_exhausted = 25f;
                    if(flag.GetFlagStep(Define.FlagType.Exhausted) == 0)
                        countTime_splash = 0;
                    else if(flag.GetFlagStep(Define.FlagType.Exhausted) == 1)
                        countTime_splash = player.stat.curExhaustedSteps[0];
                }
            }
        }
        
        
    }

    public void FadeOutWall()
    {
        // 인풋이 있을 때
        if(Managers.Input.Hinput != 0 && Managers.Input.Vinput != 0)
        {
            // 플레이어 좌표를 스크린포인트(픽셀단위)로 변환 
            Vector3 screenPos = Camera.main.WorldToScreenPoint(Managers.Game.player.transform.position);
            // 카메라에서 스크린포인트(플레이어 위치)를 통과하는 레이 생성
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // 레이캐스트로 카메라와 플레이어 사이에 wall이 있는 지 감지
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, 1<<11); // Layer11 : Wall
            // 감지된 벽을 순회
            foreach(RaycastHit hit in hits)
            {
                Wall tempWall = hit.collider.GetComponent<Wall>();
                if(tempWall == null)   
                    continue;

                // 감지한 벽이 투명하지 않다면, 렌더러 비활성화 처리하고 transparentWalls 리스트에 저장
                if(!tempWall.isTransparent)
                {
                    tempWall.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    tempWall.isTransparent = true;
                    transparentWalls.Add(tempWall);   
                }
            }

            // transparentWalls 리스트를 역순회하며..
            // 레이가 감지한 배열에 포함되어 있는 지 확인하고
            // 포함되지 않았다면 플레이어를 가리고 있지 않는 것이므로 다시 렌더러를 켜줌 
            // transparentWalls 리스트에서 제거
            for(int i = transparentWalls.Count -1; i >= 0; i--)
            {
                for (int j = 0; j < hits.Length; j ++)
                {
                    if(hits[j].collider.GetComponent<Wall>() == transparentWalls[i])
                        return;
                }

                transparentWalls[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                transparentWalls[i].isTransparent = false;
                transparentWalls.Remove(transparentWalls[i]); 
            }
        }
    }
}
