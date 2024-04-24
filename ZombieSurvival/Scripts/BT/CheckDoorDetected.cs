
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckDoorDetected : Node
{
    Transform transform;
    NavMeshAgent navMeshAgent;
    Animator anim;

    public CheckDoorDetected(Transform _transform)
    {
        transform = _transform;
        navMeshAgent = transform.GetComponent<NavMeshAgent>();
        anim = transform.GetComponent<Animator>();
    }
    public override NodeState Evaluate()
    {
        if(transform.GetComponent<ZombieBT>().isAttacked)
        {
            state = NodeState.Failure;
            if(GetData("door") != null)
            {
                ClearData("door");
                anim.SetBool("IsDoorAttack", false);
            }
            return state;
        }    

        if(Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hit, 0.95f, 1<<12))
        {
            if(!hit.collider.GetComponent<Door>().isOpen)
            {
                navMeshAgent.isStopped = true;
                //Debug.Log("문 발견");
                Node root = FindRoot(this);
                root.SetData("door", hit.transform);
                anim.SetBool("IsDoorAttack", true);
                anim.SetBool("IsMove", false);

                state = NodeState.Success;
                //Debug.Log($"CheckDoorDetected {state}");
                return state;
            }
            else
            {
                ClearData("door");
                anim.SetBool("IsDoorAttack", false);
            }
        }

        state = NodeState.Failure;
        anim.SetBool("IsDoorAttack", false);
        //Debug.Log($"CheckDoorDetected {state}");
        return state;
    }
}
