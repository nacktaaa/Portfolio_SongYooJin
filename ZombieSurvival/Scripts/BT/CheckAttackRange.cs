
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckAttackRange : Node
{
    Transform transform;
    Animator anim;
    NavMeshAgent navMeshAgent;
    public CheckAttackRange(Transform _transform)
    {
        transform = _transform;
        navMeshAgent = _transform.GetComponent<NavMeshAgent>();
        anim = transform.GetComponent<Animator>();
    }
    public override NodeState Evaluate()
    {
        object target = GetData("target");
        if(target != null)
        {
            if(transform.GetComponent<ZombieBT>().isAttacked)
            {
                state = NodeState.Failure;
                return state;
            }

            Transform targetPos = (Transform)target;
            if(targetPos == null)
            {
                anim.SetBool("IsAttack", false);
                anim.SetBool("IsEat", false);
                state = NodeState.Failure;
                return state;
            }

            if(Vector3.Distance(transform.position, targetPos.position) <= 1.1f)
            {
                navMeshAgent.isStopped = true;
                if(targetPos.gameObject.layer.Equals(LayerMask.NameToLayer("DeadBody")))
                    anim.SetBool("IsEat", true);
                else
                    anim.SetBool("IsAttack", true);
                
                anim.SetBool("IsMove", false);
                state = NodeState.Success;
                //Debug.Log($"CheckAttackRagne {state}");
                return state;
            }
        }
        anim.SetBool("IsAttack", false);
        state = NodeState.Failure;
        //Debug.Log($"CheckAttackRagne {state}");
        return state;
    }
}
