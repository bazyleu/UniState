using System;
using UniState;

namespace UniState
{
    public class StateWithMetadata : IDisposable
    {
        public StateTransitionInfo TransitionInfo { get; private set; }
        public IExecutableState State { get; private set; }
        public StateBehaviourData BehaviourData { get; private set; }

        private bool _disposed;

        public bool IsEmpty => State == null;

        public void BuildState(StateTransitionInfo transitionInfo, StateBehaviourData behaviourData)
        {
            TransitionInfo = transitionInfo;
            State = transitionInfo.Creator.Create();
            BehaviourData = behaviourData;

            _disposed = false;
        }

        public void CopyData(StateWithMetadata data)
        {
            if (data != null)
            {
                TransitionInfo = data.TransitionInfo;
                State = data.State;
                BehaviourData = data.BehaviourData;
            }
            else
            {
                Clear();
            }

            _disposed = false;
        }

        public void Clear()
        {
            TransitionInfo = null;
            State = null;
            BehaviourData = null;
            _disposed = false;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                State?.Dispose();
            }
        }
    }
}