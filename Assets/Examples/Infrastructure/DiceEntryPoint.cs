using System.Threading;
using Cysharp.Threading.Tasks;
using Examples.States;
using UniState;
using VContainer;
using VContainer.Unity;

namespace Examples.Infrastructure
{
    public class DiceEntryPoint : IStartable
    {
        private readonly IObjectResolver _objectResolver;

        public DiceEntryPoint(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Start()
        {
            var stateMachine =  StateMachineHelper.CreateStateMachine<StateMachine>(_objectResolver.ToTypeResolver());

            stateMachine.Execute<StartGameState>(CancellationToken.None).Forget();
        }
    }
}