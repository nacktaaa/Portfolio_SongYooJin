
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskAttack : Node
{
    float attackTime = 2.5f;
    bool isTaskRunning = false;
    Transform transform;
    Animator anim;
    NavMeshAgent navMeshAgent;
    AudioSource audio;
    Define.ZombieGender genderType;
    
    public TaskAttack(Transform _transform, Define.ZombieGender _genderType)
    {
        transform = _transform;
        anim = transform.GetComponent<Animator>();
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        audio = transform.GetComponent<AudioSource>();
        genderType = _genderType;
        isTaskRunning = false;
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if(target == null)
        {
            anim.SetBool("IsAttack", false);
            anim.SetBool("IsEat", false);
            state = NodeState.Failure;
            return state;
        }
        else
        {
            if(!isTaskRunning)
            {
                if(target.gameObject.layer.Equals(LayerMask.NameToLayer("DeadBody")))
                {
                    bool isOver = target.GetComponent<DeadBody>().OnDamaged();
                    EatingSoundplay();
                    if(isOver)
                    {
                        ClearData("target");
                        anim.SetBool("IsEat", false);
                        navMeshAgent.isStopped = false;
                    }
                }
                else
                {
                    Vector3 dir = target.position - transform.position;
                    if(Vector3.Dot(transform.forward, dir.normalized) >= 0.5f)
                    {
                        if(target.GetComponent<PlayerController>()?.stat.HP > 0)
                        {
                            target.GetComponent<PlayerController>().OnDamaged();
                            BiteSoundPlay();
                        }
                        else
                        {
                            ClearData("target");
                            anim.SetBool("IsAttack", false);
                            state = NodeState.Failure;
                            return state;
                        }
                    }
                    else
                    {
                        state = NodeState.Failure;
                        return state;
                    }
                }
                isTaskRunning = true;
            }

            transform.GetComponent<ZombieBT>().attackCount += Time.deltaTime;
            if(transform.GetComponent<ZombieBT>().attackCount > attackTime)
            {
                transform.GetComponent<ZombieBT>().attackCount = 0;
                isTaskRunning = false;
            }
        }

        state = NodeState.Running;
        //Debug.Log($"TaskAttack {state}");
        return state;
    }

    void EatingSoundplay()
    {
        switch(genderType)
        {
            case Define.ZombieGender.Female :
                audio.PlayOneShot(Managers.Sound.zombieSoundsDict["FEating"]);
                break;
            case Define.ZombieGender.Male :
                audio.PlayOneShot(Managers.Sound.zombieSoundsDict["MEating"]);
                break;
        }
    }

    void BiteSoundPlay()
    {
        int rndIdx = UnityEngine.Random.Range(0,2);
        if(rndIdx != 0)
            audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Bite1"]);
        else
            audio.PlayOneShot(Managers.Sound.zombieSoundsDict["Bite2"]);

    }
}
