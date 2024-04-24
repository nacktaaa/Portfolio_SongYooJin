using System;

using UnityEngine;

[Serializable]
public class PlayerStat : MonoBehaviour
{
# region 필드 목록
    // 기본 스탯
    [SerializeField]
    int curHp = 100;            // 체력
    [SerializeField]
    int maxHp = 100;         // 최대 체력
    [SerializeField]
    public int curMaxHp;
    [SerializeField]
    float hunger = 0;      // 허기
    [SerializeField]
    float[] hungerSteps = new float[] { 50, 75, 100 };
    [SerializeField]
    float thirst = 0;      // 목마름
    [SerializeField]
    float[] thirstSteps = new float[] { 50, 75, 100 };
    [SerializeField]
    float sleepiness = 0;  // 졸음
    [SerializeField]
    float[] tiredSteps = new float[] { 50, 75, 100 };
    [SerializeField]
    public float[] curTiredSteps = new float[3];
    [SerializeField]
    float curWeight = 0;    // 현재 무게
    [SerializeField]
    float[] overWeights = new float[] {10, 18, 20};
    [SerializeField]
    public float[] curOverWeights = new float[3];

    [SerializeField]
    float[] bleedingSetps = new float[] {10, 20};
    [SerializeField]
    float viewAngle = 120f; // 시야각 
    [SerializeField]
    public float curViewAngle;
    [SerializeField]
    float detectRange = 2f; //   주변감지영역
    [SerializeField]
    public float curDetectRange;
    [SerializeField]
    float injuryPoints = 0; // 부상 입은 정도.  3 이상이면 부상 플래그 발동
    [SerializeField]
    float sickPoints = 0; // 아픈 정도. 

    // 팩터 변수 
    [SerializeField]
    float basicSpeed = 5f;      // 기본 이동 속도
    [SerializeField]
    public float curBasicSpeed;
    [SerializeField]
    float splashSpeed = 1.5f;  // 대쉬 이동 속도
    [SerializeField]
    float crouchSpeed = 0.6f;    // 웅크린 이동 속도
    [SerializeField]
    float rotateSpeed = 180f;  // 회전 속도
    [SerializeField]
    public float curRotateSpeed;
    [SerializeField]
    float actionSpeed = 5f;        // 아이템 습득,사용 속도
    [SerializeField]
    float curActionSpeed;
    [SerializeField]
    float attack = 5;              // 공격력
    [SerializeField]
    public float curAttack;
    [SerializeField]
    public float bleedingRate = 80;         // 출혈 확률(%)
    [SerializeField]
    float injuryRate = 70;          // 부상 확률(%)
    [SerializeField]
    float curInjuryRate;
    [SerializeField]
    float sickRate = 50;            // 병에 걸릴 확률(%)
    [SerializeField]
    float curSickRate;
    [SerializeField]
    public float infectionRate = 25f;            // 좀비에 감염될 확률(%)
    [SerializeField]
    float[] exhaustedSteps = new float[] {40, 50, 60};
    [SerializeField]
    public float[] curExhaustedSteps = new float[3];
    [SerializeField]
    public bool isInfected = false;

#endregion

    public int HP 
    {
        get{return curHp;} 
        set 
        {
            curHp = value;
            curHp = Mathf.Clamp(curHp, 0, curMaxHp);

            if(curHp <=0)
            {
                //Debug.Log("GAME OVER");
                Managers.Game.causeOfDeath = "HP = 0";
                Managers.Game.player.SetState(Define.PlayerStateType.Death);
            }
        }
    }
    public float Hunger 
    {
        get{return hunger;}
        set
        {
            hunger = value;
            CheckFlagStep(hunger, hungerSteps, Define.FlagType.Hunger, isHunger:true);
        }
    }
    public float Thirst 
    {
        get{return thirst;}
        set
        {
            thirst = value;
            CheckFlagStep(thirst, thirstSteps, Define.FlagType.Thirst);
        }
    }
    public float Sleepiness 
    {
        get{return sleepiness;}
        set
        {
            sleepiness = value;
            CheckFlagStep(sleepiness, curTiredSteps, Define.FlagType.Tired);
        }
    }
    public float CurWeight
    {
        get { return curWeight; }
        set
        {
            curWeight = value;
            CheckFlagStep(curWeight, curOverWeights, Define.FlagType.Overweight);
        }
    }
    public float DetectRange {get {return curDetectRange;}}
    public float CurInjuryRate {get {return curInjuryRate;}}
    public float CurSickRate {get {return curSickRate;}}
    public float InjuryPoints 
    {   get {return injuryPoints;}
        set
        {
            injuryPoints = value;
            if (injuryPoints >= 3)
            {
                if(!Managers.Game.flag.HasFlag(Define.FlagType.Injury))
                {
                    Managers.Game.flag.AddFlag(Define.FlagType.Injury);
                }
                else
                {
                    Managers.Game.flag.ChangeFlagStep(Define.FlagType.Injury, isPlus: true);
                }
                    injuryPoints = 0;
            }

        }
    }
    public float SickPoints
    {
        get { return sickPoints;}
        set
        {
            sickPoints = value;
            if (sickPoints >= 3)
            {
                Managers.Game.flag.AddFlag(Define.FlagType.Sick);
                sickPoints = 0;
            }
        }
    }
    public float CurActionSpeed {get {return curActionSpeed;}}

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Managers.Game.flag.FlagAddOrChangeEvent -= ChangeStats;
        Managers.Game.flag.FlagAddOrChangeEvent += ChangeStats;
        Managers.Game.flag.FlagRemoveEvent -= RestoreStats;
        Managers.Game.flag.FlagRemoveEvent += RestoreStats;
    }
    public void Clear()
    {
        Managers.Game.flag.FlagAddOrChangeEvent -= ChangeStats;
        Managers.Game.flag.FlagRemoveEvent -= RestoreStats;        
    }

    private void Init()
    {
        curMaxHp = maxHp;
        curViewAngle = viewAngle;
        curDetectRange = detectRange;
        curBasicSpeed = basicSpeed;
        curRotateSpeed = rotateSpeed;
        curActionSpeed = actionSpeed;
        curAttack = attack;
        curInjuryRate = injuryRate;
        curSickRate = sickRate;
        
        for(int i = 0; i < tiredSteps.Length; i++)
        {
            curTiredSteps[i] = tiredSteps[i];
            curOverWeights[i] = overWeights[i];
            curExhaustedSteps[i] = exhaustedSteps[i];
        }

        HP = curMaxHp;


        for(int j = 0; j < bleedingSetps.Length; j ++)
            Managers.Game.durTime_Bleeding[j] = bleedingSetps[j];
    }

    private void RestoreStats(Define.FlagType type)
    {
        //Debug.Log($"{type}플래그가 제거 되었다.");
        ChangeStats(type, 0);
    }

    private void ChangeStats(Define.FlagType type, int step)
    {
        switch(type)
        {
            case Define.FlagType.Hunger :
            {
                if(step == 1)
                {
                    //Debug.Log($"{type}플래그가 {step}단계가 되었다. ");
                    curAttack *= 0.85f;
                    for(int i = 0; i < curOverWeights.Length; i++)
                        curOverWeights[i] *= 0.9f;
                }
                else if(step == 2)
                {
                    //Debug.Log($"{type}플래그가 {step}단계가 되었다. ");
                    curAttack *= 0.85f;
                    for(int i = 0; i < curOverWeights.Length; i++)
                        curOverWeights[i] *= 0.9f;
                }
                else
                {
                    curAttack = attack;
                    for(int i = 0; i < curOverWeights.Length; i++)
                        curOverWeights[i] = overWeights[i];
                }
                CheckFlagStep(curWeight, curOverWeights, Define.FlagType.Overweight);
            }
                break;
            case Define.FlagType.Full :
            {
                if(step == 1)
                {
                    curAttack *= 1.2f;
                    curInjuryRate *= 0.8f;
                    curSickRate *= 0.8f;
                    for(int i = 0; i < curOverWeights.Length; i++)
                        curOverWeights[i] *= 1.2f;
                }
                else
                {
                    curAttack = attack;
                    curInjuryRate = injuryRate;
                    curSickRate = sickRate;
                    for(int i = 0; i < curOverWeights.Length; i++)
                        curOverWeights[i] = overWeights[i];
                }
            }
                break;
            case Define.FlagType.Thirst :
            {
                if(step == 1)
                {
                    curBasicSpeed *= 0.95f;
                    curRotateSpeed *= 0.95f;
                    curViewAngle *= 0.95f;
                    curDetectRange *= 0.95f;
                }
                else if(step == 2)
                {
                    curBasicSpeed *= 0.85f;
                    curRotateSpeed *= 0.85f;
                    curViewAngle *= 0.85f;
                    curDetectRange *= 0.85f;
                }
                else
                {
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                    curViewAngle = viewAngle;
                    curDetectRange = detectRange;
                }
                Managers.Game.player.ChangeViewAngle();
            }
                break;
            case Define.FlagType.Tired :
            {
                if(step == 1)
                {
                    curBasicSpeed *= 0.9f;
                    curRotateSpeed *= 0.9f;
                    curViewAngle *= 0.9f;
                    curDetectRange *= 0.9f;
                }
                else if(step == 2)
                {
                    curBasicSpeed *= 0.8f;
                    curRotateSpeed *= 0.8f;
                    curViewAngle *= 0.8f;
                    curDetectRange *= 0.8f;
                }
                else
                {
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                    curViewAngle = viewAngle;
                    curDetectRange = detectRange;
                }
                Managers.Game.player.ChangeViewAngle();
            }
                break;
            case Define.FlagType.Overweight :
            {
                if(step == 1)
                {
                    curBasicSpeed *= 0.85f;
                    curRotateSpeed *= 0.85f;
                }
                else if(step == 2)
                {
                    curBasicSpeed *= 0.7f;
                    curRotateSpeed *= 0.7f;
                }
                else
                {
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                }
            }
                break;
            case Define.FlagType.Bleeding :
            {
                if(step == 1)
                {
                    curSickRate *= 1.1f;
                    for(int i = 0; i < curExhaustedSteps.Length; i++)
                        curExhaustedSteps[i] *= 0.9f;
                    for(int i = 0; i < curTiredSteps.Length; i++)
                        curTiredSteps[i] *= 0.9f;
                }
                else if(step == 2)
                {
                    curSickRate *= 1.2f;
                    for(int i = 0; i < curExhaustedSteps.Length; i++)
                        curExhaustedSteps[i] *= 0.8f;
                    for(int i = 0; i < curTiredSteps.Length; i++)
                        curTiredSteps[i] *= 0.8f;
                }
                else
                {
                    curSickRate = sickRate;
                    for(int i = 0; i < curExhaustedSteps.Length; i++)
                        curExhaustedSteps[i] = exhaustedSteps[i];
                    for(int i = 0; i < curTiredSteps.Length; i++)
                        curTiredSteps[i] = tiredSteps[i];
                }
            }
                break;
            case Define.FlagType.Injury :
            {
                if(step == 1)
                {
                    curAttack *= 0.85f;
                    curBasicSpeed *= 0.9f;
                    curRotateSpeed *= 0.9f;
                    curActionSpeed *= 0.9f;
                }
                else if(step == 2)
                {
                    curAttack *= 0.85f;
                    curBasicSpeed *= 0.85f;
                    curRotateSpeed *= 0.85f;
                    curActionSpeed *= 0.85f;
                }
                else
                {
                    curAttack = attack;
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                    curActionSpeed = actionSpeed;
                }
            }
                break;
            case Define.FlagType.Sick :
            {
                if(step == 1)
                {
                    curAttack *= 0.9f;
                    curBasicSpeed *= 0.9f;
                    curRotateSpeed *= 0.9f;
                    curActionSpeed *= 0.9f;
                }
                else if(step == 2)
                {
                    curAttack *= 0.8f;
                    curBasicSpeed *= 0.8f;
                    curRotateSpeed *= 0.8f;
                    curActionSpeed *= 0.8f;
                }
                else
                {
                    curAttack = attack;
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                    curActionSpeed = actionSpeed;
                }
            }
                break;
            case Define.FlagType.Exhausted :
            {
                if(step == 1)
                {
                    curBasicSpeed *= 0.85f;
                    curRotateSpeed *= 0.85f;
                    curActionSpeed *= 0.85f;
                }
                else if(step == 2)
                {
                    curBasicSpeed *= 0.75f;
                    curRotateSpeed *= 0.75f;
                    curActionSpeed *= 0.75f;
                }
                else
                {
                    curBasicSpeed = basicSpeed;
                    curRotateSpeed = rotateSpeed;
                    curActionSpeed = actionSpeed;
                }
            }
                break;
            case Define.FlagType.Death :
            {
                Managers.Game.causeOfDeath = type.ToString();
                Managers.Game.player.SetState(Define.PlayerStateType.Death);
            }   
                break;
            case Define.FlagType.Zombie :
            {
                Managers.Game.causeOfDeath = type.ToString();
                Managers.Game.player.SetState(Define.PlayerStateType.Death);
            }
                break;
            case Define.FlagType.None :
                break;
        }
    }

    public float GetMaxSpeed()
    {
        return basicSpeed * splashSpeed;
    }

    public float GetDamage()
    {
        float damage = curAttack;

        if(Managers.Game.player.isEquip)
            damage += Managers.Game.player.mountedWeapon.stat.atk;

        return damage;
    }

    public float GetMoveSpeed(bool _isCrouch = false, bool _isAim = false)
    {
        float speed = curBasicSpeed;
        if(_isCrouch)
        {
            speed *= crouchSpeed;
        }

        if (Managers.Input.Splash)
            speed *= splashSpeed;

        if(_isAim)
            speed = curBasicSpeed * 0.4f;
        
        return speed;
    }
    
    public float GetRotateSpeed()
    {
        float speed = curRotateSpeed;
        return speed;
    }

    public float GetViewAngle()
    {
        float angle = curViewAngle;
        return angle;
    }

    public void CheckFlagStep(float curStat, float[] steps, Define.FlagType _flagtype, bool isHunger = false)
    {
        if(curStat >= steps[0])  // curStat의 수치가 1단계 수치 이상일 경우
            {
                // 이미 해당 플래그를 갖고 있을 경우
                if(Managers.Game.flag.HasFlag(_flagtype)) 
                {
                    // 이미 해당 플래그가 2단계일 경우
                    if(Managers.Game.flag.GetFlagStep(_flagtype) == 2)
                    {
                        if(curStat >= steps[2]) // 3단계 수치 이상이라면, 사망 처리
                        {
                            Managers.Game.flag.RemoveAllFlag();
                            Managers.Game.flag.AddFlag(Define.FlagType.Death);
                            Managers.Game.causeOfDeath = _flagtype.ToString();
                        }
                        else if (curStat < steps[1]) // 수치가 2단계 이하라면
                        {
                            // 플래그 단계 낮추기
                            Managers.Game.flag.ChangeFlagStep(_flagtype, isPlus: false);
                        }
                    }
                    // 해당 플래그가 1단계일 경우
                    else if(Managers.Game.flag.GetFlagStep(_flagtype) == 1)
                    {
                        if(curStat >= steps[1]) // 2단계 수치 이상이라면
                        {
                            // 플래그 단계 증가
                            Managers.Game.flag.ChangeFlagStep(_flagtype, isPlus: true);
                        }
                    }
                }
                else // 해당 플래그가 없었을 경우
                {
                    // 해당 플래그 추가
                    Managers.Game.flag.AddFlag(_flagtype);
                }
            }
            else // 1단계 수치 이하일 경우
            {
                // 이미 해당 플래그가 있었다면
                if(Managers.Game.flag.HasFlag(_flagtype)) 
                    Managers.Game.flag.RemoveFlag(_flagtype); // 플래그 삭제
                
                if(isHunger && curStat < 0) // Full 플래그의 경우 예외처리 
                {
                    if(!Managers.Game.flag.HasFlag(Define.FlagType.Full))
                        Managers.Game.flag.AddFlag(Define.FlagType.Full);
                }
            }
    }

    public string GetWeightText()
    {
        string weightText = string.Format("{0:0.##}/{1:0.##}", CurWeight, curOverWeights[0]);
        return weightText;
    }

    public void LoadStats(SaveData _saveData)
    {
        curHp = _saveData.curHp;
        curMaxHp = _saveData.curMaxHp;
        hunger = _saveData.hunger;
        thirst = _saveData.thirst;
        sleepiness = _saveData.sleepiness;
        curWeight = _saveData.curWeight;
        curViewAngle = _saveData.curViewAngle;
        curDetectRange = _saveData.curDetectRange;
        injuryPoints = _saveData.injuryPoints;
        sickPoints = _saveData.sickPoints;
        curBasicSpeed = _saveData.curBasicSpeed;
        curRotateSpeed = _saveData.curRotateSpeed;
        curActionSpeed = _saveData.curActionSpeed;
        curAttack = _saveData.curAttack;
        curInjuryRate = _saveData.curInjuryRate;
        curSickRate = _saveData.curSickRate;
        isInfected = _saveData.isInfected;

        for(int i = 0; i < curTiredSteps.Length; i ++)
        {
            curTiredSteps[i] = _saveData.curTiredSteps[i];
            curOverWeights[i] = _saveData.curOverWeights[i];
            curExhaustedSteps[i] = _saveData.curExhaustedSteps[i];
        }
    
    }
}
