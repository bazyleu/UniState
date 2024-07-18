using UniState;

namespace UniState
{
    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly ITypeResolver _currentResolver;

        public StateMachineFactory(ITypeResolver currentResolver)
        {
            _currentResolver = currentResolver;
        }

        public IExecutableStateMachine Create<TSateMachine>(ITypeResolver typeResolver)
            where TSateMachine : class, IStateMachine =>
            StateMachineHelper.CreateStateMachine<TSateMachine>(typeResolver);

        public IExecutableStateMachine Create<TSateMachine>()
            where TSateMachine : class, IStateMachine =>
            StateMachineHelper.CreateStateMachine<TSateMachine>(_currentResolver);

        //TODO: Move under compilation flags
        public IExecutableStateMachine Create<TSateMachine>(VContainer.IObjectResolver objectResolver)
            where TSateMachine : class, IStateMachine =>
            StateMachineHelper.CreateStateMachine<TSateMachine>(new VContainerTypeResolver(objectResolver));
    }
}