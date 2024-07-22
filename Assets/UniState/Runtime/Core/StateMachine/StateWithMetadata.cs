using System;
using UniState;

namespace UniState
{
    public class StateWithMetadata : IDisposable
    {
        public IExecutableState State { get; private set; }
        public StateBehaviourData BehaviourData { get; private set; }

        private bool _disposed = false;

        public bool IsEmpty => State == null;

        public void SetData(IExecutableState state, StateBehaviourData behaviourData)
        {
            State = state;
            BehaviourData = behaviourData;
            _disposed = false;
        }

        public void SetData(StateWithMetadata data)
        {
            if (data != null)
            {
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