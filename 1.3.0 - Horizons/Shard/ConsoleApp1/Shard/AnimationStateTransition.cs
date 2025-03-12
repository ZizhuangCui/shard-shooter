using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Animation
{
    class AnimationStateTransition
    {
        public AnimationState FromState { get; private set; }
        public AnimationState ToState { get; private set; }
        public Func<bool> Condition { get; private set; }

        public AnimationStateTransition(AnimationState fromState, AnimationState toState, Func<bool> condition)
        {
            FromState = fromState;
            ToState = toState;
            Condition = condition;
        }
    }
}
