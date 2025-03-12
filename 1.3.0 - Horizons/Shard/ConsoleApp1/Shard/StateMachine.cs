using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.StateMachine
{
    class State
    {
        public string stateName;
    }

    class StateTransition<T> where T : State
    {
        public T FromState { get; private set; }
        public T ToState { get; private set; }
        public Func<bool> Condition { get; private set; }

        public StateTransition(T fromState, T toState, Func<bool> condition)
        {
            FromState = fromState;
            ToState = toState;
            Condition = condition;
        }
    }
}
