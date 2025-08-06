using Examples.States;
using Reflex.Attributes;
using Reflex.Core;
using UniState;
using UnityEngine;

namespace Examples.Infrastructure.Reflex
{
    public class DiceInstaller : MonoBehaviour, IInstaller
    {
        [Inject]
        private readonly DiceEntryPoint _entryPoint;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.AddStateMachine(typeof(StateMachine), typeof(IStateMachine));

            builder.AddState(typeof(LostState));
            builder.AddState(typeof(RollDiceState));
            builder.AddState(typeof(StartGameState));
            builder.AddState(typeof(WinState));

            builder.AddSingleton(container =>
            {
                DiceEntryPoint entryPoint = new(container.Resolve<IStateMachine>());
                entryPoint.Start();

                return entryPoint;
            });
        }
    }
}