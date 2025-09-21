using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.States
{
    public class WinState : StateBase
    {
        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Debug.Log("Congratulations! You won this game!");

            return UniTask.FromResult(Transition.GoToExit());
        }
    }
}