using System;
using Shard.StateMachine;

namespace Shard.Animation
{
    class AnimationState: State
    {
        public Animation animationClip { get; private set; }

        public Action OnEnter { get; set; }
        public Action OnExit { get; set; }

        public AnimationState(string name, Animation animation)
        {
            stateName = name;
            animationClip = animation;
        }
    }
}
