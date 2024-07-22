using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace UniStateTests.PlayMode.States
{
    public class Test2State : StateBase
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("++ Test2State ++ ");

            return UniTask.FromResult(Transition.GoTo<Test3StateAbstract>());
        }
    }
}