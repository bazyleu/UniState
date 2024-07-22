using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace UniStateTests.PlayMode.States
{
    public interface ITest4State : IState<EmptyPayload>
    {
    }

    public class Test4State : StateBase, ITest4State
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("++ Test4State ++ ");

            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}