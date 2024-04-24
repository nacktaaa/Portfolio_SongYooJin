using BehaviorTree;
using UnityEngine;

public class CheckOnDeath : Node
{
    Transform transform;
    public CheckOnDeath(Transform _transform)
    {
        transform = _transform;
    }

    public override NodeState Evaluate()
    {
        if(transform.GetComponent<ZombieBT>().isDeath)
        {
            state = NodeState.Success;
            //Debug.Log($"CheckOnDeath {state}");
            return state;
        }
        state = NodeState.Failure;
        //Debug.Log($"CheckOnDeath {state}");
        return state;
    }
}
