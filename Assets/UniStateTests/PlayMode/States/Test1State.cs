using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace UniStateTests.PlayMode.States
{
    [StateBehaviour(ProhibitReturnToState = true, InitializeOnStateTransition = true)]
    public class Test1State: StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("++ Test1State ++ ");

            return UniTask.FromResult(Transition.GoTo<Test2State>());
        }
    }
}