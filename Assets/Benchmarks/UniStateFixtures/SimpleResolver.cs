using System;
using Benchmarks.Common;
using UniState;

namespace Benchmarks.UniStateFixtures
{
    public class SimpleResolver : ITypeResolver
    {
        private readonly BenchmarkHelper _benchmarkHelper;

        private BarState _barState;
        private FooState _fooState;
        private StateMachine _stateMachine;
        private StateMachineWithoutHistory _stateMachineWithoutHistory;

        public SimpleResolver(BenchmarkHelper benchmarkHelper)
        {
            _benchmarkHelper = benchmarkHelper;
        }

        public object Resolve(Type type)
        {
            if (typeof(BarState) == type)
            {
                return _barState ??= new BarState(_benchmarkHelper);
            }

            if (typeof(FooState) == type)
            {
                return _fooState ??= new FooState(_benchmarkHelper);
            }

            if (typeof(StateMachine) == type)
            {
                return _stateMachine ??= new StateMachine();
            }

            if (typeof(StateMachineWithoutHistory) == type)
            {
                return _stateMachineWithoutHistory ??= new StateMachineWithoutHistory();
            }

            return null;
        }
    }
}