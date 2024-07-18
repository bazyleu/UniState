using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace UniStateTests.PlayMode.States
{
    public abstract class Test3StateAbstract : StateBase
    {

    }

    public class Test3State : Test3StateAbstract
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("++ Test3State ++ ");

            return UniTask.FromResult(Transition.GoTo<ITest4State>());
        }
    }
}