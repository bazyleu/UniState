using System.Threading;
using Cysharp.Threading.Tasks;
using Examples.States;
using UniState;

namespace Examples.Infrastructure.Reflex
{
    public class DiceEntryPoint
    {
        private readonly IStateMachine _stateMachine;
        
        public DiceEntryPoint(IStateMachine stateMachine) =>  _stateMachine = stateMachine;

        public void Start()
        {
            _stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
}