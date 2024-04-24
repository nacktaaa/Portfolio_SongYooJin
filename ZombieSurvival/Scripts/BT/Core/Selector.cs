
using System.Collections.Generic;


namespace BehaviorTree
{
    /// <summary>
    /// 자식 노드 중 하나라도 Success 라면 Success 를 반환하고 순회 중단.
    /// 여러 행동 중 하나만 취할 때 사용
    /// </summary>
    public class Selector : Node
    {
        public Selector() : base() {}
        public Selector(List<Node> children) : base(children) {}
        
        public override NodeState Evaluate()
        {
            foreach(Node node in children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.Failure :
                        continue;
                    case NodeState.Success :
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running :
                        state = NodeState.Running;
                        return state;
                    default :
                        continue;
                }
            }
            state = NodeState.Failure;
            return state;
        }
    }

}
