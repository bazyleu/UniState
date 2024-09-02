using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.PlayMode.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack3 : StateBase
    {
        private readonly IStateLogger _stateLogger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack3(IStateLogger stateLogger, GoBackTestFlags flags)
        {
            _stateLogger = stateLogger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _stateLogger.LogLine("StateGoBack3 - Execute");

            await UniTask.Yield();
            
            return Transition.GoBack();
        }
    }
}