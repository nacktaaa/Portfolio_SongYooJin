using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
//using TreeEditor;
using UnityEngine;
using UnityEngine.AI;

public class TaskOnDamaged : Node
{
    Transform transform;
    Animator anim;
    NavMeshAgent navMeshAgent;
    AudioSource audio;
    bool isTaskRunning = false;
    float runningTime = 1.5f;
    float countTime = 0;

    public TaskOnDamaged(Transform _transform)
    {
        transform = _transform;
        anim = transform.GetComponent<Animator>();
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        audio = transform.GetComponent<AudioSource>();
        isTaskRunning = false;
        countTime = 0;
    }

    public override NodeState Evaluate()
    {
        if(!isTaskRunning)
        {
            // 좀비 hp 깎기
            transform.GetComponent<ZombieBT>().hp -= (int)Managers.Game.player.stat.GetDamage();
            DamagedSoundPlay();
            //Debug.Log("Damage" + (int)Managers.Game.player.stat.GetDamage());
            navMeshAgent.isStopped = true;
            // 좀비 피격 액션
            if(transform.GetComponent<ZombieBT>().hp <= 0)
            {
                transform.GetComponent<ZombieBT>().isDeath = true;
            }
            else
            {
                anim.SetTrigger("OnDamaged_A");
                anim.SetBool("IsAttack", false);
                anim.SetBool("IsMove", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsDoorAttack", false);
                if(Managers.Game.player != null)
                    FindRoot(this).SetData("target", Managers.Game.player.transform);
            }
            isTaskRunning = true;
        }
        countTime += Time.deltaTime;
        if(countTime >= runningTime)
        {
            transform.GetComponent<ZombieBT>().isAttacked = false;
            navMeshAgent.isStopped = false;
            isTaskRunning = false;
            countTime = 0;
        }
        state = NodeState.Running;
        //Debug.Log($"TaskOnDamaged {state}");
        return state;
    }

    void DamagedSoundPlay()
    {
        int rnd = UnityEngine.Random.Range(0,2);
        if(rnd != 0)
            audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Smash1"]);
        else
            audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Smash2"]);

    }

}
