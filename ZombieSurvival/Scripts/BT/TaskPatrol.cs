using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using UnityEngine.AI;
using System;

public class TaskPatrol : Node
{
    Animator anim;
    Transform transform;
    Vector3 destination;
    NavMeshAgent navMeshAgent;
    AudioSource audio;
    Define.ZombieGender genderType;
    float soundTime = UnityEngine.Random.Range(3f, 6f);
    float soundCounter = 0f;

    float waitTime = UnityEngine.Random.Range(10f, 15f);
    float waitCounter = 0f;
    bool isWait = true;
    
    public TaskPatrol(Transform _transform, Define.ZombieGender _genderType)
    {
        transform = _transform;
        anim = transform.GetComponent<Animator>();
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        audio = transform.GetComponent<AudioSource>();
        genderType = _genderType;
    }

    public override NodeState Evaluate()
    {
        if(isWait)
        {
            anim.SetBool("IsMove", false);
            waitCounter += Time.deltaTime;
            if(waitCounter > waitTime)
            {
                isWait = false;
                if(RandomPoint(transform.position, UnityEngine.Random.Range(-8f, 8f), out destination))
                {
                    navMeshAgent.SetDestination(destination);
                    navMeshAgent.isStopped = false;
                    anim.SetBool("IsMove", true);
                }
                waitCounter = 0;
            }    
        }
        else
        {
            navMeshAgent.SetDestination(destination);
            if(Vector3.Distance(transform.position, navMeshAgent.destination) < 0.01f)
            {
                isWait = true;
                navMeshAgent.isStopped = true;
                anim.SetBool("IsMove", false);
            }

        }

        IdleSoundPlay();
        state = NodeState.Running;
        //Debug.Log($"TaskPatrol {state}");
        return state;
    }

    private void IdleSoundPlay()
    {
        soundCounter += Time.deltaTime;
        string rndKey = null;
        if(soundCounter > soundTime)
        {
            int rndIdx = UnityEngine.Random.Range(1,9);
            switch(genderType)
            {
                case Define.ZombieGender.Female :
                    rndKey = $"FIdle{rndIdx}";
                    break;
                case Define.ZombieGender.Male :
                    rndKey = $"MIdle{rndIdx}";
                    break;
            }
            if(rndKey != null)
                audio.PlayOneShot(Managers.Sound.zombieSoundsDict[rndKey]);
            
            soundCounter = 0;
        } 
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for(int i = 0; i < 30; i ++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
}
