using UniState;

namespace Benchmarks.UniStateFixtures
{
    public class StateMachineWithoutHistory : StateMachine
    {
        protected override int MaxHistorySize => 0;
    }
}