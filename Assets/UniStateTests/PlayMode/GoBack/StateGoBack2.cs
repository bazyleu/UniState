using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.PlayMode.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack2 : StateBase
    {
        private readonly IStateLogger _stateLogger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack2(IStateLogger stateLogger, GoBackTestFlags flags)
        {
            _stateLogger = stateLogger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _stateLogger.LogLine("StateGoBack2 - Execute");

            await UniTask.Yield();

            if (_flags.ExecutedState2)
            {
                return Transition.GoBack();
            }

            _flags.ExecutedState2 = true;

            return Transition.GoTo<StateGoBack3>();
        }
    }
}