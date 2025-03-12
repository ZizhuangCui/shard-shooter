using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.BehaviourTree
{
    public enum NodeState
    {
        Running,
        Success,
        Failure
    }

    public abstract class Node
    {
        public abstract NodeState Execute();
    }

    public abstract class Composite : Node
    {
        protected List<Node> children = new List<Node>();

        public void AddChild(Node child)
        {
            children.Add(child);
        }
    }

    public class Selector : Composite
    {
        public override NodeState Execute()
        {
            foreach (var child in children)
            {
                var state = child.Execute();
                if (state != NodeState.Failure)
                {
                    return state;
                }
            }
            return NodeState.Failure;
        }
    }

    public class Sequence : Composite
    {
        public override NodeState Execute()
        {
            foreach (var child in children)
            {
                var state = child.Execute();
                if (state != NodeState.Success)
                {
                    return state;
                }
            }
            return NodeState.Success;
        }
    }

    public class Inverter : Node
    {
        private Node child;

        public Inverter(Node child)
        {
            this.child = child;
        }

        public override NodeState Execute()
        {
            var state = child.Execute();
            return state switch
            {
                NodeState.Success => NodeState.Failure,
                NodeState.Failure => NodeState.Success,
                _ => NodeState.Running
            };
        }
    }

    public class ActionNode : Node
    {
        private Func<NodeState> action;

        public ActionNode(Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Execute()
        {
            return action.Invoke();
        }
    }
}
