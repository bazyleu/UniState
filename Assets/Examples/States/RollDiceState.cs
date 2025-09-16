using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Examples.States
{
    public class RollDiceState : StateBase
    {
        public override async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            Debug.Log("Need to roll 5+. Rolling the dice...");

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

            var dice = Random.Range(0, 7);

            Debug.Log($"Dice is {dice}");

            if (dice > 4)
            {
                return Transition.GoTo<WinState>();
            }

            return Transition.GoTo<LostState>();
        }
    }
}