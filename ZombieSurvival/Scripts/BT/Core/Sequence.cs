
using System.Collections.Generic;


namespace BehaviorTree
{
    /// <summary>
    /// 자식 노드 중 하나라도 Failure일 경우 순회를 중단하고 Failure을 반환 
    /// 왼쪽노드부터 순차적으로 진행해야 하는 행동에 적합
    /// </summary>
    public class Sequence : Node
    {
        public Sequence() : base() {}
        public Sequence(List<Node> children) : base(children) {}
        
        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach(Node node in children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.Failure :
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Success :
                        continue;
                    case NodeState.Running :
                        anyChildIsRunning = true;
                        continue;
                    default :
                        state = NodeState.Success;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }

}
