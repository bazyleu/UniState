using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.States
{
    public class LostState : StateBase
    {
        public override async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            Debug.Log("You lost. You will have a another chance in...");

            Debug.Log("3 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            Debug.Log("2 seconds");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            Debug.Log("1 second");
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);

            return Transition.GoBack();
        }
    }
}