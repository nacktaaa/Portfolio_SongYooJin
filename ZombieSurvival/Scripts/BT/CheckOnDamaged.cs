
using BehaviorTree;
using UnityEngine;

public class CheckOnDamaged : Node
{
    Transform transform;

    public CheckOnDamaged(Transform _transform)
    {
        transform = _transform;
    }

    public override NodeState Evaluate()
    {
        if(transform.GetComponent<ZombieBT>().isAttacked)
        {
            state = NodeState.Success;
            transform.GetComponent<ZombieBT>().attackCount = 0;
            //Debug.Log($"CheckOnDamage {state}");
            return state;
        }
        state = NodeState.Failure;
        //Debug.Log($"CheckOnDamage {state}");
        return state;
    }

    
}
