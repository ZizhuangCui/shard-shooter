using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shard.StateMachine;

namespace Shard.Animation
{
    class Animator : StateMachine<AnimationState>
    {
        protected override void OnStateEnter(AnimationState state)
        {
            state.animationClip.Play();
        }

        protected override void OnStateUpdate(AnimationState state)
        {
            state.animationClip.Update();
        }

        protected override void OnStateExit(AnimationState state)
        {
            state.animationClip.Stop();
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
