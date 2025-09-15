using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;

namespace Examples.States
{
    internal class StartGameState : StateBase
    {
        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Debug.Log("Welcome to the game! You game will be loaded in 2 seconds!");

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

            return Transition.GoTo<RollDiceState>();
        }
    }
}