using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskOnDeath : Node
{
    Transform transform;
    NavMeshAgent navMeshAgent;
    Animator anim;
    bool isTaskRunning = false;
    float runningTime = 5f;
    float countTime = 0;
    AudioSource audio;
    Define.ZombieGender genderType;

    public TaskOnDeath(Transform _transform, Define.ZombieGender _genderType)
    {
        transform = _transform;
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        anim = transform.GetComponent<Animator>();
        audio = transform.GetComponent<AudioSource>();
        genderType = _genderType;
        countTime = 0;
        isTaskRunning = false;
    }

    public override NodeState Evaluate()
    {
        if(!isTaskRunning)
        {
            anim.SetTrigger("IsDead");
            Managers.Game.killcount ++;
            navMeshAgent.isStopped = true;
            transform.GetComponent<Collider>().enabled = true;
            DeathSoundPlay();
            isTaskRunning = true;
        }

        countTime += Time.deltaTime;
        if(countTime > runningTime)
        {
            Managers.Pool.Push(transform.gameObject);
            transform.GetComponent<ZombieBT>().HPInit(); 
            //Debug.Log($"TaskOnDeath {state}");
            countTime = 0;
            state = NodeState.Success;
            return state;
        }
        //Debug.Log($"TaskOnDeath {state}");
        state = NodeState.Success;
        return state;
    }

    void DeathSoundPlay()
    {
        string rndKey = null;
        int rndIdx = UnityEngine.Random.Range(1,5);
        switch(genderType)
        {
            case Define.ZombieGender.Female :
                rndKey = $"FDeath{rndIdx}";
                break;
            case Define.ZombieGender.Male :
                rndKey = $"MDeath{rndIdx}";
                break;
        }
        if(rndKey != null)
            audio.PlayOneShot(Managers.Sound.zombieSoundsDict[rndKey]);
    }
}
