#if UNISTATE_ZENJECT_SUPPORT

using System;
using Zenject;

namespace UniState
{
    public static class ZenjectContainerExtenstion
    {
        public static ConcreteIdArgConditionCopyNonLazyBinder BindStateMachine<TStateMachine>(
            this DiContainer container)
            where TStateMachine : IStateMachine =>
            container.Bind<TStateMachine>().ToSelf().AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindAbstractStateMachine<TInterface, TStateMachine>(
            this DiContainer container)
            where TStateMachine : TInterface
            where TInterface : IStateMachine =>
            container.Bind<TInterface>().To<TStateMachine>().AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindState<TState>(
            this DiContainer container) =>
            container.BindInterfacesAndSelfTo<TState>().AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindState(
            this DiContainer container, Type type)
            => container.BindInterfacesAndSelfTo(type).AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindAbstractState<TInterface, TState>(
            this DiContainer container)
            where TState : TInterface =>
            container.Bind<TInterface>().To<TState>().AsTransient();
    }
}

#endif