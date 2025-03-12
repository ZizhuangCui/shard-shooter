using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shard.StateMachine;

namespace Shard.Animation
{
    class AnimationState: State
    {
        public Animation animationClip { get; private set; }

        public AnimationState(string name, Animation animation)
        {
            stateName = name;
            animationClip = animation;
        }
    }
}
