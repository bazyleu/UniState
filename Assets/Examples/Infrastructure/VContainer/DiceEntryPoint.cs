using System.Threading;
using Cysharp.Threading.Tasks;
using Examples.States;
using UniState;
using VContainer.Unity;

namespace Examples.Infrastructure.VContainer
{
    public class DiceEntryPoint : IStartable
    {
        private readonly IStateMachine _stateMachine;

        public DiceEntryPoint(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            _stateMachine.ExecuteAsync<StartGameState>(CancellationToken.None).Forget();
        }
    }
}