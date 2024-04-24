using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    Transform transform;
    NavMeshAgent navMeshAgent;
    Animator anim;
    AudioSource audio;
    Define.ZombieGender genderType;
    float soundTime = UnityEngine.Random.Range(1f, 3f);
    float soundCounter = 0f;

    public TaskGoToTarget(Transform _transform, Define.ZombieGender _genderType)
    {
        transform = _transform;
        anim = _transform.GetComponent<Animator>();
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        audio = transform.GetComponent<AudioSource>();
        genderType = _genderType;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if(target != null)
        {
            if(target.GetComponent<PlayerController>() != null)
                Managers.Sound.PlayBGM(Define.BGM.Chase1);
                
            navMeshAgent.isStopped = false;
            anim.SetBool("IsMove", true);
            navMeshAgent.SetDestination(target.position);

            if(Vector3.Distance(transform.position, target.position) > 20f)
            {
                ClearData("target");
                state = NodeState.Failure;
                //Debug.Log($"TaskGoToTarget {state}");
                return state;
            }

            ChaseSoundPlay();

            state = NodeState.Running;
            //Debug.Log($"TaskGoToTarget {state}");
            return state;
        }
        state = NodeState.Failure;
        //Debug.Log($"TaskGoToTarget {state}");
        return state;
    }

    void ChaseSoundPlay()
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
            {
                audio.PlayOneShot(Managers.Sound.zombieSoundsDict[rndKey]);
            }
            soundCounter = 0;
        }   
    }
}
