using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UniStateTests.PlayMode.Common;

namespace UniStateTests.PlayMode.GoBack
{
    public class StateGoBack1 : StateBase
    {
        private readonly IStateLogger _stateLogger;
        private readonly GoBackTestFlags _flags;

        public StateGoBack1(IStateLogger stateLogger, GoBackTestFlags flags)
        {
            _stateLogger = stateLogger;
            _flags = flags;
        }

        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            _stateLogger.LogLine("StateGoBack1 - Execute");

            await UniTask.Yield();

            if (_flags.ExecutedState1)
            {
                return Transition.GoBack();
            }

            _flags.ExecutedState1 = true;

            return Transition.GoTo<StateGoBack2>();
        }
    }
}