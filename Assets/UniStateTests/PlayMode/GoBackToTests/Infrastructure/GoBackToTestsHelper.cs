using System.Collections.Generic;
using UniState;

namespace UniStateTests.PlayMode.GoBackToTests.Infrastructure
{
    public class GoBackToTestsHelper
    {
        public readonly struct TransitionTuple
        {
            public readonly string Log;
            public readonly StateTransitionInfo Transition;

            public TransitionTuple(string log, StateTransitionInfo transition)
            {
                Log = log;
                Transition = transition;
            }
        }

        public readonly Stack<TransitionTuple> TransitionsStack = new(4);
        public string ExpectedLog;
    }
}