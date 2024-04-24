
using BehaviorTree;
using UnityEngine;

public class CheckFOVRange : Node
{
    Transform transform;
    int obstacleMask = 1<<3 | 1<<11 | 1<<12;    // Layer3 "Obstacle", Layer11 "Wall", Layer12 "Door"
    int targetMask = 1<<9 | 1<<13;              // Layer9 "Player", Layer13 "DeadBody"
    public CheckFOVRange(Transform _transform)
    {
        transform = _transform;
    }

    public override NodeState Evaluate()
    {
        object target = GetData("target");
        bool isSuccess = false;
        if(target == null)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 10f, targetMask);
            if(cols.Length > 0 )
            {
                Vector3 dirToTarget = cols[0].transform.position - transform.position;
                if(!Physics.Raycast(transform.position, dirToTarget, 5f, obstacleMask))
                {
                    if(Vector3.Distance(transform.position, cols[0].transform.position) < 1.8f)
                        isSuccess = true;

                    float angle = Vector3.Angle(transform.forward, dirToTarget.normalized);
                    if(angle < 120/2)
                        isSuccess = true;
                }
            }

            if(isSuccess)
            {
                FindRoot(this).SetData("target", cols[0].transform);
                state = NodeState.Success;
            }
            else
            {
                state = NodeState.Failure;
            }
            //Debug.Log($"CheckFOVRange {state}");
            return state;
        }   
        state = NodeState.Success;
        //Debug.Log($"CheckFOVRange {state}");
        return state;
    }
}
