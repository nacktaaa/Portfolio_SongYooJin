using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskDoorAttack : Node
{
    float attackTime = 1.2f;
    float attackCount = 1.2f;
    Animator anim;
    AudioSource audio;
    NavMeshAgent navMeshAgent;

    public TaskDoorAttack(Transform _transform)
    {
        anim = _transform.GetComponent<Animator>();
        navMeshAgent = _transform.GetComponent<NavMeshAgent>();
        audio = _transform.GetComponent<AudioSource>();
    }
    public override NodeState Evaluate()
    {
        Transform door = (Transform)GetData("door");
        if(door != null)
        {
            attackCount += Time.deltaTime;
            if(attackCount > attackTime)
            {
                bool isOver = door.GetComponent<Door>().OnDamaged();
                DoorAttackSoundPlay();
                if(isOver)
                {
                    ClearData("door");
                    BreakDoorSoundPlay();
                    anim.SetBool("IsDoorAttack", false);
                    navMeshAgent.isStopped = false;
                    state = NodeState.Failure;
                    return state;
                }
                attackCount = 0;
            }

             state = NodeState.Running;
             return state;
        }

        attackCount = 0;
        state = NodeState.Failure;
        //Debug.Log($"TaskDoorAttack {state}");
        return state;
    }

    void DoorAttackSoundPlay()
    {
        audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Thump"], 1f);
    }

    void BreakDoorSoundPlay()
    {
        audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Breakdoor"], 1f);
    }
}
