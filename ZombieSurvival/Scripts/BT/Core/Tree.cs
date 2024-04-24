
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 행동트리를 작동시키는, 루트가 있는 트리.
    /// </summary>
    public abstract class Tree : MonoBehaviour
    {
        protected Node root = null;

        protected void Start()
        {
            root = SetupTree();
        }
        protected virtual void Update()
        {
            if(root != null )
                root.Evaluate();
        }

        protected abstract Node SetupTree();
    
    }

}
