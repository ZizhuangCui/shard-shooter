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

    class StateMachine<T> where T : State
    {
        protected Dictionary<string, T> states = new Dictionary<string, T>();
        protected List<StateTransition<T>> transitions = new List<StateTransition<T>>();
        protected T currentState;

        public void AddState(string name, T state)
        {
            states[name] = state;
        }

        public T GetState(string name)
        {
            return states.ContainsKey(name) ? states[name] : null;
        }

        public void AddTransition(T fromState, T toState, Func<bool> condition)
        {
            transitions.Add(new StateTransition<T>(fromState, toState, condition));
        }

        public void SetInitialState(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                currentState = states[stateName];
                OnStateEnter(currentState);
            }
            else
            {
                Console.WriteLine($"State {stateName} not found in StateMachine.");
            }
        }

        public void Update()
        {
            foreach (var transition in transitions)
            {
                if (transition.FromState == currentState && transition.Condition())
                {
                    SetCurrentState(transition.ToState);
                    break;
                }
            }

            OnStateUpdate(currentState);
        }

        protected virtual void OnStateEnter(T state) { }
        protected virtual void OnStateUpdate(T state) { }
        protected virtual void OnStateExit(T state) { }

        protected void SetCurrentState(T newState)
        {
            if (currentState != newState)
            {
                OnStateExit(currentState);
                currentState = newState;
                OnStateEnter(currentState);
            }
        }
    }

}
