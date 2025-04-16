using System;

namespace UniState
{
    public interface IStateCreator
    {
        public Type StateType { get; }
        IExecutableState Create();
    }
}