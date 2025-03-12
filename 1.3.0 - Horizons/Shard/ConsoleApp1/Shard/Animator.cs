using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shard.StateMachine;

namespace Shard.Animation
{
    class Animator
    {
        private Dictionary<string, AnimationState> states;
        private List<StateTransition<AnimationState>> transitions;
        private AnimationState currentState;

        public Animator()
        {
            states = new Dictionary<string, AnimationState>();
            transitions = new List<StateTransition<AnimationState>>();
        }

        public void AddState(string name, Animation animation)
        {
            AnimationState state = new AnimationState(name, animation);
            states[name] = state;
        }

        public AnimationState GetState(string name)
        {
            return states.ContainsKey(name) ? states[name] : null;
        }
        public void AddTransition(AnimationState fromState, AnimationState toState, Func<bool> condition)
        {
            transitions.Add(new StateTransition<AnimationState>(fromState, toState, condition));
        }

        public void SetInitialState(string stateName)
        {
            if (states.ContainsKey(stateName))
            {
                currentState = states[stateName];
                currentState.animationClip.Play();
            }
            else
            {
                Console.WriteLine($"State {stateName} not found in Animator.");
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

            currentState?.animationClip?.Update();
        }

        private void SetCurrentState(AnimationState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                currentState.animationClip.Play();
            }
        }

        public string GetCurrentSpritePath()
        {
            return currentState?.animationClip?.spritePath;
        }

        public void Play()
        {
            currentState?.animationClip.Play();
        }

        public void Stop()
        {
            currentState?.animationClip.Stop();
        }
    }
}
