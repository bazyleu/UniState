using Examples.States;
using UniState;
using VContainer;
using VContainer.Unity;

namespace Examples.Infrastructure
{
    public class DiceScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<DiceEntryPoint>();

            builder.RegisterStateMachine<IStateMachine, StateMachine>();

            builder.RegisterState<StartGameState>();
            builder.RegisterState<RollDiceState>();
            builder.RegisterState<LostState>();
            builder.RegisterState<WinState>();
        }
    }
}