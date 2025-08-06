#if UNISTATE_ZENJECT_SUPPORT

using System;
using Zenject;

namespace UniState
{
    public static class ZenjectContainerExtenstion
    {
        public static void BindStateMachine<TInterface, TStateMachine>(
            this DiContainer container)
            where TStateMachine : TInterface
            where TInterface : IStateMachine => container.BindStateMachineInternal<TInterface, TStateMachine>(false);

        public static void BindStateMachineAsSingle<TInterface, TStateMachine>(
            this DiContainer container)
            where TStateMachine : TInterface
            where TInterface : IStateMachine => container.BindStateMachineInternal<TInterface, TStateMachine>(true);

        public static ConcreteIdArgConditionCopyNonLazyBinder BindState<TState>(
            this DiContainer container)
            where TState : IExecutableState =>
            container.BindInterfacesAndSelfTo<TState>().AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindStateAsSingle<TState>(
            this DiContainer container)
            where TState : IExecutableState =>
            container.BindInterfacesAndSelfTo<TState>().AsSingle();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindState<TInterface, TState>(
            this DiContainer container)
            where TState : TInterface, IExecutableState =>
            container.Bind<TInterface>().To<TState>().AsTransient();

        public static ConcreteIdArgConditionCopyNonLazyBinder BindStateAsSingle<TInterface, TState>(
            this DiContainer container)
            where TState : TInterface, IExecutableState =>
            container.Bind<TInterface>().To<TState>().AsSingle();

        private static void BindStateMachineInternal<TInterface, TStateMachine>(
            this DiContainer container,
            bool bindAsSingle)
            where TStateMachine : TInterface
            where TInterface : IStateMachine
        {
            if (typeof(TInterface) == typeof(TStateMachine))
                throw new ArgumentException(
                    $"BindStateMachine<{typeof(TInterface).Name}>: Type parameters must differ : use BindStateMachine<Interface, Implementation>() where Implementation implements Interface.\");");

            var interfaceBinder = container.Bind<TInterface>().FromMethod(ctx =>
            {
                var diContainer = ctx.Container;
                var stateMachine = diContainer.Resolve<TStateMachine>();

                stateMachine.SetResolver(diContainer.ToTypeResolver());

                return stateMachine;
            });

            if (bindAsSingle)
            {
                container.Bind<TStateMachine>().AsSingle();
                interfaceBinder.AsSingle();
            }
            else
            {
                container.Bind<TStateMachine>().AsTransient();
                interfaceBinder.AsTransient();
            }
        }
    }
}

#endif