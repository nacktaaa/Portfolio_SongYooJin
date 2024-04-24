using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PatternType
{
    None, Summon, FireProjectileLine, FireProjectileCurved, Melee, Jump 
}
public enum TriggerType
{
    None, CloseToPlayer
}

[CreateAssetMenu]
public class MonsterStatus : ScriptableObject
{
    [Header("Info")]
    public string monsterName;
    public float monsterSize = 1f;
    public Sprite sprite;
    public bool flipX = false;
    public Vector2 colSize = new Vector2(0.27f, 0.27f);
    public CapsuleDirection2D direction = CapsuleDirection2D.Horizontal;
    public RuntimeAnimatorController animCtr;
    
    [Header("Monster Property")]
    public float damage = 10;
    //public float defence = 3;
    public float hp = 10;
    [Range(0f,3f)]
    public float moveSpeed = 1;
    public float exp = 1;

    [Header("Attack Property")]
    public TriggerType triggerType;
    [Range(0.1f,10)]
    public float triggerRange = 1;
    public PatternType patternType;
    public string animState;

    //public float attackCoolTime = 1;
    public float baseCoolTime = 1;
    public float attackCoolTime = 1;
    public bool isStop;
    public bool isAfterDeath;
    public bool isTargeting;
    
    [Header("Projectile (option)")]
    public GameObject projectile;
    public float projectileSpeed = 1;
    [Tooltip("투사체의 개수")]
    [Range(1,99)]
    public int projectileAmount = 1;
    [Tooltip("몇초 후에 사라지는가")]
    public float lifeSpan = 1;
    [Tooltip("연사 인가?")]
    public bool isContinuous;
    [Tooltip("연사일 경우, 연사 횟수")]
    [Range(1,10)]
    public int continousCount = 1;
    [Tooltip("연사일 경우, 연사 간격을 조절 (초 단위)")]
    [Range(0.1f,10)]
    public float continuousInterval = 0.1f;

    [Header("Melee (option)")]
    [Tooltip("이펙트 프리팹 이름")]
    public string effectName = "";
    [Tooltip("공격 거리")]
    public float attackDist = 1;
    [Tooltip("공격 범위에 진입 시 약간의 딜레이")]
    [Range(0f, 0.005f)]
    public float attackDelay = 0.001f;

    [Header("Jump (option)")]
    [Tooltip("점프 높이에 영향")]
    public float jumpPower = 2;
    [Tooltip("점프 소요시간에 영향")]
    public float jumpDuration = 1;
    [Tooltip("점프 시 커브 타입(Dotween Ease 사용) In OutBack이 좀 자연스러움!")]
    public Ease curve = Ease.InCubic;

    [Header("Summon (option)")]
    [Tooltip("소환할 몬스터 스테이터스")]
    public MonsterStatus monsterTosummon;
    [Tooltip("소환 위치를 랜덤으로?")]
    public bool isRandomPos = false;
    [Tooltip("소환 거리")]
    public float summonDist = 1;
    [Tooltip("소환 수량")]
    public int summonAmount = 1;
}