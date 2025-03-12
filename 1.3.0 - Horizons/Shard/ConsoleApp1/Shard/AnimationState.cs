using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Animation
{
    class AnimationState
    {
        public string stateName { get; private set; }
        public Animation animationClip { get; private set; }

        public AnimationState(string name, Animation animation)
        {
            stateName = name;
            animationClip = animation;
        }
    }
}
