
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine.AI;
using UnityEngine;

public class ZombieBT : BehaviorTree.Tree, IPoolable
{
    [SerializeField]
    int maxHP = 100;
    [SerializeField]
    float moveSpeed = 5f;
    public int hp;
    public bool isDeath = false;
    public bool isAttacked = false;
    public bool isWaked = false;
    public float attackCount = 0;
    public GameObject bloodEffect;
    public Define.ZombieGender genderType;
    NavMeshAgent navMeshAgent;
    Animator anim;

    public Define.PoolableType Type { get; set; } = Define.PoolableType.Zombie;

    private void Awake()
    {
        HPInit();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.angularSpeed = 500f;
        navMeshAgent.speed = moveSpeed;
        anim = GetComponent<Animator>();
    }

    public void HPInit()
    {
        hp = maxHP; 
    }

    protected override void Update()
    {
        if(isWaked)
            return;
        
        if(Managers.Game.isPause)
        {
            navMeshAgent.SetDestination(transform.position);
            anim.Play("Idle");
            navMeshAgent.updateRotation = false;
            return;
        }
        else
        {
            navMeshAgent.updateRotation = true;
        }
            
        base.Update();
    }

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckOnDeath(transform),
                new TaskOnDeath(transform, genderType)
            }),
            new Sequence(new List<Node>
            {
                new CheckOnDamaged(transform),
                new TaskOnDamaged(transform)
            }),
            new Selector(new List<Node> 
            {
                new Sequence(new List<Node>
                {
                    new CheckDoorDetected(transform),
                    new TaskDoorAttack(transform)
                }),
                new Sequence(new List<Node>
                {
                    new CheckAttackRange(transform),
                    new TaskAttack(transform, genderType)
                })
            }),
            new Sequence(new List<Node>
            {
                new CheckFOVRange(transform),
                new TaskGoToTarget(transform, genderType)
            }),
            new TaskPatrol(transform, genderType)
        });
        return root;
    }

    public void ShowBloodEffect(Vector3 _pos, Quaternion _rot)
    {
        GameObject blood = Instantiate(bloodEffect, _pos, _rot, transform);
        Destroy(blood, 0.8f);
    }

    public void OnDamaged(float _damage)
    {
        isAttacked = true;
    }


}
