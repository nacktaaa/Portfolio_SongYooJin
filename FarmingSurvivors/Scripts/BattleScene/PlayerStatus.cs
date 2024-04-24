using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatus : MonoBehaviour
{
    public Character character;

    [SerializeField]
    float[] NextLevelEXP;
    [SerializeField]
    public float baseMaxHp = 100, baseMoveSpeed = 5f, baseDefence = 5f, basePhysicalDamageMultiplier = 1, baseMagicalDamageMultiplier = 1;
    [SerializeField]
    private int curLevel = 0;
    [SerializeField]
    private float curExp = 0;
    [SerializeField]
    float curHp = 20;
    [SerializeField]
    float totalEXP = 0;
    public float maxHp;
    public float defence = 0;
    public float moveSpeed;

    public float attackDamageMultiplier = 1;
    public float physicalDamageMultiplier = 1;
    public float magicalDamageMultiplier = 1;
    public float cooltimeMultiplier = 1;
    public float lifespanMultiplier = 1;
    public float projSpeedMultiplier = 1;
    public float projAmountPlus = 0;
    public float projAreaMultiplier = 1;
    public float penetPlus = 0;
    public float expMultiplier = 1;


    public void Init(Dictionary<Character, Dictionary<Equipment.StatType, float>> finalMountStats, Character c)
    {

        curExp = 0;
        curLevel = 0;
        maxHp = baseMaxHp * (1 + finalMountStats[c][Equipment.StatType.Hp]);
        defence = baseDefence + finalMountStats[c][Equipment.StatType.Defence];
        attackDamageMultiplier = 1 + finalMountStats[c][Equipment.StatType.AttackDamage];
        physicalDamageMultiplier = basePhysicalDamageMultiplier + finalMountStats[c][Equipment.StatType.Strength];
        magicalDamageMultiplier = baseMagicalDamageMultiplier + finalMountStats[c][Equipment.StatType.Intelligence];
        cooltimeMultiplier = 1 + finalMountStats[c][Equipment.StatType.Cooltime];
        moveSpeed = baseMoveSpeed * (1 + finalMountStats[c][Equipment.StatType.MoveSpeed]);
        lifespanMultiplier = 1 + finalMountStats[c][Equipment.StatType.LifeSpan];
        projSpeedMultiplier = 1 + finalMountStats[c][Equipment.StatType.ProjectileSpeed];
        projAmountPlus = finalMountStats[c][Equipment.StatType.ProjectileAmount];
        projAreaMultiplier = 1+finalMountStats[c][Equipment.StatType.ProjectileArea];
        penetPlus = finalMountStats[c][Equipment.StatType.Penetration];
        expMultiplier = 1+finalMountStats[c][Equipment.StatType.ExpPlus];
        curHp = maxHp;
    }

    public float HP
    {
        get { return curHp; }
        set
        {
            curHp = value;

            if (curHp > maxHp)
                curHp = maxHp;

            if (curHp <= 0)
            {
                curHp = 0;
                transform.GetComponent<PlayerController>().isDead = true;
                transform.GetComponent<PlayerController>().OnDie();
            }
        }
    }
    public float EXP
    {
        get { return curExp; }
        set
        {
            if ((CurLevel + 1) >= NextLevelEXP.Length)
                return;
            
            curExp = value;
            totalEXP += curExp;

            GameManager.Instance.stageUI.basicStageUI.SetEXPSlider(GetEXPbarValue());

            if (curExp >= NextLevelEXP[CurLevel + 1]) // 레벨 업
            {
                CurLevel ++;
                EXP = 0;
                GameManager.Instance.stageUI.basicStageUI.SetEXPSlider(GetEXPbarValue());
                GameManager.Instance.OnLevelUp();
            }
        }
    }

    public int CurLevel
    {
        get { return curLevel; }
        set
        {
            curLevel = value >= NextLevelEXP.Length ? curLevel : value;
        }
    }

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float GetHPbarValue()
    {
        return HP / maxHp;
    }
    public float GetEXPbarValue()
    {
        if (CurLevel + 1 >= NextLevelEXP.Length)
            return 1;
        return curExp / NextLevelEXP[CurLevel + 1];
    }

}
